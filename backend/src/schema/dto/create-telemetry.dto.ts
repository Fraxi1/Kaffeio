import { IsNotEmpty, IsNumber, IsOptional, IsString } from 'class-validator';

export class CreateTelemetryDto {
    @IsNotEmpty()
    @IsNumber()
    machineId: number;

    @IsOptional()
    @IsNumber()
    lotId?: number;

    @IsNotEmpty()
    @IsString()
    dataType: string;

    @IsNotEmpty()
    @IsString()
    dataValue: string;
}
