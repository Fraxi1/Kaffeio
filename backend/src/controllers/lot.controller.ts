import { Controller, Body, Delete, Get, Param, Post, Put } from "@nestjs/common";
import { LotService } from "../services/lot.service";
import { CreateLotDto } from "../schema/dto/create-lot.dto";
import { UpdateLotDto } from "../schema/dto/update-lot.dto";

@Controller('lots')
export class LotController {
    constructor(private readonly lotService: LotService) { }

    @Post()
    async createLot(@Body() createLotDto: CreateLotDto) {
        return this.lotService.createLot(createLotDto);
    }

    @Get()
    async getAllLots() {
        return this.lotService.getAllLots();
    }

    @Get(':id')
    async getLotById(@Param('id') id: string) {
        return this.lotService.getLotById(id);
    }

    @Get('code/:code')
    async getLotByCode(@Param('code') code: string) {
        return this.lotService.getLotByCode(code);
    }

    @Put(':id')
    async updateLot(@Param('id') id: string, @Body() updateLotDto: UpdateLotDto) {
        return this.lotService.updateLot(id, updateLotDto);
    }

    @Delete(':id')
    async deleteLot(@Param('id') id: string) {
        return this.lotService.deleteLot(id);
    }
}
