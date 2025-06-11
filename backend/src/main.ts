import { NestFactory } from '@nestjs/core';
import { AppModule } from './modules/app.module';
import { ValidationPipe } from '@nestjs/common';
import { ResponseInterceptor } from './middleware/response.interceptor';
import { GlobalExceptionFilter } from './middleware/exception.filter';
import { JwtAuthGuard } from './guards/authentication.guards';
import { JwtService } from '@nestjs/jwt';
import { Reflector } from '@nestjs/core';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);

  app.enableCors();

  // Apply global pipes, interceptors and filters
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
  await app.listen(process.env.APP_PORT ?? 3000);
}
bootstrap();
