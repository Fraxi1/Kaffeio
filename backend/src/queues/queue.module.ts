import { Module } from '@nestjs/common';
import { BullModule } from '@nestjs/bullmq';
import { DatabaseQueueService } from './database-queue.service';
import { OperationsQueueService } from './operations-queue.service';
import { FallbackQueueService } from './fallback-queue.service';
import { QueueConnectionProvider } from './queue-connection.provider';


@Module({
  imports: [
    BullModule.registerQueue(
      {
        name: 'database-queue',
      },
      {
        name: 'operations-queue',
      },
    ),
  ],
  providers: [
    DatabaseQueueService, 
    OperationsQueueService, 
    FallbackQueueService,
    QueueConnectionProvider
  ],
  exports: [DatabaseQueueService, OperationsQueueService, FallbackQueueService],
})
export class QueueModule { }
