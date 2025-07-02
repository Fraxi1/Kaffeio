/* eslint-disable @typescript-eslint/no-unsafe-assignment */
/* eslint-disable @typescript-eslint/no-unsafe-call */
/* eslint-disable @typescript-eslint/no-unsafe-argument */
/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-return */
import { Injectable, HttpException, HttpStatus } from '@nestjs/common';
import axios from 'axios';

@Injectable()
export class AuthService {
  private sanitizeInput(input: string): string {
    // Basic sanitization to prevent injection attacks
    if (!input) return '';
    return input
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;')
      .trim();
  }

  async login(email: string, password: string) {
    try {
      // Sanitize inputs
      const sanitizedEmail = this.sanitizeInput(email);

      // We don't sanitize password as it might contain special characters
      // that are valid for passwords but would be removed by sanitization

      if (!sanitizedEmail || !password) {
        throw new HttpException('Email and password are required', HttpStatus.BAD_REQUEST);
      }

      // Get the backend URL from environment or use default
      const backendUrl = process.env.BACKEND_URL || 'http://localhost:3001/api';

      // Call the backend auth endpoint
      const response = await axios.post(`${backendUrl}/auth/login`, {
        email: sanitizedEmail,
        password: password
      });

      return response.data;
    } catch (error) {
      if (error.response) {
        // The request was made and the server responded with a status code
        // that falls out of the range of 2xx
        throw new HttpException(
          error.response.data.message || 'Authentication failed',
          error.response.status || HttpStatus.UNAUTHORIZED
        );
      } else if (error.request) {
        // The request was made but no response was received
        throw new HttpException(
          'No response from authentication server',
          HttpStatus.SERVICE_UNAVAILABLE
        );
      } else {
        // Something happened in setting up the request that triggered an Error
        throw new HttpException(
          error.message || 'Authentication error',
          HttpStatus.INTERNAL_SERVER_ERROR
        );
      }
    }
  }

  async getUsers() {
    try {
      // Get the backend URL from environment or use default
      const backendUrl = process.env.BACKEND_URL || 'http://localhost:3001/api';

      // Call the backend users endpoint
      const response = await axios.get(`${backendUrl}/users`);

      return response.data.data || [];
    } catch (error) {
      if (error.response) {
        throw new HttpException(
          error.response.data.message || 'Failed to fetch users',
          error.response.status || HttpStatus.INTERNAL_SERVER_ERROR
        );
      } else if (error.request) {
        throw new HttpException(
          'No response from server',
          HttpStatus.SERVICE_UNAVAILABLE
        );
      } else {
        throw new HttpException(
          error.message || 'Error fetching users',
          HttpStatus.INTERNAL_SERVER_ERROR
        );
      }
    }
  }

  async searchUsers(query: string) {
    try {
      // Sanitize input
      const sanitizedQuery = this.sanitizeInput(query);

      if (!sanitizedQuery) {
        return this.getUsers(); // Return all users if query is empty
      }

      // Get the backend URL from environment or use default
      const backendUrl = process.env.BACKEND_URL || 'http://localhost:3001/api';

      // Call the backend users endpoint and filter results
      const response = await axios.get(`${backendUrl}/users`);

      const users = response.data.data || [];

      // Filter users based on the query (case-insensitive)
      const lowerQuery = sanitizedQuery.toLowerCase();
      const filteredUsers = users.filter(user =>
        user.email.toLowerCase().includes(lowerQuery) ||
        (user.firstName && user.firstName.toLowerCase().includes(lowerQuery)) ||
        (user.lastName && user.lastName.toLowerCase().includes(lowerQuery))
      );

      return filteredUsers;
    } catch (error) {
      if (error.response) {
        throw new HttpException(
          error.response.data.message || 'Failed to search users',
          error.response.status || HttpStatus.INTERNAL_SERVER_ERROR
        );
      } else if (error.request) {
        throw new HttpException(
          'No response from server',
          HttpStatus.SERVICE_UNAVAILABLE
        );
      } else {
        throw new HttpException(
          error.message || 'Error searching users',
          HttpStatus.INTERNAL_SERVER_ERROR
        );
      }
    }
  }
}
