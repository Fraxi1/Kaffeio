import { Controller, Body, Delete, Get, Param, Post, Put, Patch } from "@nestjs/common";
import { OrderService } from "../services/order.service";
import { CreateOrderDto } from "../schema/dto/create-order.dto";
import { UpdateOrderDto } from "../schema/dto/update-order.dto";

@Controller('orders')
export class OrderController {
    constructor(private readonly orderService: OrderService) { }

    @Post()
    async createOrder(@Body() createOrderDto: CreateOrderDto) {
        return this.orderService.createOrder(createOrderDto);
    }

    @Get()
    async getAllOrders() {
        return this.orderService.getAllOrders();
    }

    @Get('customer/:customerId')
    async getOrdersByCustomer(@Param('customerId') customerId: string) {
        return this.orderService.getOrdersByCustomer(customerId);
    }

    @Get('status/:status')
    async getOrdersByStatus(@Param('status') status: string) {
        return this.orderService.getOrdersByStatus(status);
    }

    @Get(':id')
    async getOrderById(@Param('id') id: string) {
        return this.orderService.getOrderById(id);
    }

    @Put(':id')
    async updateOrder(@Param('id') id: string, @Body() updateOrderDto: UpdateOrderDto) {
        return this.orderService.updateOrder(id, updateOrderDto);
    }

    @Patch(':id/status')
    async updateOrderStatus(@Param('id') id: string, @Body() body: { status: string }) {
        return this.orderService.updateOrderStatus(id, body.status);
    }

    @Delete(':id')
    async deleteOrder(@Param('id') id: string) {
        return this.orderService.deleteOrder(id);
    }
}
