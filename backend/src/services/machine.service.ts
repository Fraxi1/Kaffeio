/* eslint-disable @typescript-eslint/no-unnecessary-type-assertion */
import { Injectable, NotFoundException, ConflictException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Machine } from '../schema/entities/machine.entity';
import { CreateMachineDto } from '../schema/dto/create-machine.dto';
import { UpdateMachineDto } from '../schema/dto/update-machine.dto';
import { Facility } from '../schema/entities/facility.entity';

@Injectable()
export class MachineService {
    constructor(
        @InjectRepository(Machine)
        private machineRepository: Repository<Machine>,
        @InjectRepository(Facility)
        private facilityRepository: Repository<Facility>,
    ) { }

    /**
     * Creates a new machine
     * @param createMachineDto - Machine creation data
     * @returns Created machine object
     */
    async createMachine(createMachineDto: CreateMachineDto): Promise<Machine> {
        // Sanitize input to prevent injection attacks
        const sanitizedDto = this.sanitizeInput<CreateMachineDto>(createMachineDto);

        // Check if facility exists
        const facility = await this.facilityRepository.findOne({
            where: { id: sanitizedDto.facilityId }
        });

        if (!facility) {
            throw new NotFoundException(`Facility with ID ${sanitizedDto.facilityId} not found`);
        }

        // Create new machine
        const newMachine = this.machineRepository.create(sanitizedDto);

        // Save machine to database
        return this.machineRepository.save(newMachine);
    }

    /**
     * Get all machines
     * @returns List of all machines
     */
    async getAllMachines(): Promise<Machine[]> {
        return this.machineRepository.find({
            relations: ['facility']
        });
    }

    /**
     * Get machines by facility ID
     * @param facilityId - Facility ID
     * @returns List of machines in the facility
     */
    async getMachinesByFacility(facilityId: string): Promise<Machine[]> {
        const facilityIdNum = parseInt(facilityId, 10);
        if (isNaN(facilityIdNum)) {
            throw new BadRequestException('Invalid facility ID');
        }

        // Check if facility exists
        const facility = await this.facilityRepository.findOne({
            where: { id: facilityIdNum }
        });

        if (!facility) {
            throw new NotFoundException(`Facility with ID ${facilityId} not found`);
        }

        return this.machineRepository.find({
            where: { facilityId: facilityIdNum },
            relations: ['facility']
        });
    }

    /**
     * Get machine by ID
     * @param id - Machine ID
     * @returns Machine data
     */
    async getMachineById(id: string): Promise<Machine> {
        const machineId = parseInt(id, 10);
        if (isNaN(machineId)) {
            throw new BadRequestException('Invalid machine ID');
        }

        const machine = await this.machineRepository.findOne({
            where: { id: machineId },
            relations: ['facility']
        });

        if (!machine) {
            throw new NotFoundException(`Machine with ID ${id} not found`);
        }

        return machine;
    }

    /**
     * Updates an existing machine
     * @param id - Machine ID
     * @param updateMachineDto - Machine update data
     * @returns Updated machine object
     */
    async updateMachine(id: string, updateMachineDto: UpdateMachineDto): Promise<Machine> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('Machine ID is required');
        }

        // Convert string ID to number if needed
        const machineId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof machineId !== 'number' || isNaN(machineId)) {
            throw new BadRequestException('Invalid machine ID format');
        }

        // Sanitize input
        const sanitizedDto = this.sanitizeInput<UpdateMachineDto>(updateMachineDto);

        // Check if machine exists
        const existingMachine = await this.machineRepository.findOne({ where: { id: machineId } });
        if (!existingMachine) {
            throw new NotFoundException(`Machine with ID ${id} not found`);
        }

        // If updating facilityId, check if facility exists
        if (sanitizedDto.facilityId) {
            const facility = await this.facilityRepository.findOne({
                where: { id: sanitizedDto.facilityId }
            });

            if (!facility) {
                throw new NotFoundException(`Facility with ID ${sanitizedDto.facilityId} not found`);
            }
        }

        // Update machine
        await this.machineRepository.update(machineId, sanitizedDto);

        // Return updated machine
        const updatedMachine = await this.machineRepository.findOne({
            where: { id: machineId },
            relations: ['facility']
        });

        if (!updatedMachine) {
            throw new NotFoundException(`Machine with ID ${id} not found after update`);
        }

        return updatedMachine;
    }

    /**
     * Updates the status of a machine
     * @param id - Machine ID
     * @param status - New status
     * @returns Updated machine object
     */
    async updateMachineStatus(id: string, status: string): Promise<Machine> {
        // Validate ID
        const machineId = parseInt(id, 10);
        if (isNaN(machineId)) {
            throw new BadRequestException('Invalid machine ID');
        }

        // Check if machine exists
        const machine = await this.machineRepository.findOne({ where: { id: machineId } });
        if (!machine) {
            throw new NotFoundException(`Machine with ID ${id} not found`);
        }

        // Sanitize status
        const sanitizedStatus = this.sanitizeString(status);

        // Update machine status
        await this.machineRepository.update(machineId, { status: sanitizedStatus });

        // Return updated machine
        return this.getMachineById(id);
    }

    /**
     * Deletes a machine
     * @param id - Machine ID
     * @returns Success message
     */
    async deleteMachine(id: string): Promise<{ message: string }> {
        // Convert string ID to number if needed
        const machineId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof machineId !== 'number' || isNaN(machineId)) {
            throw new BadRequestException('Invalid machine ID format');
        }

        // Check if machine exists
        const machine = await this.machineRepository.findOne({ where: { id: machineId } });
        if (!machine) {
            throw new NotFoundException(`Machine with ID ${id} not found`);
        }

        // Delete machine
        await this.machineRepository.delete(machineId);

        return { message: `Machine with ID ${id} has been deleted` };
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
                    (sanitized as Record<string, unknown>)[key] = this.sanitizeString(value);
                }
            }
        }

        return sanitized;
    }

    /**
     * Sanitizes a string to prevent injection attacks
     * @param value - String to sanitize
     * @returns Sanitized string
     */
    private sanitizeString(value: string): string {
        return value
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#x27;')
            .replace(/\//g, '&#x2F;')
            .replace(/`/g, '&#96;')
            .trim();
    }
}
