import { Entity, PrimaryGeneratedColumn, Column, ManyToOne, JoinColumn } from "typeorm";
import { Facility } from "./facility.entity";

@Entity('Machines')
export class Machine {
    @PrimaryGeneratedColumn()
    id: number;

    @Column({ length: 100, nullable: false })
    name: string;

    @Column({ length: 50, nullable: false })
    type: string;

    @Column()
    facilityId: number;

    @ManyToOne(() => Facility)
    @JoinColumn({ name: 'facilityId' })
    facility: Facility;

    @Column({ length: 20, default: 'Idle' })
    status: string;
}
