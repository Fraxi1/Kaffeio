# Simulatore di Macchine - Kaffeio

## Panoramica

Il simulatore di macchine è un componente fondamentale del sistema Kaffeio che simula il comportamento delle macchine reali presenti nelle strutture produttive di CoffeeMek S.p.A. Questo componente è essenziale per lo sviluppo, il testing e le dimostrazioni del sistema senza la necessità di connessione a macchine fisiche.

## Funzionalità Principali

- Simulazione di diversi tipi di macchine (Fresa CNC, Tornio automatico, Linea assemblaggio, Linea di test)
- Generazione di dati di telemetria realistici basati su modelli statistici
- Simulazione di diversi scenari di produzione e anomalie
- Gestione dei fusi orari per simulare le strutture in diverse parti del mondo
- Invio dei dati al backend tramite API REST

## Architettura

Il simulatore è sviluppato come un'applicazione NestJS indipendente che può essere eseguita separatamente dal backend principale. È progettato per essere scalabile e configurabile, permettendo di simulare un numero variabile di macchine e strutture.

### Componenti del Simulatore

1. **Generatori di Dati**: Moduli responsabili della generazione di dati realistici per ogni tipo di macchina
2. **Scheduler**: Gestisce la frequenza di invio dei dati e simula il passaggio del tempo
3. **Gestione dei Fusi Orari**: Assicura che i timestamp generati siano corretti per ogni struttura
4. **API Client**: Gestisce la comunicazione con il backend API
5. **Interfaccia Web**: Fornisce un'interfaccia per configurare e monitorare il simulatore

## Tipi di Macchine Simulate

### 1. Fresa CNC

La fresa CNC simula una macchina utilizzata per la lavorazione di componenti metallici.

**Dati generati:**
- Tempo ciclo (secondi)
- Profondità di taglio (mm)
- Vibrazione (mm/s)
- Allarmi utensile (boolean/string)

**Esempio di dati:**
```json
{
  "machineId": 1,
  "lotId": 1,
  "dataType": "cnc_data",
  "dataValue": {
    "cycle_time": 45.2,
    "cutting_depth": 2.5,
    "vibration": 0.42,
    "tool_alarm": false
  },
  "timestamp": "2025-06-25T12:30:00.000Z",
  "timeZone": "Europe/Rome"
}
```

### 2. Tornio Automatico

Il tornio automatico simula una macchina utilizzata per la lavorazione di componenti cilindrici.

**Dati generati:**
- Stato macchina (string)
- Velocità rotazione (RPM)
- Temperatura mandrino (°C)
- Numero pezzi completati (integer)

**Esempio di dati:**
```json
{
  "machineId": 2,
  "lotId": 1,
  "dataType": "lathe_data",
  "dataValue": {
    "machine_status": "running",
    "rotation_speed": 1200,
    "spindle_temperature": 65.8,
    "completed_pieces": 42
  },
  "timestamp": "2025-06-25T12:35:00.000Z",
  "timeZone": "Europe/Rome"
}
```

### 3. Linea Assemblaggio

La linea di assemblaggio simula il processo di assemblaggio delle macchine da caffè.

**Dati generati:**
- Tempo medio per stazione (secondi)
- Numero operatori (integer)
- Anomalie rilevate (string)

**Esempio di dati:**
```json
{
  "machineId": 3,
  "lotId": 1,
  "dataType": "assembly_data",
  "dataValue": {
    "station_time": 120.5,
    "operators": 3,
    "anomalies": "None"
  },
  "timestamp": "2025-06-25T14:10:00.000Z",
  "timeZone": "Europe/Rome"
}
```

### 4. Linea di Test

La linea di test simula il processo di controllo qualità delle macchine assemblate.

**Dati generati:**
- Esiti test funzionali (pass/fail)
- Pressione caldaia (bar)
- Temperatura caldaia (°C)
- Consumo energetico (kWh)

**Esempio di dati:**
```json
{
  "machineId": 4,
  "lotId": 1,
  "dataType": "test_data",
  "dataValue": {
    "test_result": "pass",
    "pressure": 9.2,
    "temperature": 92.5,
    "energy_consumption": 1.2
  },
  "timestamp": "2025-06-25T15:45:00.000Z",
  "timeZone": "Europe/Rome"
}
```

## Configurazione

Il simulatore può essere configurato tramite file di configurazione o variabili d'ambiente.

### File di Configurazione

```json
{
  "api": {
    "baseUrl": "http://localhost:3000/api",
    "authToken": "your-auth-token"
  },
  "simulation": {
    "intervalMs": 5000,
    "facilities": [
      {
        "id": 1,
        "name": "Stabilimento Italia",
        "timeZone": "Europe/Rome",
        "machines": [
          {
            "id": 1,
            "name": "Fresa CNC 001",
            "type": "Fresa CNC"
          },
          {
            "id": 2,
            "name": "Tornio 002",
            "type": "Tornio automatico"
          }
        ]
      },
      {
        "id": 2,
        "name": "Stabilimento Brasile",
        "timeZone": "America/Sao_Paulo",
        "machines": [
          {
            "id": 3,
            "name": "Linea Assemblaggio 001",
            "type": "Linea assemblaggio"
          }
        ]
      }
    ],
    "lots": [
      {
        "id": 1,
        "code": "LOT-001",
        "description": "Macchine da caffè modello Pro X1"
      }
    ]
  }
}
```

### Variabili d'Ambiente

```
SIMULATOR_API_BASE_URL=http://localhost:3000/api
SIMULATOR_API_AUTH_TOKEN=your-auth-token
SIMULATOR_INTERVAL_MS=5000
```

## Utilizzo

### Installazione

```bash
# Clona il repository
git clone https://github.com/your-organization/kaffeio.git
cd kaffeio/machine-simulator

# Installa le dipendenze
npm install
```

### Configurazione

Copia il file `.env.example` in `.env` e configura le variabili d'ambiente necessarie.

```bash
cp .env.example .env
```

### Avvio del Simulatore

```bash
# Modalità sviluppo
npm run start:dev

# Modalità produzione
npm run build
npm run start:prod
```

### Interfaccia Web

Il simulatore include un'interfaccia web accessibile all'indirizzo `http://localhost:3030` che permette di:

- Visualizzare lo stato delle macchine simulate
- Configurare i parametri di simulazione
- Visualizzare i dati generati in tempo reale
- Simulare scenari di errore o anomalie

## Integrazione con il Backend

Il simulatore comunica con il backend tramite le API REST. Per ogni dato generato, il simulatore effettua una richiesta POST all'endpoint `/api/telemetry` del backend.

### Autenticazione

Il simulatore utilizza un token JWT per autenticarsi con il backend. Il token può essere configurato tramite variabile d'ambiente o file di configurazione.

### Gestione degli Errori

Il simulatore implementa strategie di retry e circuit breaker per gestire eventuali errori di comunicazione con il backend.

## Scenari di Simulazione

Il simulatore supporta diversi scenari predefiniti per testare il comportamento del sistema in diverse condizioni.

### Scenario Normale

Simula il funzionamento normale delle macchine con dati che rientrano nei parametri attesi.

### Scenario Anomalia

Simula anomalie nelle macchine, generando dati che indicano potenziali problemi (temperature elevate, vibrazioni eccessive, ecc.).

### Scenario Errore

Simula errori nelle macchine, generando dati che indicano un malfunzionamento che richiede intervento.

## Estensione del Simulatore

Il simulatore è progettato per essere facilmente estensibile. Per aggiungere un nuovo tipo di macchina:

1. Creare una nuova classe che estende la classe base `MachineSimulator`
2. Implementare il metodo `generateData()` per generare i dati specifici del nuovo tipo di macchina
3. Registrare il nuovo tipo di macchina nel modulo principale del simulatore

Esempio:

```typescript
import { MachineSimulator } from './base-simulator';

export class NewMachineSimulator extends MachineSimulator {
  generateData() {
    return {
      machineId: this.machineId,
      lotId: this.lotId,
      dataType: 'new_machine_data',
      dataValue: {
        // Dati specifici del nuovo tipo di macchina
      },
      timestamp: new Date().toISOString(),
      timeZone: this.timeZone
    };
  }
}
```
