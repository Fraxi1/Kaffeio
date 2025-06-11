/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import { Injectable } from '@nestjs/common';
import { InjectQueue } from '@nestjs/bullmq';
import { Queue, JobsOptions } from 'bullmq';

@Injectable()
export class DatabaseQueueService {
  constructor(@InjectQueue('database-queue') private databaseQueue: Queue) { }

  /**
   * Add a database operation job to the queue
   * @param name - The name of the operation
   * @param data - The data for the operation
   * @param opts - Optional job options
   * @returns The job ID
   */
  async addJob<T>(name: string, data: T, opts?: JobsOptions): Promise<string> {
    const job = await this.databaseQueue.add(name, data, {
      removeOnComplete: true,
      removeOnFail: false,
      attempts: 3,
      backoff: {
        type: 'exponential',
        delay: 1000,
      },
      ...opts,
    });
    return job.id?.toString() || '';
  }

  /**
   * Get a job by ID
   * @param jobId - The job ID
   * @returns The job data or null if not found
   */
  async getJob<T = unknown>(jobId: string): Promise<T | null> {
    const job = await this.databaseQueue.getJob(jobId);
    return job ? (job.data as T) : null;
  }

  /**
   * Get the queue status
   * @returns Queue statistics
   */
  async getQueueStatus(): Promise<Record<string, number>> {
    return {
      waiting: await this.databaseQueue.getWaitingCount(),
      active: await this.databaseQueue.getActiveCount(),
      completed: await this.databaseQueue.getCompletedCount(),
      failed: await this.databaseQueue.getFailedCount(),
      delayed: await this.databaseQueue.getDelayedCount(),
    };
  }
}
