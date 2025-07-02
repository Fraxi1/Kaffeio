import { Column, Entity, PrimaryGeneratedColumn } from "typeorm";

@Entity()
export class Facility {
    @PrimaryGeneratedColumn()
    id: number;

    @Column()
    name: string;

    @Column()
    location: string;

    @Column()
    timeZone: string;
}