/* eslint-disable @typescript-eslint/no-unnecessary-type-assertion */
import { Injectable, NotFoundException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository, Between } from 'typeorm';
import { Telemetry } from '../schema/entities/telemetry.entity';
import { Machine } from '../schema/entities/machine.entity';
import { Lot } from '../schema/entities/lots.entity';
import { CreateTelemetryDto } from '../schema/dto/create-telemetry.dto';
import { UpdateTelemetryDto } from '../schema/dto/update-telemetry.dto';

@Injectable()
export class TelemetryService {
    constructor(
        @InjectRepository(Telemetry)
        private telemetryRepository: Repository<Telemetry>,
        @InjectRepository(Machine)
        private machineRepository: Repository<Machine>,
        @InjectRepository(Lot)
        private lotRepository: Repository<Lot>,
    ) { }

    /**
     * Creates a new telemetry record
     * @param createTelemetryDto - Telemetry creation data
     * @returns Created telemetry object
     */
    async createTelemetry(createTelemetryDto: CreateTelemetryDto): Promise<Telemetry> {
        // Sanitize input to prevent injection attacks
        const sanitizedDto = this.sanitizeInput<CreateTelemetryDto>(createTelemetryDto);

        // Check if machine exists
        const machine = await this.machineRepository.findOne({
            where: { id: sanitizedDto.machineId }
        });

        if (!machine) {
            throw new NotFoundException(`Machine with ID ${sanitizedDto.machineId} not found`);
        }

        // Check if lot exists if provided
        if (sanitizedDto.lotId) {
            const lot = await this.lotRepository.findOne({
                where: { Id: sanitizedDto.lotId }
            });

            if (!lot) {
                throw new NotFoundException(`Lot with ID ${sanitizedDto.lotId} not found`);
            }
        }

        // Create new telemetry record
        const newTelemetry = this.telemetryRepository.create(sanitizedDto);

        // Save telemetry to database
        return this.telemetryRepository.save(newTelemetry);
    }

    /**
     * Get all telemetry records with pagination
     * @param page - Page number
     * @param limit - Records per page
     * @returns List of telemetry records
     */
    async getAllTelemetry(page = 1, limit = 100): Promise<{ data: Telemetry[], total: number, page: number, limit: number }> {
        const skip = (page - 1) * limit;

        const [data, total] = await this.telemetryRepository.findAndCount({
            relations: ['machine', 'lot'],
            skip,
            take: limit,
            order: { timestamp: 'DESC' }
        });

        return {
            data,
            total,
            page,
            limit
        };
    }

    /**
     * Get telemetry by ID
     * @param id - Telemetry ID
     * @returns Telemetry data
     */
    async getTelemetryById(id: string): Promise<Telemetry> {
        const telemetryId = parseInt(id, 10);
        if (isNaN(telemetryId)) {
            throw new BadRequestException('Invalid telemetry ID');
        }

        const telemetry = await this.telemetryRepository.findOne({
            where: { id: telemetryId },
            relations: ['machine', 'lot']
        });

        if (!telemetry) {
            throw new NotFoundException(`Telemetry with ID ${id} not found`);
        }

        return telemetry;
    }

    /**
     * Get telemetry by machine ID with pagination
     * @param machineId - Machine ID
     * @param page - Page number
     * @param limit - Records per page
     * @returns List of telemetry records for the machine
     */
    async getTelemetryByMachine(
        machineId: string,
        page = 1,
        limit = 100
    ): Promise<{ data: Telemetry[], total: number, page: number, limit: number }> {
        const machineIdNum = parseInt(machineId, 10);
        if (isNaN(machineIdNum)) {
            throw new BadRequestException('Invalid machine ID');
        }

        // Check if machine exists
        const machine = await this.machineRepository.findOne({
            where: { id: machineIdNum }
        });

        if (!machine) {
            throw new NotFoundException(`Machine with ID ${machineId} not found`);
        }

        const skip = (page - 1) * limit;

        const [data, total] = await this.telemetryRepository.findAndCount({
            where: { machineId: machineIdNum },
            relations: ['machine', 'lot'],
            skip,
            take: limit,
            order: { timestamp: 'DESC' }
        });

        return {
            data,
            total,
            page,
            limit
        };
    }

    /**
     * Get telemetry by lot ID with pagination
     * @param lotId - Lot ID
     * @param page - Page number
     * @param limit - Records per page
     * @returns List of telemetry records for the lot
     */
    async getTelemetryByLot(
        lotId: string,
        page = 1,
        limit = 100
    ): Promise<{ data: Telemetry[], total: number, page: number, limit: number }> {
        const lotIdNum = parseInt(lotId, 10);
        if (isNaN(lotIdNum)) {
            throw new BadRequestException('Invalid lot ID');
        }

        // Check if lot exists
        const lot = await this.lotRepository.findOne({
            where: { Id: lotIdNum }
        });

        if (!lot) {
            throw new NotFoundException(`Lot with ID ${lotId} not found`);
        }

        const skip = (page - 1) * limit;

        const [data, total] = await this.telemetryRepository.findAndCount({
            where: { lotId: lotIdNum },
            relations: ['machine', 'lot'],
            skip,
            take: limit,
            order: { timestamp: 'DESC' }
        });

        return {
            data,
            total,
            page,
            limit
        };
    }

    /**
     * Get telemetry by date range with pagination
     * @param startDate - Start date
     * @param endDate - End date
     * @param page - Page number
     * @param limit - Records per page
     * @returns List of telemetry records within date range
     */
    async getTelemetryByDateRange(
        startDate: string,
        endDate: string,
        page = 1,
        limit = 100
    ): Promise<{ data: Telemetry[], total: number, page: number, limit: number }> {
        const start = new Date(startDate);
        const end = new Date(endDate);

        if (isNaN(start.getTime()) || isNaN(end.getTime())) {
            throw new BadRequestException('Invalid date format');
        }

        const skip = (page - 1) * limit;

        const [data, total] = await this.telemetryRepository.findAndCount({
            where: {
                timestamp: Between(start, end)
            },
            relations: ['machine', 'lot'],
            skip,
            take: limit,
            order: { timestamp: 'DESC' }
        });

        return {
            data,
            total,
            page,
            limit
        };
    }

    /**
     * Updates an existing telemetry record
     * @param id - Telemetry ID
     * @param updateTelemetryDto - Telemetry update data
     * @returns Updated telemetry object
     */
    async updateTelemetry(id: string, updateTelemetryDto: UpdateTelemetryDto): Promise<Telemetry> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('Telemetry ID is required');
        }

        // Convert string ID to number if needed
        const telemetryId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof telemetryId !== 'number' || isNaN(telemetryId)) {
            throw new BadRequestException('Invalid telemetry ID format');
        }

        // Sanitize input
        const sanitizedDto = this.sanitizeInput<UpdateTelemetryDto>(updateTelemetryDto);

        // Check if telemetry exists
        const existingTelemetry = await this.telemetryRepository.findOne({ where: { id: telemetryId } });
        if (!existingTelemetry) {
            throw new NotFoundException(`Telemetry with ID ${id} not found`);
        }

        // If updating machineId, check if machine exists
        if (sanitizedDto.machineId) {
            const machine = await this.machineRepository.findOne({
                where: { id: sanitizedDto.machineId }
            });

            if (!machine) {
                throw new NotFoundException(`Machine with ID ${sanitizedDto.machineId} not found`);
            }
        }

        // If updating lotId, check if lot exists
        if (sanitizedDto.lotId) {
            const lot = await this.lotRepository.findOne({
                where: { Id: sanitizedDto.lotId }
            });

            if (!lot) {
                throw new NotFoundException(`Lot with ID ${sanitizedDto.lotId} not found`);
            }
        }

        // Update telemetry
        await this.telemetryRepository.update(telemetryId, sanitizedDto);

        // Return updated telemetry
        const updatedTelemetry = await this.telemetryRepository.findOne({
            where: { id: telemetryId },
            relations: ['machine', 'lot']
        });

        if (!updatedTelemetry) {
            throw new NotFoundException(`Telemetry with ID ${id} not found after update`);
        }

        return updatedTelemetry;
    }

    /**
     * Deletes a telemetry record
     * @param id - Telemetry ID
     * @returns Success message
     */
    async deleteTelemetry(id: string): Promise<{ message: string }> {
        // Convert string ID to number if needed
        const telemetryId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof telemetryId !== 'number' || isNaN(telemetryId)) {
            throw new BadRequestException('Invalid telemetry ID format');
        }

        // Check if telemetry exists
        const telemetry = await this.telemetryRepository.findOne({ where: { id: telemetryId } });
        if (!telemetry) {
            throw new NotFoundException(`Telemetry with ID ${id} not found`);
        }

        // Delete telemetry
        await this.telemetryRepository.delete(telemetryId);

        return { message: `Telemetry with ID ${id} has been deleted` };
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
