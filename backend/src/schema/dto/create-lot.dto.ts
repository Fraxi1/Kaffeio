import { IsNotEmpty, IsOptional, IsString } from 'class-validator';

export class CreateLotDto {
    @IsNotEmpty()
    @IsString()
    Code: string;

    @IsOptional()
    @IsString()
    Description?: string;

    @IsOptional()
    @IsString()
    Status?: string;

    @IsOptional()
    CurrentMachineId?: number;
}
