import { IsNumber, IsOptional, IsString } from 'class-validator';

export class UpdateTelemetryDto {
    @IsOptional()
    @IsNumber()
    machineId?: number;

    @IsOptional()
    @IsNumber()
    lotId?: number;

    @IsOptional()
    @IsString()
    dataType?: string;

    @IsOptional()
    @IsString()
    dataValue?: string;
}
