import { IsNumber, IsOptional, IsString, IsDateString } from 'class-validator';

export class UpdateProductionScheduleDto {
  @IsOptional()
  @IsNumber()
  lotId?: number;

  @IsOptional()
  @IsNumber()
  machineId?: number;

  @IsOptional()
  @IsNumber()
  facilityId?: number;

  @IsOptional()
  @IsDateString()
  scheduledStartTime?: Date;

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
