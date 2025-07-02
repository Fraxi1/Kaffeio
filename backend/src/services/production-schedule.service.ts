/* eslint-disable @typescript-eslint/no-unnecessary-type-assertion */
import { Injectable, NotFoundException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository, Between } from 'typeorm';
import { ProductionSchedule } from '../schema/entities/production-schedule.entity';
import { Machine } from '../schema/entities/machine.entity';
import { Lot } from '../schema/entities/lots.entity';
import { Facility } from '../schema/entities/facility.entity';
import { CreateProductionScheduleDto } from '../schema/dto/create-production-schedule.dto';
import { UpdateProductionScheduleDto } from '../schema/dto/update-production-schedule.dto';

@Injectable()
export class ProductionScheduleService {
    constructor(
        @InjectRepository(ProductionSchedule)
        private scheduleRepository: Repository<ProductionSchedule>,
        @InjectRepository(Machine)
        private machineRepository: Repository<Machine>,
        @InjectRepository(Lot)
        private lotRepository: Repository<Lot>,
        @InjectRepository(Facility)
        private facilityRepository: Repository<Facility>,
    ) { }

    /**
     * Creates a new production schedule entry
     * @param createScheduleDto - Production schedule creation data
     * @returns Created production schedule object
     */
    async createSchedule(createScheduleDto: CreateProductionScheduleDto): Promise<ProductionSchedule> {
        // Sanitize input to prevent injection attacks
        const sanitizedDto = this.sanitizeInput<CreateProductionScheduleDto>(createScheduleDto);

        // Check if lot exists
        const lot = await this.lotRepository.findOne({
            where: { Id: sanitizedDto.lotId }
        });

        if (!lot) {
            throw new NotFoundException(`Lot with ID ${sanitizedDto.lotId} not found`);
        }

        // Check if machine exists
        const machine = await this.machineRepository.findOne({
            where: { id: sanitizedDto.machineId }
        });

        if (!machine) {
            throw new NotFoundException(`Machine with ID ${sanitizedDto.machineId} not found`);
        }

        // Check if facility exists
        const facility = await this.facilityRepository.findOne({
            where: { id: sanitizedDto.facilityId }
        });

        if (!facility) {
            throw new NotFoundException(`Facility with ID ${sanitizedDto.facilityId} not found`);
        }

        // Check for scheduling conflicts
        await this.checkSchedulingConflicts(
            sanitizedDto.machineId,
            sanitizedDto.scheduledStartTime,
            sanitizedDto.scheduledEndTime,
            null
        );

        // Create new schedule entry
        const newSchedule = this.scheduleRepository.create(sanitizedDto);

        // Save schedule to database
        return this.scheduleRepository.save(newSchedule);
    }

    /**
     * Get all production schedules
     * @returns List of production schedules
     */
    async getAllSchedules(): Promise<ProductionSchedule[]> {
        return this.scheduleRepository.find({
            relations: ['lot', 'machine', 'facility'],
            order: { scheduledStartTime: 'ASC' }
        });
    }

    /**
     * Get production schedule by ID
     * @param id - Production schedule ID
     * @returns Production schedule data
     */
    async getScheduleById(id: string): Promise<ProductionSchedule> {
        const scheduleId = parseInt(id, 10);
        if (isNaN(scheduleId)) {
            throw new BadRequestException('Invalid production schedule ID');
        }

        const schedule = await this.scheduleRepository.findOne({
            where: { id: scheduleId },
            relations: ['lot', 'machine', 'facility']
        });

        if (!schedule) {
            throw new NotFoundException(`Production schedule with ID ${id} not found`);
        }

        return schedule;
    }

    /**
     * Get production schedules by lot ID
     * @param lotId - Lot ID
     * @returns List of production schedules for the lot
     */
    async getSchedulesByLot(lotId: string): Promise<ProductionSchedule[]> {
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

        return this.scheduleRepository.find({
            where: { lotId: lotIdNum },
            relations: ['lot', 'machine', 'facility'],
            order: { scheduledStartTime: 'ASC' }
        });
    }

    /**
     * Get production schedules by machine ID
     * @param machineId - Machine ID
     * @returns List of production schedules for the machine
     */
    async getSchedulesByMachine(machineId: string): Promise<ProductionSchedule[]> {
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

        return this.scheduleRepository.find({
            where: { machineId: machineIdNum },
            relations: ['lot', 'machine', 'facility'],
            order: { scheduledStartTime: 'ASC' }
        });
    }

    /**
     * Get production schedules by facility ID
     * @param facilityId - Facility ID
     * @returns List of production schedules for the facility
     */
    async getSchedulesByFacility(facilityId: string): Promise<ProductionSchedule[]> {
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

        return this.scheduleRepository.find({
            where: { facilityId: facilityIdNum },
            relations: ['lot', 'machine', 'facility'],
            order: { scheduledStartTime: 'ASC' }
        });
    }

    /**
     * Get production schedules by date range
     * @param startDate - Start date
     * @param endDate - End date
     * @returns List of production schedules within date range
     */
    async getSchedulesByDateRange(startDate: string, endDate: string): Promise<ProductionSchedule[]> {
        const start = new Date(startDate);
        const end = new Date(endDate);

        if (isNaN(start.getTime()) || isNaN(end.getTime())) {
            throw new BadRequestException('Invalid date format');
        }

        return this.scheduleRepository.find({
            where: {
                scheduledStartTime: Between(start, end)
            },
            relations: ['lot', 'machine', 'facility'],
            order: { scheduledStartTime: 'ASC' }
        });
    }

    /**
     * Get production schedules by status
     * @param status - Schedule status
     * @returns List of production schedules with the specified status
     */
    async getSchedulesByStatus(status: string): Promise<ProductionSchedule[]> {
        return this.scheduleRepository.find({
            where: { status },
            relations: ['lot', 'machine', 'facility'],
            order: { scheduledStartTime: 'ASC' }
        });
    }

    /**
     * Updates an existing production schedule
     * @param id - Production schedule ID
     * @param updateScheduleDto - Production schedule update data
     * @returns Updated production schedule object
     */
    async updateSchedule(id: string, updateScheduleDto: UpdateProductionScheduleDto): Promise<ProductionSchedule> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('Production schedule ID is required');
        }

        // Convert string ID to number if needed
        const scheduleId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof scheduleId !== 'number' || isNaN(scheduleId)) {
            throw new BadRequestException('Invalid production schedule ID format');
        }

        // Sanitize input
        const sanitizedDto = this.sanitizeInput<UpdateProductionScheduleDto>(updateScheduleDto);

        // Check if schedule exists
        const existingSchedule = await this.scheduleRepository.findOne({ where: { id: scheduleId } });
        if (!existingSchedule) {
            throw new NotFoundException(`Production schedule with ID ${id} not found`);
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

        // If updating machineId, check if machine exists
        if (sanitizedDto.machineId) {
            const machine = await this.machineRepository.findOne({
                where: { id: sanitizedDto.machineId }
            });

            if (!machine) {
                throw new NotFoundException(`Machine with ID ${sanitizedDto.machineId} not found`);
            }
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

        // Check for scheduling conflicts if updating times or machine
        if (sanitizedDto.scheduledStartTime || sanitizedDto.scheduledEndTime || sanitizedDto.machineId) {
            const machineId = sanitizedDto.machineId || existingSchedule.machineId;
            const startTime = sanitizedDto.scheduledStartTime || existingSchedule.scheduledStartTime;
            const endTime = sanitizedDto.scheduledEndTime || existingSchedule.scheduledEndTime;

            await this.checkSchedulingConflicts(machineId, startTime, endTime, scheduleId);
        }

        // Update schedule
        await this.scheduleRepository.update(scheduleId, sanitizedDto);

        // Return updated schedule
        const updatedSchedule = await this.scheduleRepository.findOne({
            where: { id: scheduleId },
            relations: ['lot', 'machine', 'facility']
        });

        if (!updatedSchedule) {
            throw new NotFoundException(`Production schedule with ID ${id} not found after update`);
        }

        return updatedSchedule;
    }

    /**
     * Updates the status of a production schedule
     * @param id - Production schedule ID
     * @param status - New status
     * @returns Updated production schedule object
     */
    async updateScheduleStatus(id: string, status: string): Promise<ProductionSchedule> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('Production schedule ID is required');
        }

        // Convert string ID to number if needed
        const scheduleId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof scheduleId !== 'number' || isNaN(scheduleId)) {
            throw new BadRequestException('Invalid production schedule ID format');
        }

        // Validate status
        const validStatuses = ['Scheduled', 'InProgress', 'Completed', 'Cancelled'];
        if (!validStatuses.includes(status)) {
            throw new BadRequestException(`Invalid status. Must be one of: ${validStatuses.join(', ')}`);
        }

        // Check if schedule exists
        const existingSchedule = await this.scheduleRepository.findOne({ where: { id: scheduleId } });
        if (!existingSchedule) {
            throw new NotFoundException(`Production schedule with ID ${id} not found`);
        }

        // Update schedule status
        await this.scheduleRepository.update(scheduleId, { status });

        // Return updated schedule
        const updatedSchedule = await this.scheduleRepository.findOne({
            where: { id: scheduleId },
            relations: ['lot', 'machine', 'facility']
        });

        if (!updatedSchedule) {
            throw new NotFoundException(`Production schedule with ID ${id} not found after update`);
        }

        return updatedSchedule;
    }

    /**
     * Deletes a production schedule
     * @param id - Production schedule ID
     * @returns Success message
     */
    async deleteSchedule(id: string): Promise<{ message: string }> {
        // Convert string ID to number if needed
        const scheduleId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof scheduleId !== 'number' || isNaN(scheduleId)) {
            throw new BadRequestException('Invalid production schedule ID format');
        }

        // Check if schedule exists
        const schedule = await this.scheduleRepository.findOne({ where: { id: scheduleId } });
        if (!schedule) {
            throw new NotFoundException(`Production schedule with ID ${id} not found`);
        }

        // Delete schedule
        await this.scheduleRepository.delete(scheduleId);

        return { message: `Production schedule with ID ${id} has been deleted` };
    }

    /**
     * Check for scheduling conflicts
     * @param machineId - Machine ID
     * @param startTime - Scheduled start time
     * @param endTime - Scheduled end time
     * @param excludeScheduleId - Schedule ID to exclude from conflict check
     */
    private async checkSchedulingConflicts(
        machineId: number,
        startTime: Date,
        endTime: Date | null | undefined,
        excludeScheduleId: number | null
    ): Promise<void> {
        // If no end time is provided, assume a default duration (e.g., 1 hour)
        const effectiveEndTime = endTime || new Date(new Date(startTime).getTime() + 3600000);

        // Find schedules that overlap with the given time range for the specified machine
        const query = this.scheduleRepository
            .createQueryBuilder('schedule')
            .where('schedule.machineId = :machineId', { machineId })
            .andWhere(
                '(schedule.scheduledStartTime < :endTime AND (schedule.scheduledEndTime IS NULL OR schedule.scheduledEndTime > :startTime))',
                { startTime, endTime: effectiveEndTime }
            );

        // Exclude the current schedule if updating
        if (excludeScheduleId) {
            query.andWhere('schedule.id != :excludeScheduleId', { excludeScheduleId });
        }

        const conflictingSchedules = await query.getMany();

        if (conflictingSchedules.length > 0) {
            throw new BadRequestException(
                `Scheduling conflict detected. Machine #${machineId} is already scheduled during the requested time period.`
            );
        }
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
