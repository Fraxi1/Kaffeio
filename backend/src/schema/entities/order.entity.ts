/* eslint-disable @typescript-eslint/no-unsafe-return */
import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, JoinColumn, OneToMany } from 'typeorm';
import { Customer } from './customer.entity';
import { Lot } from './lots.entity';

@Entity()
export class Order {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  orderNumber: string;

  @Column()
  customerId: number;

  @Column()
  quantity: number;

  @Column({ type: 'timestamp', default: () => 'CURRENT_TIMESTAMP' })
  orderDate: Date;

  @Column({ type: 'timestamp', nullable: true })
  deliveryDate: Date;

  @Column({ default: 'Pending' })
  status: string; // Pending, InProgress, Completed

  @Column({ nullable: true })
  notes: string;

  @ManyToOne(() => Customer, customer => customer.orders)
  @JoinColumn({ name: 'customerId' })
  customer: Customer;

  @OneToMany(() => Lot, lot => lot.order, { cascade: true })
  lots: Lot[];
}
