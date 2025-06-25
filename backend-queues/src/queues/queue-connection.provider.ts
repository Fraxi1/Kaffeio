import { Injectable, OnModuleInit } from '@nestjs/common';
import { OperationsQueueService } from './operations-queue.service';
import { FallbackQueueService } from './fallback-queue.service';

/**
 * Provider to connect queue services after initialization
 * This resolves circular dependency issues between queue services
 */
@Injectable()
export class QueueConnectionProvider implements OnModuleInit {
  constructor(
    private operationsQueueService: OperationsQueueService,
    private fallbackQueueService: FallbackQueueService,
  ) {}

  /**
   * Connect queue services after all modules are initialized
   */
  onModuleInit() {
    // Connect the operations queue to the fallback queue
    this.operationsQueueService.setFallbackQueueService(this.fallbackQueueService);
  }
}
