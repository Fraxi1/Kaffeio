/* eslint-disable @typescript-eslint/no-unnecessary-type-assertion */
import { Injectable, NotFoundException, ConflictException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Lot } from '../schema/entities/lots.entity';
import { CreateLotDto } from '../schema/dto/create-lot.dto';
import { UpdateLotDto } from '../schema/dto/update-lot.dto';
import { Machine } from '../schema/entities/machine.entity';

@Injectable()
export class LotService {
    constructor(
        @InjectRepository(Lot)
        private lotRepository: Repository<Lot>,
        @InjectRepository(Machine)
        private machineRepository: Repository<Machine>,
    ) { }

    /**
     * Creates a new lot
     * @param createLotDto - Lot creation data
     * @returns Created lot object
     */
    async createLot(createLotDto: CreateLotDto): Promise<Lot> {
        // Sanitize input to prevent injection attacks
        const sanitizedDto = this.sanitizeInput<CreateLotDto>(createLotDto);

        // Check if lot with this code already exists
        const existingLot = await this.lotRepository.findOne({
            where: { Code: sanitizedDto.Code }
        });

        if (existingLot) {
            throw new ConflictException(`Lot with code ${sanitizedDto.Code} already exists`);
        }

        // If CurrentMachineId is provided, check if machine exists
        if (sanitizedDto.CurrentMachineId) {
            const machine = await this.machineRepository.findOne({
                where: { id: sanitizedDto.CurrentMachineId }
            });
            
            if (!machine) {
                throw new NotFoundException(`Machine with ID ${sanitizedDto.CurrentMachineId} not found`);
            }
        }

        // Create new lot
        const newLot = this.lotRepository.create(sanitizedDto);

        // Save lot to database
        return this.lotRepository.save(newLot);
    }

    /**
     * Get all lots
     * @returns List of all lots
     */
    async getAllLots(): Promise<Lot[]> {
        return this.lotRepository.find({
            relations: ['CurrentMachine']
        });
    }

    /**
     * Get lot by ID
     * @param id - Lot ID
     * @returns Lot data
     */
    async getLotById(id: string): Promise<Lot> {
        const lotId = parseInt(id, 10);
        if (isNaN(lotId)) {
            throw new BadRequestException('Invalid lot ID');
        }

        const lot = await this.lotRepository.findOne({
            where: { Id: lotId },
            relations: ['CurrentMachine']
        });

        if (!lot) {
            throw new NotFoundException(`Lot with ID ${id} not found`);
        }

        return lot;
    }

    /**
     * Get lot by code
     * @param code - Lot code
     * @returns Lot data
     */
    async getLotByCode(code: string): Promise<Lot> {
        const lot = await this.lotRepository.findOne({
            where: { Code: code },
            relations: ['CurrentMachine']
        });

        if (!lot) {
            throw new NotFoundException(`Lot with code ${code} not found`);
        }

        return lot;
    }

    /**
     * Updates an existing lot
     * @param id - Lot ID
     * @param updateLotDto - Lot update data
     * @returns Updated lot object
     */
    async updateLot(id: string, updateLotDto: UpdateLotDto): Promise<Lot> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('Lot ID is required');
        }

        // Convert string ID to number if needed
        const lotId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof lotId !== 'number' || isNaN(lotId)) {
            throw new BadRequestException('Invalid lot ID format');
        }

        // Sanitize input
        const sanitizedDto = this.sanitizeInput<UpdateLotDto>(updateLotDto);

        // Check if lot exists
        const existingLot = await this.lotRepository.findOne({ where: { Id: lotId } });
        if (!existingLot) {
            throw new NotFoundException(`Lot with ID ${id} not found`);
        }

        // If updating code, check if it's already in use by another lot
        if (sanitizedDto.Code) {
            const lotWithCode = await this.lotRepository.findOne({
                where: { Code: sanitizedDto.Code }
            });

            if (lotWithCode && lotWithCode.Id !== lotId) {
                throw new ConflictException(`Code ${sanitizedDto.Code} is already in use`);
            }
        }

        // If updating CurrentMachineId, check if machine exists
        if (sanitizedDto.CurrentMachineId) {
            const machine = await this.machineRepository.findOne({
                where: { id: sanitizedDto.CurrentMachineId }
            });
            
            if (!machine) {
                throw new NotFoundException(`Machine with ID ${sanitizedDto.CurrentMachineId} not found`);
            }
        }

        // Update lot
        await this.lotRepository.update(lotId, sanitizedDto);

        // Return updated lot
        const updatedLot = await this.lotRepository.findOne({
            where: { Id: lotId },
            relations: ['CurrentMachine']
        });

        if (!updatedLot) {
            throw new NotFoundException(`Lot with ID ${id} not found after update`);
        }

        return updatedLot;
    }

    /**
     * Deletes a lot
     * @param id - Lot ID
     * @returns Success message
     */
    async deleteLot(id: string): Promise<{ message: string }> {
        // Convert string ID to number if needed
        const lotId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof lotId !== 'number' || isNaN(lotId)) {
            throw new BadRequestException('Invalid lot ID format');
        }

        // Check if lot exists
        const lot = await this.lotRepository.findOne({ where: { Id: lotId } });
        if (!lot) {
            throw new NotFoundException(`Lot with ID ${id} not found`);
        }

        // Delete lot
        await this.lotRepository.delete(lotId);

        return { message: `Lot with ID ${id} has been deleted` };
    }

    /**
     * Sanitizes input to prevent injection attacks
     * @param input - Input data
     * @returns Sanitized input
     */
    private sanitizeInput<T>(input: T): T {
        // Create a shallow copy of the input
        const sanitized = { ...input } as T;

        // Sanitize string fields
        for (const key in sanitized) {
            if (Object.prototype.hasOwnProperty.call(sanitized, key)) {
                const value = (sanitized as Record<string, unknown>)[key];
                if (typeof value === 'string') {
                    // Remove potentially dangerous characters and scripts
                    const sanitizedValue = value
                        .replace(/</g, '&lt;')
                        .replace(/>/g, '&gt;')
                        .replace(/"/g, '&quot;')
                        .replace(/'/g, '&#x27;')
                        .replace(/\//g, '&#x2F;')
                        .replace(/`/g, '&#96;')
                        .trim();
                    (sanitized as Record<string, unknown>)[key] = sanitizedValue;
                }
            }
        }

        return sanitized;
    }
}
