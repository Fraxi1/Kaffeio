import { Controller, Body, Delete, Get, Param, Post, Put, Query } from "@nestjs/common";
import { CustomerService } from "../services/customer.service";
import { CreateCustomerDto } from "../schema/dto/create-customer.dto";
import { UpdateCustomerDto } from "../schema/dto/update-customer.dto";

@Controller('customers')
export class CustomerController {
    constructor(private readonly customerService: CustomerService) { }

    @Post()
    async createCustomer(@Body() createCustomerDto: CreateCustomerDto) {
        return this.customerService.createCustomer(createCustomerDto);
    }

    @Get()
    async getAllCustomers() {
        return this.customerService.getAllCustomers();
    }

    @Get('email')
    async getCustomerByEmail(@Query('email') email: string) {
        return this.customerService.getCustomerByEmail(email);
    }

    @Get(':id')
    async getCustomerById(@Param('id') id: string) {
        return this.customerService.getCustomerById(id);
    }

    @Put(':id')
    async updateCustomer(@Param('id') id: string, @Body() updateCustomerDto: UpdateCustomerDto) {
        return this.customerService.updateCustomer(id, updateCustomerDto);
    }

    @Delete(':id')
    async deleteCustomer(@Param('id') id: string) {
        return this.customerService.deleteCustomer(id);
    }
}
