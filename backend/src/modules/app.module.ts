
import { Module } from '@nestjs/common';
import { ConfigModule } from '@nestjs/config';
import { TypeOrmModule } from '@nestjs/typeorm';

import { UserModule } from './user.module';
import { AuthModule } from './auth.module';
import { FacilityModule } from './facility.module';
import { LotModule } from './lot.module';
import { MachineModule } from './machine.module';
import { TelemetryModule } from './telemetry.module';
import { CustomerModule } from './customer.module';
import { OrderModule } from './order.module';
import { ProductionScheduleModule } from './production-schedule.module';

@Module({
  imports: [
    ConfigModule.forRoot({
      isGlobal: true,
    }),
    TypeOrmModule.forRoot({
      type: 'postgres',
      host: process.env.DB_HOST,
      port: Number(process.env.DB_PORT),
      username: process.env.DB_USER,
      password: process.env.DB_PASSWORD,
      database: process.env.DB_NAME,
      entities: [__dirname + '/../**/*.entity.{js,ts}'],
      autoLoadEntities: true,
      synchronize: true,
    }),


    UserModule,
    AuthModule,
    FacilityModule,
    LotModule,
    MachineModule,
    TelemetryModule,
    CustomerModule,
    OrderModule,
    ProductionScheduleModule
  ],
  controllers: [],
  providers: [],
})
export class AppModule { }
