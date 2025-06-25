import { NestFactory } from '@nestjs/core';
import { AppModule } from './modules/app.module';
import { ValidationPipe } from '@nestjs/common';
import { ResponseInterceptor } from './middleware/response.interceptor';
import { GlobalExceptionFilter } from './middleware/exception.filter';
import { JwtAuthGuard } from './guards/authentication.guards';
import { JwtService } from '@nestjs/jwt';
import { Reflector } from '@nestjs/core';
import { Logger } from '@nestjs/common';

const logger = new Logger('Bootstrap');

async function bootstrap() {
  try {
    logger.log('Starting application bootstrap...');

    const app = await NestFactory.create(AppModule);

    app.enableCors();

    // Apply global pipes, interceptors and filters
    try {
      app.useGlobalPipes(new ValidationPipe({
        whitelist: true,
        transform: true,
        forbidNonWhitelisted: true,
      }));

      // Apply global JWT authentication guard
      const reflector = app.get(Reflector);

      const jwtService = app.get(JwtService);

      app.useGlobalGuards(new JwtAuthGuard(jwtService, reflector));

      app.useGlobalInterceptors(new ResponseInterceptor());

      app.useGlobalFilters(new GlobalExceptionFilter());

      app.setGlobalPrefix('api');

      const port = process.env.APP_PORT ?? 3000;
      await app.listen(port);
      logger.log(`Application is running on: ${await app.getUrl()}`);
    } catch (error) {
      logger.error('Error during application setup:', error);
    }
  } catch (error) {
    logger.error('Fatal error during bootstrap:', error);
  }
}

bootstrap().catch(error => {
  logger.error('Unhandled bootstrap error:', error);
});
