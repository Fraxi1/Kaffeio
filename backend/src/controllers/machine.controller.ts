import { Controller, Body, Delete, Get, Param, Post, Put, Patch } from "@nestjs/common";
import { MachineService } from "../services/machine.service";
import { CreateMachineDto } from "../schema/dto/create-machine.dto";
import { UpdateMachineDto } from "../schema/dto/update-machine.dto";

@Controller('machines')
export class MachineController {
    constructor(private readonly machineService: MachineService) { }

    @Post()
    async createMachine(@Body() createMachineDto: CreateMachineDto) {
        return this.machineService.createMachine(createMachineDto);
    }

    @Get()
    async getAllMachines() {
        return this.machineService.getAllMachines();
    }

    @Get('facility/:facilityId')
    async getMachinesByFacility(@Param('facilityId') facilityId: string) {
        return this.machineService.getMachinesByFacility(facilityId);
    }

    @Get(':id')
    async getMachineById(@Param('id') id: string) {
        return this.machineService.getMachineById(id);
    }

    @Put(':id')
    async updateMachine(@Param('id') id: string, @Body() updateMachineDto: UpdateMachineDto) {
        return this.machineService.updateMachine(id, updateMachineDto);
    }

    @Patch(':id/status')
    async updateMachineStatus(@Param('id') id: string, @Body() body: { status: string }) {
        return this.machineService.updateMachineStatus(id, body.status);
    }

    @Delete(':id')
    async deleteMachine(@Param('id') id: string) {
        return this.machineService.deleteMachine(id);
    }
}
