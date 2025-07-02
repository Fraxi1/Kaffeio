import { IsNumber, IsOptional, IsString } from 'class-validator';

export class UpdateMachineDto {
    @IsOptional()
    @IsString()
    name?: string;

    @IsOptional()
    @IsString()
    type?: string;

    @IsOptional()
    @IsNumber()
    facilityId?: number;

    @IsOptional()
    @IsString()
    status?: string;
}
