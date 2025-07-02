/* eslint-disable @typescript-eslint/no-unsafe-assignment */
/* eslint-disable @typescript-eslint/no-unsafe-member-access */
import { Controller, Post, Get, Body, HttpException, HttpStatus } from '@nestjs/common';
import { AuthService } from '../service';

@Controller('auth')
export class AuthController {
  constructor(private readonly authService: AuthService) { }

  @Post('login')
  async login(@Body() loginDto: { email: string; password: string }) {
    try {
      const result = await this.authService.login(loginDto.email, loginDto.password);
      return {
        success: true,
        data: result,
        timestamp: new Date().toISOString(),
        path: '/auth/login'
      };
    } catch (error) {
      throw new HttpException({
        success: false,
        message: error.message || 'Authentication failed',
        timestamp: new Date().toISOString(),
        path: '/auth/login'
      }, HttpStatus.UNAUTHORIZED);
    }
  }

  @Get('users')
  async getUsers() {
    try {
      const users = await this.authService.getUsers();
      return {
        success: true,
        data: users,
        timestamp: new Date().toISOString(),
        path: '/auth/users'
      };
    } catch (error) {
      throw new HttpException({
        success: false,
        message: error.message || 'Failed to fetch users',
        timestamp: new Date().toISOString(),
        path: '/auth/users'
      }, HttpStatus.INTERNAL_SERVER_ERROR);
    }
  }

  @Post('search')
  async searchUsers(@Body() searchDto: { query: string }) {
    try {
      const users = await this.authService.searchUsers(searchDto.query);
      return {
        success: true,
        data: users,
        timestamp: new Date().toISOString(),
        path: '/auth/search'
      };
    } catch (error) {
      throw new HttpException({
        success: false,
        message: error.message || 'Failed to search users',
        timestamp: new Date().toISOString(),
        path: '/auth/search'
      }, HttpStatus.INTERNAL_SERVER_ERROR);
    }
  }
}
