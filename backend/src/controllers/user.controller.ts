import { Controller, Body, Delete, Get, Param, Post, Put } from "@nestjs/common";
import { UserService } from "../services/user.service";
import { User } from "../schema/entities/user.entity";

@Controller('users')
export class UserController {
    constructor(private readonly userService: UserService) { }

    @Post()
    async createUser(@Body() user: User) {
        return this.userService.createUser(user);
    }

    @Get()
    async getAllUsers() {
        return this.userService.getAllUsers();
    }

    @Get(':id')
    async getUserById(@Param('id') id: string) {
        return this.userService.getUserById(id);
    }

    @Put(':id')
    async updateUser(@Param('id') id: string, @Body() user: User) {
        return this.userService.updateUser(id, user);
    }

    @Delete(':id')
    async deleteUser(@Param('id') id: string) {
        return this.userService.deleteUser(id);
    }
}