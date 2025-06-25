import { IsOptional, IsString } from 'class-validator';

export class UpdateLotDto {
    @IsOptional()
    @IsString()
    Code?: string;

    @IsOptional()
    @IsString()
    Description?: string;

    @IsOptional()
    @IsString()
    Status?: string;

    @IsOptional()
    CurrentMachineId?: number;
}
