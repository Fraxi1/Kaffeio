import { Controller, Body, Delete, Get, Param, Post, Put, Query, Patch } from "@nestjs/common";
import { ProductionScheduleService } from "../services/production-schedule.service";
import { CreateProductionScheduleDto } from "../schema/dto/create-production-schedule.dto";
import { UpdateProductionScheduleDto } from "../schema/dto/update-production-schedule.dto";

@Controller('production-schedules')
export class ProductionScheduleController {
    constructor(private readonly scheduleService: ProductionScheduleService) { }

    @Post()
    async createSchedule(@Body() createScheduleDto: CreateProductionScheduleDto) {
        return this.scheduleService.createSchedule(createScheduleDto);
    }

    @Get()
    async getAllSchedules() {
        return this.scheduleService.getAllSchedules();
    }

    @Get('lot/:lotId')
    async getSchedulesByLot(@Param('lotId') lotId: string) {
        return this.scheduleService.getSchedulesByLot(lotId);
    }

    @Get('machine/:machineId')
    async getSchedulesByMachine(@Param('machineId') machineId: string) {
        return this.scheduleService.getSchedulesByMachine(machineId);
    }

    @Get('facility/:facilityId')
    async getSchedulesByFacility(@Param('facilityId') facilityId: string) {
        return this.scheduleService.getSchedulesByFacility(facilityId);
    }

    @Get('date-range')
    async getSchedulesByDateRange(
        @Query('startDate') startDate: string,
        @Query('endDate') endDate: string
    ) {
        return this.scheduleService.getSchedulesByDateRange(startDate, endDate);
    }

    @Get('status/:status')
    async getSchedulesByStatus(@Param('status') status: string) {
        return this.scheduleService.getSchedulesByStatus(status);
    }

    @Get(':id')
    async getScheduleById(@Param('id') id: string) {
        return this.scheduleService.getScheduleById(id);
    }

    @Put(':id')
    async updateSchedule(
        @Param('id') id: string,
        @Body() updateScheduleDto: UpdateProductionScheduleDto
    ) {
        return this.scheduleService.updateSchedule(id, updateScheduleDto);
    }

    @Patch(':id/status')
    async updateScheduleStatus(
        @Param('id') id: string,
        @Body() body: { status: string }
    ) {
        return this.scheduleService.updateScheduleStatus(id, body.status);
    }

    @Delete(':id')
    async deleteSchedule(@Param('id') id: string) {
        return this.scheduleService.deleteSchedule(id);
    }
}
