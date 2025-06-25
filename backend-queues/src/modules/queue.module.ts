/* eslint-disable @typescript-eslint/no-unsafe-member-access */
import { Module, Logger } from '@nestjs/common';
import { BullModule } from '@nestjs/bullmq';
import { DatabaseQueueService } from '../queues/database-queue.service';
import { OperationsQueueService } from '../queues/operations-queue.service';
import { FallbackQueueService } from '../queues/fallback-queue.service';
import { QueueConnectionProvider } from '../queues/queue-connection.provider';


@Module({
  imports: [
    BullModule.registerQueue(
      {
        name: 'database-queue',
        // Add connection options for debugging
        connection: {
          enableReadyCheck: false,
          maxRetriesPerRequest: 3,
          retryStrategy: (times) => {
            console.log(`[Redis] Retry attempt ${times} for database-queue`);
            return Math.min(times * 100, 3000);
          }
        }
      },
      {
        name: 'operations-queue',
        // Add connection options for debugging
        connection: {
          enableReadyCheck: false,
          maxRetriesPerRequest: 3,
          retryStrategy: (times) => {
            console.log(`[Redis] Retry attempt ${times} for operations-queue`);
            return Math.min(times * 100, 3000);
          }
        }
      },
      {
        name: 'fallback-queue',
        // Add connection options for debugging
        connection: {
          enableReadyCheck: false,
          maxRetriesPerRequest: 3,
          retryStrategy: (times) => {
            console.log(`[Redis] Retry attempt ${times} for fallback-queue`);
            return Math.min(times * 100, 3000);
          }
        }
      }
    ),
  ],
  
  providers: [
    // Logger provider
    {
      provide: Logger,
      useValue: new Logger('QueueModule')
    },
    
    // Queue services
    DatabaseQueueService,
    OperationsQueueService,
    FallbackQueueService,
    
    // String token providers for dependency injection
    {
      provide: 'OperationsQueueService',
      useExisting: OperationsQueueService
    },
    {
      provide: 'FallbackQueueService',
      useExisting: FallbackQueueService
    },
    
    // Queue connection provider
    QueueConnectionProvider,
    
    // Module initialization
    {
      provide: 'QUEUE_MODULE_INIT',
      useFactory: (ops: OperationsQueueService, fallback: FallbackQueueService, logger: Logger) => {
        logger.log('Starting QUEUE_MODULE_INIT');
        try {
          // Connect the services to resolve circular dependency
          logger.log('Setting FallbackQueueService in OperationsQueueService');
          ops.setFallbackQueueService(fallback);
          
          logger.log('Setting OperationsQueueService in FallbackQueueService');
          fallback.setOperationsQueueService(ops);
          
          logger.log('QUEUE_MODULE_INIT completed successfully');
          return true;
        } catch (error: any) {
          logger.error(`Error in QUEUE_MODULE_INIT: ${error.message}`, error.stack);
          throw error;
        }
      },
      inject: [OperationsQueueService, FallbackQueueService, Logger]
    }
  ],
  exports: [DatabaseQueueService, OperationsQueueService, FallbackQueueService],
})
export class QueueModule { }
