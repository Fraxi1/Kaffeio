import { Column, Entity, PrimaryGeneratedColumn } from "typeorm";

@Entity()
export class Machine {
    @PrimaryGeneratedColumn()
    id: number;

    @Column()
    machineId: number;

    @Column()
    lotId: number;

    @Column()
    timestamp: Date;

    @Column()
    dataType: string;

    @Column()
    dataValue: string;
}