import { IsNumber, IsOptional, IsString, IsDateString } from 'class-validator';

export class UpdateOrderDto {
  @IsOptional()
  @IsString()
  orderNumber?: string;

  @IsOptional()
  @IsNumber()
  customerId?: number;

  @IsOptional()
  @IsNumber()
  quantity?: number;

  @IsOptional()
  @IsDateString()
  orderDate?: Date;

  @IsOptional()
  @IsDateString()
  deliveryDate?: Date;

  @IsOptional()
  @IsString()
  status?: string;

  @IsOptional()
  @IsString()
  notes?: string;
}
