import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";
import { Machine } from "../schema/entities/machine.entity";
import { Facility } from "../schema/entities/facility.entity";
import { MachineService } from "../services/machine.service";
import { MachineController } from "../controllers/machine.controller";

@Module({
    imports: [TypeOrmModule.forFeature([Machine, Facility])],
    controllers: [MachineController],
    providers: [MachineService],
    exports: [MachineService],
})
export class MachineModule { }
