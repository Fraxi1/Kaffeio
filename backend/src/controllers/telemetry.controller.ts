import { Controller, Body, Delete, Get, Param, Post, Put, Query } from "@nestjs/common";
import { TelemetryService } from "../services/telemetry.service";
import { CreateTelemetryDto } from "../schema/dto/create-telemetry.dto";
import { UpdateTelemetryDto } from "../schema/dto/update-telemetry.dto";

@Controller('telemetry')
export class TelemetryController {
    constructor(private readonly telemetryService: TelemetryService) { }

    @Post()
    async createTelemetry(@Body() createTelemetryDto: CreateTelemetryDto) {
        return this.telemetryService.createTelemetry(createTelemetryDto);
    }

    @Get()
    async getAllTelemetry(
        @Query('page') page: number = 1,
        @Query('limit') limit: number = 100
    ) {
        return this.telemetryService.getAllTelemetry(page, limit);
    }

    @Get('machine/:machineId')
    async getTelemetryByMachine(
        @Param('machineId') machineId: string,
        @Query('page') page: number = 1,
        @Query('limit') limit: number = 100
    ) {
        return this.telemetryService.getTelemetryByMachine(machineId, page, limit);
    }

    @Get('lot/:lotId')
    async getTelemetryByLot(
        @Param('lotId') lotId: string,
        @Query('page') page: number = 1,
        @Query('limit') limit: number = 100
    ) {
        return this.telemetryService.getTelemetryByLot(lotId, page, limit);
    }

    @Get('daterange')
    async getTelemetryByDateRange(
        @Query('startDate') startDate: string,
        @Query('endDate') endDate: string,
        @Query('page') page: number = 1,
        @Query('limit') limit: number = 100
    ) {
        return this.telemetryService.getTelemetryByDateRange(startDate, endDate, page, limit);
    }

    @Get(':id')
    async getTelemetryById(@Param('id') id: string) {
        return this.telemetryService.getTelemetryById(id);
    }

    @Put(':id')
    async updateTelemetry(@Param('id') id: string, @Body() updateTelemetryDto: UpdateTelemetryDto) {
        return this.telemetryService.updateTelemetry(id, updateTelemetryDto);
    }

    @Delete(':id')
    async deleteTelemetry(@Param('id') id: string) {
        return this.telemetryService.deleteTelemetry(id);
    }
}
