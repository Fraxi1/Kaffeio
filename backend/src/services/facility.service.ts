/* eslint-disable @typescript-eslint/no-unnecessary-type-assertion */
import { Injectable, NotFoundException, ConflictException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Facility } from '../schema/entities/facility.entity';
import { CreateFacilityDto } from '../schema/dto/create-facility.dto';
import { UpdateFacilityDto } from '../schema/dto/update-facility.dto';

@Injectable()
export class FacilityService {
    constructor(
        @InjectRepository(Facility)
        private facilityRepository: Repository<Facility>,
    ) { }

    /**
     * Creates a new facility
     * @param createFacilityDto - Facility creation data
     * @returns Created facility object
     */
    async createFacility(createFacilityDto: CreateFacilityDto): Promise<Facility> {
        // Sanitize input to prevent injection attacks
        const sanitizedDto = this.sanitizeInput<CreateFacilityDto>(createFacilityDto);

        // Check if facility with this name already exists
        const existingFacility = await this.facilityRepository.findOne({
            where: { name: sanitizedDto.name }
        });

        if (existingFacility) {
            throw new ConflictException(`Facility with name ${sanitizedDto.name} already exists`);
        }

        // Create new facility
        const newFacility = this.facilityRepository.create(sanitizedDto);

        // Save facility to database
        return this.facilityRepository.save(newFacility);
    }

    /**
     * Get all facilities
     * @returns List of all facilities
     */
    async getAllFacilities(): Promise<Facility[]> {
        return this.facilityRepository.find();
    }

    /**
     * Get facility by ID
     * @param id - Facility ID
     * @returns Facility data
     */
    async getFacilityById(id: string): Promise<Facility> {
        const facilityId = parseInt(id, 10);
        if (isNaN(facilityId)) {
            throw new BadRequestException('Invalid facility ID');
        }

        const facility = await this.facilityRepository.findOne({
            where: { id: facilityId }
        });

        if (!facility) {
            throw new NotFoundException(`Facility with ID ${id} not found`);
        }

        return facility;
    }

    /**
     * Updates an existing facility
     * @param id - Facility ID
     * @param updateFacilityDto - Facility update data
     * @returns Updated facility object
     */
    async updateFacility(id: string, updateFacilityDto: UpdateFacilityDto): Promise<Facility> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('Facility ID is required');
        }

        // Convert string ID to number if needed
        const facilityId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof facilityId !== 'number' || isNaN(facilityId)) {
            throw new BadRequestException('Invalid facility ID format');
        }

        // Sanitize input
        const sanitizedDto = this.sanitizeInput<UpdateFacilityDto>(updateFacilityDto);

        // Check if facility exists
        const existingFacility = await this.facilityRepository.findOne({ where: { id: facilityId } });
        if (!existingFacility) {
            throw new NotFoundException(`Facility with ID ${id} not found`);
        }

        // If updating name, check if it's already in use by another facility
        if (sanitizedDto.name) {
            const facilityWithName = await this.facilityRepository.findOne({
                where: { name: sanitizedDto.name }
            });

            if (facilityWithName && facilityWithName.id !== facilityId) {
                throw new ConflictException(`Name ${sanitizedDto.name} is already in use`);
            }
        }

        // Update facility
        await this.facilityRepository.update(facilityId, sanitizedDto);

        // Return updated facility
        const updatedFacility = await this.facilityRepository.findOne({
            where: { id: facilityId }
        });

        if (!updatedFacility) {
            throw new NotFoundException(`Facility with ID ${id} not found after update`);
        }

        return updatedFacility;
    }

    /**
     * Deletes a facility
     * @param id - Facility ID
     * @returns Success message
     */
    async deleteFacility(id: string): Promise<{ message: string }> {
        // Convert string ID to number if needed
        const facilityId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof facilityId !== 'number' || isNaN(facilityId)) {
            throw new BadRequestException('Invalid facility ID format');
        }

        // Check if facility exists
        const facility = await this.facilityRepository.findOne({ where: { id: facilityId } });
        if (!facility) {
            throw new NotFoundException(`Facility with ID ${id} not found`);
        }

        // Delete facility
        await this.facilityRepository.delete(facilityId);

        return { message: `Facility with ID ${id} has been deleted` };
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
