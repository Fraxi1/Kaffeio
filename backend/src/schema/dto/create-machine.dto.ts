import { IsNotEmpty, IsNumber, IsOptional, IsString } from 'class-validator';

export class CreateMachineDto {
    @IsNotEmpty()
    @IsString()
    name: string;

    @IsNotEmpty()
    @IsString()
    type: string;

    @IsNotEmpty()
    @IsNumber()
    facilityId: number;

    @IsOptional()
    @IsString()
    status?: string;
}
