import {
  ExceptionFilter,
  Catch,
  ArgumentsHost,
  HttpException,
  HttpStatus,
} from '@nestjs/common';
import { Request, Response } from 'express';
import { ApiResponse } from './response.interface';

@Catch()
export class GlobalExceptionFilter implements ExceptionFilter {
  catch(exception: any, host: ArgumentsHost) {
    const ctx = host.switchToHttp();
    const response = ctx.getResponse<Response>();
    const request = ctx.getRequest<Request>();
    
    // Default to internal server error
    let status = HttpStatus.INTERNAL_SERVER_ERROR;
    let errorCode = 'INTERNAL_SERVER_ERROR';
    let message = 'An unexpected error occurred';
    let details: any = undefined;
    
    // Handle HttpExceptions
    if (exception instanceof HttpException) {
      status = exception.getStatus();
      const exceptionResponse = exception.getResponse();
      
      if (typeof exceptionResponse === 'object') {
        // If the exception already has our format, use it directly
        if ('success' in exceptionResponse && 'error' in exceptionResponse) {
          return response.status(status).json(exceptionResponse);
        }
        
        // Otherwise, extract information from the exception
        errorCode = `HTTP_${status}`;
        message = exceptionResponse['message'] || exception.message;
        
        // Sanitize details to remove sensitive information
        const { password, ...sanitizedDetails } = exceptionResponse as any;
        details = sanitizedDetails;
      } else {
        message = exceptionResponse as string;
      }
    }
    
    // Handle database errors
    if (exception.code) {
      if (exception.code === '23505') {
        status = HttpStatus.CONFLICT;
        errorCode = 'DUPLICATE_ENTRY';
        message = 'A record with this data already exists';
      } else if (exception.code === '22P02') {
        status = HttpStatus.BAD_REQUEST;
        errorCode = 'INVALID_INPUT';
        message = 'Invalid input data format';
      } else if (exception.code === '23503') {
        status = HttpStatus.BAD_REQUEST;
        errorCode = 'FOREIGN_KEY_VIOLATION';
        message = 'Referenced record does not exist';
      }
    }
    
    // Create standardized error response
    const errorResponse: ApiResponse<null> = {
      success: false,
      error: {
        code: errorCode,
        message,
        details: process.env.NODE_ENV === 'development' ? details : undefined,
      },
      timestamp: new Date().toISOString(),
      path: request.url,
    };
    
    response.status(status).json(errorResponse);
  }
}
