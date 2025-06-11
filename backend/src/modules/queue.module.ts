import { Module } from '@nestjs/common';
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
      },
      {
        name: 'operations-queue',
      },
      {
        name: 'fallback-queue',
      }
    ),
  ],
  providers: [
    DatabaseQueueService,
    {
      provide: OperationsQueueService,
      useClass: OperationsQueueService
    },
    {
      provide: FallbackQueueService,
      useClass: FallbackQueueService
    },
    {
      provide: 'OperationsQueueService',
      useExisting: OperationsQueueService
    },
    {
      provide: 'FallbackQueueService',
      useExisting: FallbackQueueService
    },
    QueueConnectionProvider,
    {
      provide: 'QUEUE_MODULE_INIT',
      useFactory: (ops: OperationsQueueService, fallback: FallbackQueueService) => {
        // Connect the services to resolve circular dependency
        ops.setFallbackQueueService(fallback);
        fallback.setOperationsQueueService(ops);
        return true;
      },
      inject: [OperationsQueueService, FallbackQueueService]
    }
  ],
  exports: [DatabaseQueueService, OperationsQueueService, FallbackQueueService],
})
export class QueueModule { }
