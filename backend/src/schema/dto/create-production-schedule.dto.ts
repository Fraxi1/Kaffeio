import { IsNotEmpty, IsNumber, IsOptional, IsString, IsDateString } from 'class-validator';

export class CreateProductionScheduleDto {
  @IsNotEmpty()
  @IsNumber()
  lotId: number;

  @IsNotEmpty()
  @IsNumber()
  machineId: number;

  @IsNotEmpty()
  @IsNumber()
  facilityId: number;

  @IsNotEmpty()
  @IsDateString()
  scheduledStartTime: Date;

  @IsOptional()
  @IsDateString()
  scheduledEndTime?: Date;

  @IsOptional()
  @IsDateString()
  actualStartTime?: Date;

  @IsOptional()
  @IsDateString()
  actualEndTime?: Date;

  @IsOptional()
  @IsString()
  status?: string;

  @IsOptional()
  @IsNumber()
  priority?: number;

  @IsOptional()
  @IsString()
  notes?: string;
}
