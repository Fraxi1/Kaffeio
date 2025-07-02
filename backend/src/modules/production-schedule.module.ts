import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";
import { ProductionSchedule } from "../schema/entities/production-schedule.entity";
import { Machine } from "../schema/entities/machine.entity";
import { Lot } from "../schema/entities/lots.entity";
import { Facility } from "../schema/entities/facility.entity";
import { ProductionScheduleService } from "../services/production-schedule.service";
import { ProductionScheduleController } from "../controllers/production-schedule.controller";

@Module({
    imports: [TypeOrmModule.forFeature([ProductionSchedule, Machine, Lot, Facility])],
    controllers: [ProductionScheduleController],
    providers: [ProductionScheduleService],
    exports: [ProductionScheduleService],
})
export class ProductionScheduleModule { }
