import { Entity, PrimaryGeneratedColumn, Column, CreateDateColumn, ManyToOne, JoinColumn } from 'typeorm';
import { Machine } from './machine.entity';
import { Order } from './order.entity';

@Entity('Lots')
export class Lot {
    @PrimaryGeneratedColumn()
    Id: number;

    @Column({ length: 50, unique: true })
    Code: string;

    @Column({ length: 200, nullable: true })
    Description: string;

    @CreateDateColumn()
    CreatedAt: Date;

    @Column({ length: 20, default: 'Created' })
    Status: string;

    @ManyToOne(() => Machine, { nullable: true })
    @JoinColumn({ name: 'CurrentMachineId' })
    CurrentMachine: Machine;

    @Column({ nullable: true })
    orderId: number;

    @ManyToOne(() => Order, order => order.lots, { nullable: true })
    @JoinColumn({ name: 'orderId' })
    order: Order;
}
