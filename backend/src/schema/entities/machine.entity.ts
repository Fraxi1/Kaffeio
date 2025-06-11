import { Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn } from "typeorm";
import { Facility } from "./facility.entity";

@Entity()
export class Machine {
    @PrimaryGeneratedColumn()
    id: number;

    @Column()
    name: string;

    @Column()
    type: string;

    @ManyToOne(() => Facility, (Facility) => Facility.name, {cascade:true})
    @JoinColumn({name:"id"})
    facilityId: number;

    @Column()
    status: boolean;
}