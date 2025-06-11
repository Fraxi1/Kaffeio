/* eslint-disable @typescript-eslint/no-unsafe-member-access */
import { Injectable, Logger } from '@nestjs/common';
import { promises as fs } from 'fs';
import { join } from 'path';
import { OperationsQueueService } from './operations-queue.service';

interface FallbackJob {
  id: string;
  name: string;
  data: any;
  timestamp: string;
  attempts: number;
}

@Injectable()
export class FallbackQueueService {
  private readonly logger = new Logger(FallbackQueueService.name);
  private readonly fallbackFilePath = join(process.cwd(), 'temp', 'fallback-queue.json');
  private isProcessing = false;
  private processingInterval: NodeJS.Timeout | null = null;

  constructor(private operationsQueueService: OperationsQueueService) {
    // Create temp directory if it doesn't exist
    void this.ensureTempDirectoryExists();
    
    // Start processing fallback jobs periodically
    this.startProcessing();
  }

  /**
   * Add a job to the fallback queue when the operations queue fails
   * @param name - Job name
   * @param data - Job data
   * @returns Job ID
   */
  async addFallbackJob<T>(name: string, data: T): Promise<string> {
    try {
      const jobId = `fallback-${Date.now()}-${Math.random().toString(36).substring(2, 9)}`;
      
      // Create a fallback job
      const job: FallbackJob = {
        id: jobId,
        name,
        data,
        timestamp: new Date().toISOString(),
        attempts: 0
      };

      // Read existing jobs
      const jobs = await this.readFallbackJobs();
      jobs.push(job);

      // Write updated jobs back to file
      await this.writeFallbackJobs(jobs);
      
      this.logger.log(`Added job ${name} to fallback queue with ID ${jobId}`);
      return jobId;
    } catch (error) {
      this.logger.error(`Failed to add job to fallback queue: ${error.message}`, error.stack);
      throw error;
    }
  }

  /**
   * Process fallback jobs and try to move them to the operations queue
   */
  async processFallbackJobs(): Promise<void> {
    if (this.isProcessing) {
      return;
    }

    this.isProcessing = true;
    try {
      const jobs = await this.readFallbackJobs();
      if (jobs.length === 0) {
        this.isProcessing = false;
        return;
      }

      this.logger.log(`Processing ${jobs.length} fallback jobs`);
      
      // Process jobs in order (oldest first)
      const remainingJobs: FallbackJob[] = [];
      
      for (const job of jobs) {
        try {
          // Try to add the job to the operations queue
          await this.operationsQueueService.addJob(job.name, job.data);
          this.logger.log(`Successfully moved job ${job.id} from fallback to operations queue`);
        } catch (err) {
          // If it fails, increment the attempt counter and keep in fallback
          this.logger.warn(`Failed to move job ${job.id} to operations queue: ${err.message}`);
          job.attempts += 1;
          
          if (job.attempts >= 5) {
            this.logger.error(`Job ${job.id} failed ${job.attempts} times, giving up`, {
              jobId: job.id,
              jobName: job.name,
              attempts: job.attempts
            });
          } else {
            this.logger.warn(`Failed to process fallback job ${job.id}, attempt ${job.attempts}`);
            remainingJobs.push(job);
          }
        }
      }

      // Write remaining jobs back to file
      await this.writeFallbackJobs(remainingJobs);
      
      this.logger.log(`Fallback processing complete. ${jobs.length - remainingJobs.length} jobs processed, ${remainingJobs.length} jobs remaining`);
    } catch (error) {
      this.logger.error(`Error processing fallback jobs: ${error.message}`, error.stack);
    } finally {
      this.isProcessing = false;
    }
  }

  /**
   * Start periodic processing of fallback jobs
   */
  startProcessing(): void {
    if (this.processingInterval) {
      clearInterval(this.processingInterval);
    }
    
    // Process fallback jobs every 30 seconds
    this.processingInterval = setInterval(() => {
      this.processFallbackJobs().catch(err => {
        this.logger.error(`Error in fallback job processing interval: ${err.message}`, err.stack);
      });
    }, 30000); // 30 seconds
    
    this.logger.log('Fallback queue processing started');
  }

  /**
   * Stop periodic processing of fallback jobs
   */
  stopProcessing(): void {
    if (this.processingInterval) {
      clearInterval(this.processingInterval);
      this.processingInterval = null;
      this.logger.log('Fallback queue processing stopped');
    }
  }

  /**
   * Read fallback jobs from the file
   */
  private async readFallbackJobs(): Promise<FallbackJob[]> {
    try {
      const fileExists = await this.fileExists(this.fallbackFilePath);
      if (!fileExists) {
        return [];
      }
      
      const data = await fs.readFile(this.fallbackFilePath, 'utf8');
      return JSON.parse(data) as FallbackJob[];
    } catch (error) {
      this.logger.error(`Failed to read fallback jobs: ${error.message}`, error.stack);
      return [];
    }
  }

  /**
   * Write fallback jobs to the file
   */
  private async writeFallbackJobs(jobs: FallbackJob[]): Promise<void> {
    try {
      await fs.writeFile(this.fallbackFilePath, JSON.stringify(jobs, null, 2), 'utf8');
    } catch (error) {
      this.logger.error(`Failed to write fallback jobs: ${error.message}`, error.stack);
      throw error;
    }
  }

  /**
   * Check if a file exists
   */
  private async fileExists(filePath: string): Promise<boolean> {
    try {
      await fs.access(filePath);
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Ensure the temp directory exists
   */
  private async ensureTempDirectoryExists(): Promise<void> {
    const tempDir = join(process.cwd(), 'temp');
    try {
      await fs.mkdir(tempDir, { recursive: true });
    } catch (error) {
      this.logger.error(`Failed to create temp directory: ${error.message}`, error.stack);
    }
  }
}
