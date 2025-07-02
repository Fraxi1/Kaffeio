import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";
import { Lot } from "../schema/entities/lots.entity";
import { Machine } from "../schema/entities/machine.entity";
import { LotService } from "../services/lot.service";
import { LotController } from "../controllers/lot.controller";

@Module({
    imports: [TypeOrmModule.forFeature([Lot, Machine])],
    controllers: [LotController],
    providers: [LotService],
    exports: [LotService],
})
export class LotModule { }
