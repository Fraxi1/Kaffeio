/* eslint-disable @typescript-eslint/no-unnecessary-type-assertion */
import { Injectable, NotFoundException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Order } from '../schema/entities/order.entity';
import { Customer } from '../schema/entities/customer.entity';
import { CreateOrderDto } from '../schema/dto/create-order.dto';
import { UpdateOrderDto } from '../schema/dto/update-order.dto';

@Injectable()
export class OrderService {
    constructor(
        @InjectRepository(Order)
        private orderRepository: Repository<Order>,
        @InjectRepository(Customer)
        private customerRepository: Repository<Customer>,
    ) { }

    /**
     * Creates a new order
     * @param createOrderDto - Order creation data
     * @returns Created order object
     */
    async createOrder(createOrderDto: CreateOrderDto): Promise<Order> {
        // Sanitize input to prevent injection attacks
        const sanitizedDto = this.sanitizeInput<CreateOrderDto>(createOrderDto);

        // Check if customer exists
        const customer = await this.customerRepository.findOne({
            where: { id: sanitizedDto.customerId }
        });

        if (!customer) {
            throw new NotFoundException(`Customer with ID ${sanitizedDto.customerId} not found`);
        }

        // Check if order number is unique
        const existingOrder = await this.orderRepository.findOne({
            where: { orderNumber: sanitizedDto.orderNumber }
        });

        if (existingOrder) {
            throw new BadRequestException(`Order with number ${sanitizedDto.orderNumber} already exists`);
        }

        // Create new order
        const newOrder = this.orderRepository.create(sanitizedDto);

        // Save order to database
        return this.orderRepository.save(newOrder);
    }

    /**
     * Get all orders
     * @returns List of orders
     */
    async getAllOrders(): Promise<Order[]> {
        return this.orderRepository.find({
            relations: ['customer', 'lots']
        });
    }

    /**
     * Get order by ID
     * @param id - Order ID
     * @returns Order data
     */
    async getOrderById(id: string): Promise<Order> {
        const orderId = parseInt(id, 10);
        if (isNaN(orderId)) {
            throw new BadRequestException('Invalid order ID');
        }

        const order = await this.orderRepository.findOne({
            where: { id: orderId },
            relations: ['customer', 'lots']
        });

        if (!order) {
            throw new NotFoundException(`Order with ID ${id} not found`);
        }

        return order;
    }

    /**
     * Get orders by customer ID
     * @param customerId - Customer ID
     * @returns List of orders for the customer
     */
    async getOrdersByCustomer(customerId: string): Promise<Order[]> {
        const customerIdNum = parseInt(customerId, 10);
        if (isNaN(customerIdNum)) {
            throw new BadRequestException('Invalid customer ID');
        }

        // Check if customer exists
        const customer = await this.customerRepository.findOne({
            where: { id: customerIdNum }
        });

        if (!customer) {
            throw new NotFoundException(`Customer with ID ${customerId} not found`);
        }

        return this.orderRepository.find({
            where: { customerId: customerIdNum },
            relations: ['customer', 'lots']
        });
    }

    /**
     * Get orders by status
     * @param status - Order status
     * @returns List of orders with the specified status
     */
    async getOrdersByStatus(status: string): Promise<Order[]> {
        return this.orderRepository.find({
            where: { status },
            relations: ['customer', 'lots']
        });
    }

    /**
     * Updates an existing order
     * @param id - Order ID
     * @param updateOrderDto - Order update data
     * @returns Updated order object
     */
    async updateOrder(id: string, updateOrderDto: UpdateOrderDto): Promise<Order> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('Order ID is required');
        }

        // Convert string ID to number if needed
        const orderId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof orderId !== 'number' || isNaN(orderId)) {
            throw new BadRequestException('Invalid order ID format');
        }

        // Sanitize input
        const sanitizedDto = this.sanitizeInput<UpdateOrderDto>(updateOrderDto);

        // Check if order exists
        const existingOrder = await this.orderRepository.findOne({ where: { id: orderId } });
        if (!existingOrder) {
            throw new NotFoundException(`Order with ID ${id} not found`);
        }

        // If updating customerId, check if customer exists
        if (sanitizedDto.customerId) {
            const customer = await this.customerRepository.findOne({
                where: { id: sanitizedDto.customerId }
            });

            if (!customer) {
                throw new NotFoundException(`Customer with ID ${sanitizedDto.customerId} not found`);
            }
        }

        // If updating orderNumber, check if it's unique
        if (sanitizedDto.orderNumber && sanitizedDto.orderNumber !== existingOrder.orderNumber) {
            const orderWithNumber = await this.orderRepository.findOne({
                where: { orderNumber: sanitizedDto.orderNumber }
            });

            if (orderWithNumber && orderWithNumber.id !== orderId) {
                throw new BadRequestException(`Order number ${sanitizedDto.orderNumber} is already in use`);
            }
        }

        // Update order
        await this.orderRepository.update(orderId, sanitizedDto);

        // Return updated order
        const updatedOrder = await this.orderRepository.findOne({
            where: { id: orderId },
            relations: ['customer', 'lots']
        });

        if (!updatedOrder) {
            throw new NotFoundException(`Order with ID ${id} not found after update`);
        }

        return updatedOrder;
    }

    /**
     * Updates the status of an order
     * @param id - Order ID
     * @param status - New status
     * @returns Updated order object
     */
    async updateOrderStatus(id: string, status: string): Promise<Order> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('Order ID is required');
        }

        // Convert string ID to number if needed
        const orderId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof orderId !== 'number' || isNaN(orderId)) {
            throw new BadRequestException('Invalid order ID format');
        }

        // Validate status
        const validStatuses = ['Pending', 'InProgress', 'Completed', 'Cancelled'];
        if (!validStatuses.includes(status)) {
            throw new BadRequestException(`Invalid status. Must be one of: ${validStatuses.join(', ')}`);
        }

        // Check if order exists
        const existingOrder = await this.orderRepository.findOne({ where: { id: orderId } });
        if (!existingOrder) {
            throw new NotFoundException(`Order with ID ${id} not found`);
        }

        // Update order status
        await this.orderRepository.update(orderId, { status });

        // Return updated order
        const updatedOrder = await this.orderRepository.findOne({
            where: { id: orderId },
            relations: ['customer', 'lots']
        });

        if (!updatedOrder) {
            throw new NotFoundException(`Order with ID ${id} not found after update`);
        }

        return updatedOrder;
    }

    /**
     * Deletes an order
     * @param id - Order ID
     * @returns Success message
     */
    async deleteOrder(id: string): Promise<{ message: string }> {
        // Convert string ID to number if needed
        const orderId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof orderId !== 'number' || isNaN(orderId)) {
            throw new BadRequestException('Invalid order ID format');
        }

        // Check if order exists
        const order = await this.orderRepository.findOne({
            where: { id: orderId },
            relations: ['lots']
        });

        if (!order) {
            throw new NotFoundException(`Order with ID ${id} not found`);
        }

        // Check if order has lots
        if (order.lots && order.lots.length > 0) {
            throw new BadRequestException(`Cannot delete order with ID ${id} because it has associated lots`);
        }

        // Delete order
        await this.orderRepository.delete(orderId);

        return { message: `Order with ID ${id} has been deleted` };
    }

    /**
     * Sanitizes input to prevent injection attacks
     * @param input - Input data
     * @returns Sanitized input
     */
    private sanitizeInput<T>(input: T): T {
        // Create a shallow copy of the input
        const sanitized = { ...input } as T;

        // Sanitize string fields
        for (const key in sanitized) {
            if (Object.prototype.hasOwnProperty.call(sanitized, key)) {
                const value = (sanitized as Record<string, unknown>)[key];
                if (typeof value === 'string') {
                    // Remove potentially dangerous characters and scripts
                    const sanitizedValue = value
                        .replace(/</g, '&lt;')
                        .replace(/>/g, '&gt;')
                        .replace(/"/g, '&quot;')
                        .replace(/'/g, '&#x27;')
                        .replace(/\//g, '&#x2F;')
                        .replace(/`/g, '&#96;')
                        .trim();
                    (sanitized as Record<string, unknown>)[key] = sanitizedValue;
                }
            }
        }

        return sanitized;
    }
}
