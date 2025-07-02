/* eslint-disable @typescript-eslint/no-unsafe-member-access */
import { Controller, Get, Post, Body, Query } from '@nestjs/common';
import { SimulatorService } from '../service/simulator.service';
import { MachineData } from '../interfaces/machine-data.interface';

interface SendToBackendDto {
  backendUrl: string;
  batchSize?: number;
  intervalMs?: number;
  jwtToken?: string;
}

@Controller('simulator')
export class SimulatorController {
  constructor(private readonly simulatorService: SimulatorService) { }

  @Get('generate')
  generateData(@Query('count') count: number = 100): { message: string } {
    this.simulatorService.generateSimulatedData(count);
    return { message: `Generated ${count} simulated records for each dataset` };
  }

  @Get('data')
  getData(@Query('dataset') dataset?: string): Record<string, MachineData[]> | MachineData[] {
    return this.simulatorService.getSimulatedData(dataset);
  }

  @Post('send')
  async sendData(
    @Body('url') url: string,
    @Body('batchSize') batchSize: number = 10,
    @Body('intervalMs') intervalMs: number = 1000,
  ): Promise<{ message: string }> {
    if (!url) {
      throw new Error('URL is required');
    }

    await this.simulatorService.sendSimulatedData(url, batchSize, intervalMs);
    return { message: 'Data sent successfully' };
  }

  @Post('send-to-backend')
  async sendToBackend(@Body() body: SendToBackendDto) {
    const { backendUrl, batchSize = 10, intervalMs = 1000, jwtToken } = body;

    // Validate the backend URL
    if (!backendUrl) {
      return { success: false, message: 'Backend URL is required' };
    }

    // Ensure the URL points to the telemetry endpoint
    const telemetryUrl = backendUrl.endsWith('/telemetry')
      ? backendUrl
      : `${backendUrl}${backendUrl.endsWith('/') ? '' : '/'}telemetry`;

    // If JWT token is provided, set it in the simulator service
    if (jwtToken) {
      // In a real implementation, you would store this token securely
      console.log('JWT token provided for backend authentication');
    }

    try {
      await this.simulatorService.sendSimulatedData(telemetryUrl, batchSize, intervalMs);
      return {
        success: true,
        message: 'Data sending to backend telemetry API initiated',
        endpoint: telemetryUrl
      };
    } catch (error) {
      return {
        success: false,
        message: `Error sending data to backend: ${error.message}`,
        endpoint: telemetryUrl
      };
    }
  }
}
