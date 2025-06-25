/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable no-useless-catch */
/* eslint-disable @typescript-eslint/no-unsafe-argument */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */
/* eslint-disable @typescript-eslint/no-unsafe-member-access */
import { Body, Controller, HttpCode, Post, UseGuards } from '@nestjs/common';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { Public } from '../decorators/public.decorator';
import { ConflictException, BadRequestException } from '@nestjs/common';
import * as bcrypt from 'bcrypt';
import { CreateUserDto } from '../schema/dto/create-user.dto';
import { LoginDto } from '../schema/dto/login.dto';

@Controller('auth')
export class AuthController {
    constructor(
        private readonly authService: AuthService,
        private readonly userService: UserService
    ) { }

    @Public()
    @Post('login')
    @HttpCode(200)
    async login(@Body() loginDto: LoginDto) {
        try {
            // Validate user credentials
            const user = await this.authService.validateUser(
                loginDto.email,
                loginDto.password
            );

            // Generate JWT token
            return this.authService.login(user);
        } catch (error) {
            throw error;
        }
    }

    @Public()
    @Post('register')
    async register(@Body() createUserDto: CreateUserDto) {
        try {
            // Check if user with email already exists
            const existingUser = await this.userService.findByEmail(createUserDto.email);
            if (existingUser) {
                throw new ConflictException('Email already in use');
            }

            // Hash the password
            const salt = await bcrypt.genSalt();
            const hashedPassword = await bcrypt.hash(createUserDto.password, salt);

            // Create new user with hashed password
            const newUser = await this.userService.createUser({
                ...createUserDto,
                password: hashedPassword
            });

            // Return user without password
            const { password, ...result } = newUser;
            return result;
        } catch (error) {
            if (error instanceof ConflictException) {
                throw error;
            }
            throw new BadRequestException('Failed to register user');
        }
    }
}