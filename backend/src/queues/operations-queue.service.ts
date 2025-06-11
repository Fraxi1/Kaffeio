/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */

import { Injectable, Logger, Inject, Optional } from '@nestjs/common';
import { InjectQueue } from '@nestjs/bullmq';
import { Queue, JobsOptions } from 'bullmq';
import { FallbackQueueService } from './fallback-queue.service';

@Injectable()
export class OperationsQueueService {
  private readonly logger = new Logger(OperationsQueueService.name);
  private fallbackQueueService: FallbackQueueService | null = null;
  
  constructor(
    @InjectQueue('operations-queue') private operationsQueue: Queue,
    @Optional() @Inject('FallbackQueueService') fallbackService?: FallbackQueueService
  ) {
    // Store fallback service if provided (will be injected after both services are created)
    if (fallbackService) {
      this.fallbackQueueService = fallbackService;
    }
  }

  /**
   * Add a general operation job to the queue
   * @param name - The name of the operation
   * @param data - The data for the operation
   * @param opts - Optional job options
   * @returns The job ID
   */
  async addJob<T>(name: string, data: T, opts?: JobsOptions): Promise<string> {
    try {
      const job = await this.operationsQueue.add(name, data, {
        removeOnComplete: true,
        removeOnFail: false,
        attempts: 2,
        backoff: {
          type: 'fixed',
          delay: 2000,
        },
        ...opts,
      });
      
      this.logger.debug(`Added job ${name} to operations queue with ID ${job.id}`);      
      return job.id?.toString() || '';
    } catch (error) {
      this.logger.error(`Failed to add job ${name} to operations queue: ${error.message}`);
      
      // If fallback service is available, use it
      if (this.fallbackQueueService) {
        this.logger.log(`Attempting to add job ${name} to fallback queue`);
        try {
          const fallbackJobId = await this.fallbackQueueService.addFallbackJob(name, data);
          this.logger.log(`Job ${name} added to fallback queue with ID ${fallbackJobId}`);
          return fallbackJobId;
        } catch (fallbackError) {
          this.logger.error(`Failed to add job to fallback queue: ${fallbackError.message}`);
          throw fallbackError;
        }
      } else {
        // No fallback available, rethrow the original error
        throw error;
      }
    }
  }

  /**
   * Get a job by ID
   * @param jobId - The job ID
   * @returns The job data or null if not found
   */
  async getJob<T = unknown>(jobId: string): Promise<T | null> {
    const job = await this.operationsQueue.getJob(jobId);
    return job ? (job.data as T) : null;
  }

  /**
   * Get the queue status
   * @returns Queue statistics
   */
  async getQueueStatus(): Promise<Record<string, number>> {
    try {
      return {
        waiting: await this.operationsQueue.getWaitingCount(),
        active: await this.operationsQueue.getActiveCount(),
        completed: await this.operationsQueue.getCompletedCount(),
        failed: await this.operationsQueue.getFailedCount(),
        delayed: await this.operationsQueue.getDelayedCount(),
      };
    } catch (error) {
      this.logger.error(`Failed to get queue status: ${error.message}`);
      return {
        waiting: 0,
        active: 0,
        completed: 0,
        failed: 0,
        delayed: 0,
        error: 1, // Indicate there was an error
      };
    }
  }
  
  /**
   * Set the fallback queue service
   * This is used to avoid circular dependency issues
   */
  setFallbackQueueService(fallbackService: FallbackQueueService): void {
    this.fallbackQueueService = fallbackService;
    this.logger.log('Fallback queue service registered with operations queue');
  }
}
