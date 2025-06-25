import { IsOptional, IsString } from 'class-validator';

export class UpdateFacilityDto {
    @IsOptional()
    @IsString()
    name?: string;

    @IsOptional()
    @IsString()
    location?: string;

    @IsOptional()
    @IsString()
    timeZone?: string;
}
