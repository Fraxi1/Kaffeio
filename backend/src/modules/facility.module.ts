import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";
import { Facility } from "../schema/entities/facility.entity";
import { FacilityService } from "../services/facility.service";
import { FacilityController } from "../controllers/facility.controller";

@Module({
    imports: [TypeOrmModule.forFeature([Facility])],
    controllers: [FacilityController],
    providers: [FacilityService],
    exports: [FacilityService],
})
export class FacilityModule { }
