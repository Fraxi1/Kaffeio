import { Module } from '@nestjs/common';
import { FileLoader, SimulatorService, AuthService } from '../service';
import { SimulatorController } from '../controllers/simulator.controller';
import { AuthController } from '../controllers/auth.controller';

@Module({
  imports: [],
  controllers: [SimulatorController, AuthController],
  providers: [FileLoader, SimulatorService, AuthService],
})
export class AppModule { }
