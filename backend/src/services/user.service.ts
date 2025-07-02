/* eslint-disable @typescript-eslint/no-unnecessary-type-assertion */
/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-call */
/* eslint-disable @typescript-eslint/no-unsafe-return */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import { Injectable, NotFoundException, ConflictException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { User, CreateUserDto, UpdateUserDto } from '../schema/entities/user.entity';

import * as bcrypt from 'bcrypt';

@Injectable()
export class UserService {
    constructor(
        @InjectRepository(User)
        private userRepository: Repository<User>,

    ) { }

    /**
     * Creates a new user
     * @param createUserDto - User creation data
     * @returns Created user object (without password)
     */
    async createUser(createUserDto: CreateUserDto): Promise<Partial<User>> {
        // Sanitize input to prevent injection attacks
        const sanitizedDto = this.sanitizeUserInput<CreateUserDto>(createUserDto);

        // Check if user with this email already exists
        const existingUser = await this.userRepository.findOne({
            where: { email: sanitizedDto.email }
        });

        if (existingUser) {
            throw new ConflictException(`User with email ${sanitizedDto.email} already exists`);
        }

        // Hash password
        const hashedPassword = await this.hashPassword(sanitizedDto.password);

        // Create new user with hashed password
        const newUser = this.userRepository.create({
            ...sanitizedDto,
            password: hashedPassword
        });

        // Save user to database
        const savedUser = await this.userRepository.save(newUser);

        // Queue functionality removed

        // Return user without password, excluding sensitive data
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const { password, ...userWithoutPassword } = savedUser;
        return userWithoutPassword;
    }

    async findByEmail(email: string): Promise<User | null> {
        return this.userRepository.findOne({ where: { email } });
    }

    /**
     * Get all users
     * @returns List of all users
     */
    async getAllUsers(): Promise<User[]> {
        // Queue functionality removed

        // Get users directly
        return this.userRepository.find({
            select: ['id', 'firstName', 'lastName', 'email'] // Exclude password
        });
    }

    /**
     * Get user by ID
     * @param id - User ID
     * @returns User data
     */
    async getUserById(id: string): Promise<User> {
        const userId = parseInt(id, 10);
        if (isNaN(userId)) {
            throw new BadRequestException('Invalid user ID');
        }

        // Queue functionality removed

        // Get user directly
        const user = await this.userRepository.findOne({
            where: { id: userId },
            select: ['id', 'firstName', 'lastName', 'email'] // Exclude password
        });

        if (!user) {
            throw new NotFoundException(`User with ID ${id} not found`);
        }

        return user;
    }

    /**
     * Updates an existing user
     * @param id - User ID
     * @param updateUserDto - User update data
     * @returns Updated user object (without password)
     */
    async updateUser(id: string, updateUserDto: UpdateUserDto): Promise<Partial<User>> {
        // Validate ID
        if (!id) {
            throw new BadRequestException('User ID is required');
        }

        // Convert string ID to number if needed
        const userId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof userId !== 'number' || isNaN(userId)) {
            throw new BadRequestException('Invalid user ID format');
        }

        // Sanitize input
        const sanitizedDto = this.sanitizeUserInput<UpdateUserDto>(updateUserDto);

        // Check if user exists
        const existingUser = await this.userRepository.findOne({ where: { id: userId } });
        if (!existingUser) {
            throw new NotFoundException(`User with ID ${id} not found`);
        }

        // If updating email, check if it's already in use by another user
        if (sanitizedDto.email) {
            const userWithEmail = await this.userRepository.findOne({
                where: { email: sanitizedDto.email }
            });

            if (userWithEmail && userWithEmail.id !== userId) {
                throw new ConflictException(`Email ${sanitizedDto.email} is already in use`);
            }
        }

        // If updating password, hash it
        if (sanitizedDto.password) {
            sanitizedDto.password = await this.hashPassword(sanitizedDto.password);
        }

        // Update user
        await this.userRepository.update(userId, sanitizedDto);

        // Return updated user
        const updatedUser = await this.userRepository.findOne({
            where: { id: userId },
            select: ['id', 'firstName', 'lastName', 'email'] // Exclude password
        });

        if (!updatedUser) {
            throw new NotFoundException(`User with ID ${id} not found after update`);
        }

        return updatedUser;
    }

    /**
     * Deletes a user
     * @param id - User ID
     * @returns Success message
     */
    async deleteUser(id: string): Promise<{ message: string }> {
        // Convert string ID to number if needed
        const userId = typeof id === 'string' ? parseInt(id, 10) : id;

        if (typeof userId !== 'number' || isNaN(userId)) {
            throw new BadRequestException('Invalid user ID format');
        }

        // Check if user exists
        const user = await this.userRepository.findOne({ where: { id: userId } });
        if (!user) {
            throw new NotFoundException(`User with ID ${id} not found`);
        }

        // Delete user
        await this.userRepository.delete(userId);

        // Queue functionality removed

        return { message: `User with ID ${id} has been deleted` };
    }

    /**
     * Hashes a password using bcrypt
     * @param password - Plain text password
     * @returns Hashed password
     */
    private async hashPassword(password: string): Promise<string> {
        try {
            const salt = await bcrypt.genSalt();
            return await bcrypt.hash(password, salt);
        } catch {
            // Error details intentionally not used
            throw new BadRequestException('Password hashing failed');
        }
    }

    /**
     * Sanitizes user input to prevent injection attacks
     * @param input - User input data
     * @returns Sanitized user input
     */
    private sanitizeUserInput<T>(input: T): T {
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