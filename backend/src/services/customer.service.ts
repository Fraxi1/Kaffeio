/* eslint-disable @typescript-eslint/no-unnecessary-type-assertion */
import { Injectable, NotFoundException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Customer } from '../schema/entities/customer.entity';
import { CreateCustomerDto } from '../schema/dto/create-customer.dto';
import { UpdateCustomerDto } from '../schema/dto/update-customer.dto';

@Injectable()
export class CustomerService {
    constructor(
        @InjectRepository(Customer)
        private customerRepository: Repository<Customer>,
    ) { }

    /**
     * Creates a new customer
     * @param createCustomerDto - Customer creation data
     * @returns Created customer object
     */
    async createCustomer(createCustomerDto: CreateCustomerDto): Promise<Customer> {
        // Sanitize input to prevent injection attacks
        const sanitizedDto = this.sanitizeInput<CreateCustomerDto>(createCustomerDto);

        // Check if customer with the same email already exists
        const existingCustomer = await this.customerRepository.findOne({
            where: { email: sanitizedDto.email }
        });

        if (existingCustomer) {
            throw new BadRequestException(`Customer with email ${sanitizedDto.email} already exists`);
        }

        // Create new customer
        const newCustomer = this.customerRepository.create(sanitizedDto);

        // Save customer to database
        return this.customerRepository.save(newCustomer);
    }

    /**
     * Get all customers
     * @returns List of customers
     */
    async getAllCustomers(): Promise<Customer[]> {
        return this.customerRepository.find({
            relations: ['orders']
        });
    }

    /**
     * Get customer by ID
     * @param id - Customer ID
     * @returns Customer data
     */
    async getCustomerById(id: string): Promise<Customer> {
        const customerId = parseInt(id, 10);
        if (isNaN(customerId)) {
            throw new BadRequestException('Invalid customer ID');
        }

        const customer = await this.customerRepository.findOne({
            where: { id: customerId },
            relations: ['orders']
        });

        if (!customer) {
            throw new NotFoundException(`Customer with ID ${id} not found`);
        }

        return customer;
    }

    /**
     * Get customer by email
     * @param email - Customer email
     * @returns Customer data
     */
    async getCustomerByEmail(email: string): Promise<Customer> {
        const customer = await this.customerRepository.findOne({
            where: { email },
            relations: ['orders']
        });

        if (!customer) {
            throw new NotFoundException(`Customer with email ${email} not found`);
        }

        return customer;
    }

    /**
     * Updates an existing customer
     * @param id - Customer ID
     * @param updateCustomerDto - Customer update data
     * @returns Updated customer object
     */
    async updateCustomer(id: string, updateCustomerDto: UpdateCustomerDto): Promise<Customer> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('Customer ID is required');
        }

        // Convert string ID to number if needed
        const customerId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof customerId !== 'number' || isNaN(customerId)) {
            throw new BadRequestException('Invalid customer ID format');
        }

        // Sanitize input
        const sanitizedDto = this.sanitizeInput<UpdateCustomerDto>(updateCustomerDto);

        // Check if customer exists
        const existingCustomer = await this.customerRepository.findOne({ where: { id: customerId } });
        if (!existingCustomer) {
            throw new NotFoundException(`Customer with ID ${id} not found`);
        }

        // If updating email, check if it's already in use by another customer
        if (sanitizedDto.email && sanitizedDto.email !== existingCustomer.email) {
            const customerWithEmail = await this.customerRepository.findOne({
                where: { email: sanitizedDto.email }
            });

            if (customerWithEmail && customerWithEmail.id !== customerId) {
                throw new BadRequestException(`Email ${sanitizedDto.email} is already in use by another customer`);
            }
        }

        // Update customer
        await this.customerRepository.update(customerId, sanitizedDto);

        // Return updated customer
        const updatedCustomer = await this.customerRepository.findOne({
            where: { id: customerId },
            relations: ['orders']
        });

        if (!updatedCustomer) {
            throw new NotFoundException(`Customer with ID ${id} not found after update`);
        }

        return updatedCustomer;
    }

    /**
     * Deletes a customer
     * @param id - Customer ID
     * @returns Success message
     */
    async deleteCustomer(id: string): Promise<{ message: string }> {
        // Convert string ID to number if needed
        const customerId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof customerId !== 'number' || isNaN(customerId)) {
            throw new BadRequestException('Invalid customer ID format');
        }

        // Check if customer exists
        const customer = await this.customerRepository.findOne({
            where: { id: customerId },
            relations: ['orders']
        });

        if (!customer) {
            throw new NotFoundException(`Customer with ID ${id} not found`);
        }

        // Check if customer has orders
        if (customer.orders && customer.orders.length > 0) {
            throw new BadRequestException(`Cannot delete customer with ID ${id} because they have associated orders`);
        }

        // Delete customer
        await this.customerRepository.delete(customerId);

        return { message: `Customer with ID ${id} has been deleted` };
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
