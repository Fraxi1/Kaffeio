/* eslint-disable @typescript-eslint/prefer-promise-reject-errors */
/* eslint-disable @typescript-eslint/no-unsafe-return */
/* eslint-disable @typescript-eslint/no-unsafe-argument */
/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import { Injectable, OnModuleInit } from '@nestjs/common';
import * as fs from 'fs';
import * as path from 'path';
import * as csvParser from 'csv-parser';
import * as XLSX from 'xlsx';
import { MachineData } from '../interfaces/machine-data.interface';

@Injectable()
export class FileLoader implements OnModuleInit {
  private datasetPath = path.join(process.cwd(), 'dataset');
  private loadedData: Record<string, MachineData[]> = {};
  private isDataLoaded = false;

  async onModuleInit() {
    await this.loadAllDatasets();
    this.isDataLoaded = true;
    console.log('All datasets loaded successfully');
  }

  async loadAllDatasets(): Promise<void> {
    try {
      const files = fs.readdirSync(this.datasetPath);

      // Group files by their base name (without extension)
      const fileGroups = this.groupFilesByBaseName(files);

      // Load each group (prioritizing CSV files)
      for (const [baseName, fileNames] of Object.entries(fileGroups)) {
        // Prefer CSV files if available
        if (fileNames.some(file => file.endsWith('.csv'))) {
          const csvFile = fileNames.find(file => file.endsWith('.csv'))!;
          this.loadedData[baseName] = await this.loadCsvFile(path.join(this.datasetPath, csvFile));
        } else if (fileNames.some(file => file.endsWith('.json'))) {
          const jsonFile = fileNames.find(file => file.endsWith('.json'))!;
          this.loadedData[baseName] = this.loadJsonFile(path.join(this.datasetPath, jsonFile));
        } else if (fileNames.some(file => file.endsWith('.xlsx'))) {
          const xlsxFile = fileNames.find(file => file.endsWith('.xlsx'))!;
          this.loadedData[baseName] = await this.loadXlsxFile(path.join(this.datasetPath, xlsxFile));
        }
      }
    } catch (error) {
      console.error('Error loading datasets:', error);
      throw error;
    }
  }

  private groupFilesByBaseName(files: string[]): Record<string, string[]> {
    const groups: Record<string, string[]> = {};

    files.forEach(file => {
      const baseName = path.basename(file, path.extname(file));
      if (!groups[baseName]) {
        groups[baseName] = [];
      }
      groups[baseName].push(file);
    });

    return groups;
  }

  private loadCsvFile(filePath: string): Promise<MachineData[]> {
    return new Promise((resolve, reject) => {
      const results: MachineData[] = [];

      fs.createReadStream(filePath)
        .pipe(csvParser())
        .on('data', (data) => {
          // Convert string values to appropriate types
          const parsedData: MachineData = {
            macchina: data.macchina,
            esito_test: data.esito_test,
            pressione_caldaia: parseFloat(data.pressione_caldaia),
            temperatura_caldaia: parseFloat(data.temperatura_caldaia),
            consumo_energetico: parseFloat(data.consumo_energetico),
            luogo: data.luogo,
            timestamp_locale: data.timestamp_locale,
            timestamp_utc: data.timestamp_utc,
            blocco_macchina: data.blocco_macchina === 'True',
            motivo_blocco: data.motivo_blocco || null,
            ultima_manutenzione: data.ultima_manutenzione
          };

          results.push(parsedData);
        })
        .on('end', () => {
          resolve(results);
        })
        .on('error', (error) => {
          reject(error);
        });
    });
  }

  private loadJsonFile(filePath: string): MachineData[] {
    try {
      const fileContent = fs.readFileSync(filePath, 'utf8');
      const data = JSON.parse(fileContent);
      return Array.isArray(data) ? data : [];
    } catch (error) {
      console.error(`Error loading JSON file ${filePath}:`, error);
      return [];
    }
  }

  private loadXlsxFile(filePath: string): Promise<MachineData[]> {
    return new Promise((resolve, reject) => {
      try {
        const workbook = XLSX.readFile(filePath);
        const sheetName = workbook.SheetNames[0];
        const worksheet = workbook.Sheets[sheetName];
        const data = XLSX.utils.sheet_to_json(worksheet);

        // Convert data to MachineData format
        const results: MachineData[] = data.map((row: any) => ({
          macchina: row.macchina,
          esito_test: row.esito_test,
          pressione_caldaia: parseFloat(row.pressione_caldaia),
          temperatura_caldaia: parseFloat(row.temperatura_caldaia),
          consumo_energetico: parseFloat(row.consumo_energetico),
          luogo: row.luogo,
          timestamp_locale: row.timestamp_locale,
          timestamp_utc: row.timestamp_utc,
          blocco_macchina: row.blocco_macchina === 'True',
          motivo_blocco: row.motivo_blocco || null,
          ultima_manutenzione: row.ultima_manutenzione
        }));

        resolve(results);
      } catch (error) {
        console.error(`Error loading XLSX file ${filePath}:`, error);
        reject(error);
      }
    });
  }

  isLoaded(): boolean {
    return this.isDataLoaded;
  }

  getData(datasetName?: string): Record<string, MachineData[]> | MachineData[] {
    if (datasetName) {
      return this.loadedData[datasetName] || [];
    }
    return this.loadedData;
  }
}
