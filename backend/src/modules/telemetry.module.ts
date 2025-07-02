import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";
import { Telemetry } from "../schema/entities/telemetry.entity";
import { Machine } from "../schema/entities/machine.entity";
import { Lot } from "../schema/entities/lots.entity";
import { TelemetryService } from "../services/telemetry.service";
import { TelemetryController } from "../controllers/telemetry.controller";

@Module({
    imports: [TypeOrmModule.forFeature([Telemetry, Machine, Lot])],
    controllers: [TelemetryController],
    providers: [TelemetryService],
    exports: [TelemetryService],
})
export class TelemetryModule { }
