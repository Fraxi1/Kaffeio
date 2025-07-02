import { Entity, Column, PrimaryGeneratedColumn, ManyToOne, JoinColumn } from 'typeorm';
import { Machine } from './machine.entity';
import { Lot } from './lots.entity';
import { Facility } from './facility.entity';

@Entity()
export class ProductionSchedule {
  @PrimaryGeneratedColumn()
  id: number;

  @Column()
  lotId: number;

  @Column()
  machineId: number;

  @Column()
  facilityId: number;

  @Column({ type: 'timestamp' })
  scheduledStartTime: Date;

  @Column({ type: 'timestamp', nullable: true })
  scheduledEndTime: Date;

  @Column({ type: 'timestamp', nullable: true })
  actualStartTime: Date;

  @Column({ type: 'timestamp', nullable: true })
  actualEndTime: Date;

  @Column({ default: 'Scheduled' })
  status: string;

  @Column({ nullable: true })
  priority: number;

  @Column({ nullable: true })
  notes: string;

  @ManyToOne(() => Lot)
  @JoinColumn({ name: 'lotId' })
  lot: Lot;

  @ManyToOne(() => Machine)
  @JoinColumn({ name: 'machineId' })
  machine: Machine;

  @ManyToOne(() => Facility)
  @JoinColumn({ name: 'facilityId' })
  facility: Facility;
}
