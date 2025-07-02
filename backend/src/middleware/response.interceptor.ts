/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-unsafe-return */
/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */
/* eslint-disable @typescript-eslint/no-unsafe-argument */
import {
  Injectable,
  NestInterceptor,
  ExecutionContext,
  CallHandler,
  HttpException,
  HttpStatus,
} from '@nestjs/common';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiResponse } from './response.interface';
import { Request } from 'express';

@Injectable()
export class ResponseInterceptor<T> implements NestInterceptor<T, ApiResponse<T>> {
  intercept(context: ExecutionContext, next: CallHandler): Observable<ApiResponse<T>> {
    const ctx = context.switchToHttp();
    const request = ctx.getRequest<Request>();
    const path = request.url;

    return next.handle().pipe(
      map((data) => this.formatSuccessResponse(data, path)),
      catchError((error) => {
        return throwError(() => this.formatErrorResponse(error, path));
      }),
    );
  }

  private formatSuccessResponse(data: T, path: string): ApiResponse<T> {
    return {
      success: true,
      data,
      timestamp: new Date().toISOString(),
      path,
    };
  }

  private formatErrorResponse(error: any, path: string): HttpException {
    const errorResponse: ApiResponse<null> = {
      success: false,
      error: {
        code: this.getErrorCode(error),
        message: this.getErrorMessage(error),
        details: this.getErrorDetails(error),
      },
      timestamp: new Date().toISOString(),
      path,
    };

    const status = this.getHttpStatus(error);
    return new HttpException(errorResponse, status);
  }

  private getErrorCode(error: any): string {
    if (error instanceof HttpException) {
      return `HTTP_${error.getStatus()}`;
    }

    // Handle database-specific errors
    if (error.code) {
      if (error.code === '23505') return 'DUPLICATE_ENTRY';
      if (error.code === '22P02') return 'INVALID_INPUT';
      if (error.code === '23503') return 'FOREIGN_KEY_VIOLATION';
    }

    return 'INTERNAL_SERVER_ERROR';
  }

  private getErrorMessage(error: any): string {
    if (error instanceof HttpException) {
      const response = error.getResponse();
      if (typeof response === 'object' && 'message' in response) {
        return Array.isArray(response.message)
          ? response.message[0]
          : response.message as string;
      }
      return error.message;
    }

    // Sanitize error messages to prevent leaking sensitive information
    if (error.code === '23505') return 'A record with this data already exists';
    if (error.code === '22P02') return 'Invalid input data format';
    if (error.code === '23503') return 'Referenced record does not exist';

    return 'An unexpected error occurred';
  }

  private getErrorDetails(error: any): any {
    if (error instanceof HttpException) {
      const response = error.getResponse();
      if (typeof response === 'object') {
        // Remove sensitive information
        const { password, ...sanitizedResponse } = response as any;
        return sanitizedResponse;
      }
    }

    // For security, don't return raw error details in production
    if (process.env.NODE_ENV === 'development') {
      // Sanitize error details
      const { password, stack, ...sanitizedError } = error;
      return sanitizedError;
    }

    return undefined;
  }

  private getHttpStatus(error: any): HttpStatus {
    if (error instanceof HttpException) {
      return error.getStatus();
    }

    // Map database errors to appropriate HTTP status codes
    if (error.code === '23505') return HttpStatus.CONFLICT;
    if (error.code === '22P02') return HttpStatus.BAD_REQUEST;
    if (error.code === '23503') return HttpStatus.BAD_REQUEST;

    return HttpStatus.INTERNAL_SERVER_ERROR;
  }
}
