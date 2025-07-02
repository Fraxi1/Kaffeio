/* eslint-disable @typescript-eslint/no-unsafe-member-access */
import { Injectable } from '@nestjs/common';
import { FileLoader } from './FileLoader';
import { MachineData } from '../interfaces/machine-data.interface';
import axios from 'axios';

// Interface for backend telemetry DTO
interface CreateTelemetryDto {
  machineId: number;
  lotId?: number;
  dataType: string;
  dataValue: string;
}

@Injectable()
export class SimulatorService {
  private simulatedData: Record<string, MachineData[]> = {};
  private readonly failureRate = 0.3; // 30% failure rate
  private readonly motivi_blocco = ['Guasto meccanico', 'Surriscaldamento', 'Errore software', 'Manutenzione programmata', 'Interruzione energia'];
  private readonly luoghi = ['Italia', 'Vietnam', 'Brasile'];

  constructor(private readonly fileLoader: FileLoader) { }

  /**
   * Generate simulated data based on the loaded datasets
   */
  generateSimulatedData(count: number = 100): void {
    if (!this.fileLoader.isLoaded()) {
      throw new Error('Dataset not loaded yet');
    }

    const loadedData = this.fileLoader.getData() as Record<string, MachineData[]>;

    // For each dataset, generate similar random data
    for (const [datasetName, dataArray] of Object.entries(loadedData)) {
      if (!dataArray || dataArray.length === 0) continue;

      this.simulatedData[datasetName] = [];

      // Calculate min/max ranges for numerical values
      const ranges = this.calculateRanges(dataArray);

      // Generate random data
      for (let i = 0; i < count; i++) {
        const randomData = this.generateRandomData(dataArray[0], ranges);
        this.simulatedData[datasetName].push(randomData);
      }
    }

    console.log(`Generated ${count} simulated records for each dataset`);
  }

  /**
   * Calculate min/max ranges for numerical properties in the dataset
   */
  private calculateRanges(data: MachineData[]): Record<string, { min: number; max: number }> {
    const ranges: Record<string, { min: number; max: number }> = {
      pressione_caldaia: { min: Infinity, max: -Infinity },
      temperatura_caldaia: { min: Infinity, max: -Infinity },
      consumo_energetico: { min: Infinity, max: -Infinity },
    };

    // Calculate min/max for each property
    for (const item of data) {
      for (const key of Object.keys(ranges)) {
        const value = item[key as keyof MachineData] as number;
        if (value < ranges[key].min) ranges[key].min = value;
        if (value > ranges[key].max) ranges[key].max = value;
      }
    }

    return ranges;
  }

  /**
   * Generate a random data point similar to the template but with random values
   */
  private generateRandomData(template: MachineData, ranges: Record<string, { min: number; max: number }>): MachineData {
    const isFailed = Math.random() < this.failureRate;
    const isBlocked = Math.random() < 0.1; // 10% chance of machine block

    const now = new Date();
    const utcNow = new Date(now.toISOString());
    const localNow = new Date(now.getTime() - (now.getTimezoneOffset() * 60000)).toISOString();

    return {
      macchina: template.macchina,
      esito_test: isFailed ? 'Fallito' : 'OK',
      pressione_caldaia: this.randomInRange(ranges.pressione_caldaia.min, ranges.pressione_caldaia.max),
      temperatura_caldaia: this.randomInRange(ranges.temperatura_caldaia.min, ranges.temperatura_caldaia.max),
      consumo_energetico: this.randomInRange(ranges.consumo_energetico.min, ranges.consumo_energetico.max),
      luogo: this.luoghi[Math.floor(Math.random() * this.luoghi.length)],
      timestamp_locale: localNow.substring(0, 19).replace('T', 'T'),
      timestamp_utc: utcNow.toISOString().substring(0, 19).replace('T', 'T'),
      blocco_macchina: isBlocked,
      motivo_blocco: isBlocked ? this.motivi_blocco[Math.floor(Math.random() * this.motivi_blocco.length)] : null,
      ultima_manutenzione: template.ultima_manutenzione
    };
  }

  /**
   * Generate a random number within a range with 2 decimal places
   */
  private randomInRange(min: number, max: number): number {
    return parseFloat((Math.random() * (max - min) + min).toFixed(2));
  }

  /**
   * Get the generated simulated data
   */
  getSimulatedData(datasetName?: string): Record<string, MachineData[]> | MachineData[] {
    if (datasetName) {
      return this.simulatedData[datasetName] || [];
    }
    return this.simulatedData;
  }

  /**
   * Prepare data for sending via axios
   */
  async sendSimulatedData(url: string, batchSize: number = 10, intervalMs: number = 1000): Promise<void> {
    // Check if the URL is the backend telemetry endpoint
    if (url.includes('/telemetry')) {
      await this.sendTelemetryToBackend(url, batchSize, intervalMs);
      return;
    }

    // Original implementation for other endpoints
    for (const [datasetName, dataArray] of Object.entries(this.simulatedData)) {
      console.log(`Preparing to send data for ${datasetName}`);

      // Send data in batches
      for (let i = 0; i < dataArray.length; i += batchSize) {
        const batch = dataArray.slice(i, i + batchSize);

        try {
          // Sanitize input data
          const sanitizedBatch = batch.map(item => this.sanitizeData(item));

          // Send data via axios
          await axios.post(url, {
            dataset: datasetName,
            data: sanitizedBatch,
            timestamp: new Date().toISOString()
          }, {
            headers: {
              'Content-Type': 'application/json'
            }
          });

          console.log(`Sent batch ${i / batchSize + 1} for ${datasetName}`);

          // Wait for the specified interval before sending the next batch
          if (i + batchSize < dataArray.length) {
            await new Promise(resolve => setTimeout(resolve, intervalMs));
          }
        } catch (error) {
          console.error(`Error sending batch for ${datasetName}:`, error);
        }
      }
    }
  }

  /**
   * Sanitize data to prevent injection attacks
   */
  private sanitizeData(data: MachineData): MachineData {
    // Create a copy of the data
    const sanitized = { ...data };

    // Sanitize string fields
    sanitized.macchina = this.sanitizeString(data.macchina);
    sanitized.esito_test = this.sanitizeString(data.esito_test);
    sanitized.luogo = this.sanitizeString(data.luogo);
    sanitized.timestamp_locale = this.sanitizeString(data.timestamp_locale);
    sanitized.timestamp_utc = this.sanitizeString(data.timestamp_utc);

    if (data.motivo_blocco) {
      sanitized.motivo_blocco = this.sanitizeString(data.motivo_blocco);
    }

    sanitized.ultima_manutenzione = this.sanitizeString(data.ultima_manutenzione);

    return sanitized;
  }

  /**
   * Sanitize a string to prevent injection attacks
   */
  private sanitizeString(str: string): string {
    if (!str) return str;
    return str
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;')
      .replace(/`/g, '&#96;')
      .replace(/\$/g, '&#36;');
  }

  /**
   * Send telemetry data to the backend API
   * Format data according to CreateTelemetryDto structure
   */
  async sendTelemetryToBackend(url: string, batchSize: number = 10, intervalMs: number = 1000): Promise<void> {
    console.log('Sending telemetry data to backend API');

    let totalSent = 0;

    for (const [datasetName, dataArray] of Object.entries(this.simulatedData)) {
      console.log(`Preparing to send telemetry for ${datasetName}`);

      // Send data in batches
      for (let i = 0; i < dataArray.length; i += batchSize) {
        const batch = dataArray.slice(i, i + batchSize);

        try {
          // Convert each machine data item to telemetry DTOs
          for (const item of batch) {
            const sanitizedItem = this.sanitizeData(item);

            // Create telemetry DTOs for each data point
            const telemetryItems: CreateTelemetryDto[] = [
              {
                machineId: parseInt(sanitizedItem.macchina.replace(/\D/g, '')) || 1, // Extract number from machine name or default to 1
                dataType: 'pressione_caldaia',
                dataValue: sanitizedItem.pressione_caldaia.toString()
              },
              {
                machineId: parseInt(sanitizedItem.macchina.replace(/\D/g, '')) || 1,
                dataType: 'temperatura_caldaia',
                dataValue: sanitizedItem.temperatura_caldaia.toString()
              },
              {
                machineId: parseInt(sanitizedItem.macchina.replace(/\D/g, '')) || 1,
                dataType: 'consumo_energetico',
                dataValue: sanitizedItem.consumo_energetico.toString()
              },
              {
                machineId: parseInt(sanitizedItem.macchina.replace(/\D/g, '')) || 1,
                dataType: 'esito_test',
                dataValue: sanitizedItem.esito_test
              },
              {
                machineId: parseInt(sanitizedItem.macchina.replace(/\D/g, '')) || 1,
                dataType: 'blocco_macchina',
                dataValue: sanitizedItem.blocco_macchina ? 'true' : 'false'
              }
            ];

            // If machine is blocked, add the reason
            if (sanitizedItem.blocco_macchina && sanitizedItem.motivo_blocco) {
              telemetryItems.push({
                machineId: parseInt(sanitizedItem.macchina.replace(/\D/g, '')) || 1,
                dataType: 'motivo_blocco',
                dataValue: sanitizedItem.motivo_blocco
              });
            }

            // Send each telemetry item individually to match backend DTO format
            for (const telemetryItem of telemetryItems) {
              await axios.post(url, telemetryItem, {
                headers: {
                  'Content-Type': 'application/json',
                  'Authorization': 'Bearer YOUR_JWT_TOKEN_HERE' // Replace with actual JWT token if needed
                }
              });

              totalSent++;
            }
          }

          console.log(`Sent telemetry batch ${i / batchSize + 1} for ${datasetName}`);

          // Wait for the specified interval before sending the next batch
          if (i + batchSize < dataArray.length) {
            await new Promise(resolve => setTimeout(resolve, intervalMs));
          }
        } catch (error) {
          console.error(`Error sending telemetry batch for ${datasetName}:`, error.message);
          if (error.response) {
            console.error('Response data:', error.response.data);
            console.error('Response status:', error.response.status);
          }
        }
      }
    }

    console.log(`Total telemetry data points sent: ${totalSent}`);
  }
}
