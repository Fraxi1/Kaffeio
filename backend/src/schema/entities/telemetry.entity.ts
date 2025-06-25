import { Entity, PrimaryGeneratedColumn, Column, ManyToOne, JoinColumn, CreateDateColumn } from "typeorm";
import { Machine } from "./machine.entity";
import { Lot } from "./lots.entity";


@Entity("Telemetry")
export class Telemetry {
    @PrimaryGeneratedColumn()
    id: number;

    @ManyToOne(() => Machine)
    @JoinColumn({ name: "machineId" })
    machine: Machine;

    @Column()
    machineId: number;

    @ManyToOne(() => Lot, { nullable: true })
    @JoinColumn({ name: "lotId" })
    lot: Lot;

    @Column({ nullable: true })
    lotId: number;

    @CreateDateColumn()
    timestamp: Date;

    @Column({ length: 50 })
    dataType: string;

    @Column("text")
    dataValue: string;
}