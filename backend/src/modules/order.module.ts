import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";
import { Order } from "../schema/entities/order.entity";
import { Customer } from "../schema/entities/customer.entity";
import { OrderService } from "../services/order.service";
import { OrderController } from "../controllers/order.controller";

@Module({
    imports: [TypeOrmModule.forFeature([Order, Customer])],
    controllers: [OrderController],
    providers: [OrderService],
    exports: [OrderService],
})
export class OrderModule { }
