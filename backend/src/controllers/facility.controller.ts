import { Controller, Body, Delete, Get, Param, Post, Put } from "@nestjs/common";
import { FacilityService } from "../services/facility.service";
import { CreateFacilityDto } from "../schema/dto/create-facility.dto";
import { UpdateFacilityDto } from "../schema/dto/update-facility.dto";

@Controller('facilities')
export class FacilityController {
    constructor(private readonly facilityService: FacilityService) { }

    @Post()
    async createFacility(@Body() createFacilityDto: CreateFacilityDto) {
        return this.facilityService.createFacility(createFacilityDto);
    }

    @Get()
    async getAllFacilities() {
        return this.facilityService.getAllFacilities();
    }

    @Get(':id')
    async getFacilityById(@Param('id') id: string) {
        return this.facilityService.getFacilityById(id);
    }

    @Put(':id')
    async updateFacility(@Param('id') id: string, @Body() updateFacilityDto: UpdateFacilityDto) {
        return this.facilityService.updateFacility(id, updateFacilityDto);
    }

    @Delete(':id')
    async deleteFacility(@Param('id') id: string) {
        return this.facilityService.deleteFacility(id);
    }
}
