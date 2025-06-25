import { NestFactory } from '@nestjs/core';
import { AppModule } from './modules/app.module';
import { ValidationPipe } from '@nestjs/common';
import { ResponseInterceptor } from './middleware/response.interceptor';
import { GlobalExceptionFilter } from './middleware/exception.filter';
import { JwtAuthGuard } from './guards/authentication.guards';
import { JwtService } from '@nestjs/jwt';
import { Reflector } from '@nestjs/core';

async function bootstrap() {
  try {
    console.log('Starting application bootstrap...');
    
    const app = await NestFactory.create(AppModule);
    console.log('NestFactory.create completed successfully');

    app.enableCors();
    console.log('CORS enabled');

  // Apply global pipes, interceptors and filters
  try {
    app.useGlobalPipes(new ValidationPipe({
      whitelist: true,
      transform: true,
      forbidNonWhitelisted: true,
    }));
    console.log('Global ValidationPipe applied');

    // Apply global JWT authentication guard
    const reflector = app.get(Reflector);
    console.log('Reflector retrieved');
    
    const jwtService = app.get(JwtService);
    console.log('JwtService retrieved');
    
    app.useGlobalGuards(new JwtAuthGuard(jwtService, reflector));
    console.log('Global JwtAuthGuard applied');

    app.useGlobalInterceptors(new ResponseInterceptor());
    console.log('Global ResponseInterceptor applied');
    
    app.useGlobalFilters(new GlobalExceptionFilter());
    console.log('Global GlobalExceptionFilter applied');

    app.setGlobalPrefix('api');
    console.log('Global API prefix set');
    
    const port = process.env.APP_PORT ?? 3000;
    await app.listen(port);
    console.log(`Application is running on: ${await app.getUrl()}`);
  } catch (error) {
    console.error('Error during application setup:', error);
  }
  } catch (error) {
    console.error('Fatal error during bootstrap:', error);
  }
}

bootstrap().catch(error => {
  console.error('Unhandled bootstrap error:', error);
});
