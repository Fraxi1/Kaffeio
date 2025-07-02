# Sistema di Code - Kaffeio

## Panoramica

Il sistema di code di Kaffeio è un componente fondamentale dell'architettura che gestisce l'elaborazione asincrona dei dati di telemetria ricevuti dalle macchine. Questo componente è implementato come un servizio separato (`backend-queues`) che utilizza Bull, una libreria di code basata su Redis, per gestire i job in modo affidabile e scalabile.

## Funzionalità Principali

- Elaborazione asincrona dei dati di telemetria
- Aggiornamento dello stato dei lotti in base ai dati ricevuti
- Generazione di notifiche e avvisi in caso di anomalie
- Calcolo di metriche e statistiche per il monitoraggio della produzione
- Gestione dei fusi orari per le diverse strutture produttive

## Architettura

Il sistema di code è implementato come un'applicazione NestJS separata che si connette allo stesso database del backend principale. Utilizza Redis come broker di messaggi per la gestione delle code.

### Componenti del Sistema di Code

1. **Produttori (Producers)**: Componenti che inseriscono job nelle code
2. **Consumatori (Consumers)**: Componenti che elaborano i job dalle code
3. **Code (Queues)**: Code che contengono i job da elaborare
4. **Scheduler**: Gestisce i job schedulati per esecuzioni future
5. **Monitoraggio**: Fornisce informazioni sullo stato delle code e dei job

## Code Implementate

### 1. Coda di Telemetria

Questa coda gestisce l'elaborazione dei dati di telemetria ricevuti dalle macchine.

**Funzionalità:**
- Validazione dei dati di telemetria
- Salvataggio dei dati nel database
- Aggiornamento dello stato delle macchine
- Aggiornamento dello stato dei lotti
- Generazione di notifiche in caso di anomalie

**Esempio di job:**
```json
{
  "machineId": 1,
  "lotId": 1,
  "dataType": "temperature",
  "dataValue": "195.5",
  "timestamp": "2025-06-25T12:30:00.000Z",
  "timeZone": "Europe/Rome"
}
```

### 2. Coda di Notifiche

Questa coda gestisce l'invio di notifiche agli utenti in caso di eventi rilevanti.

**Funzionalità:**
- Invio di email
- Invio di notifiche push
- Invio di SMS
- Registrazione delle notifiche nel database

**Esempio di job:**
```json
{
  "type": "email",
  "recipient": "user@example.com",
  "subject": "Anomalia rilevata nella macchina Fresa CNC 001",
  "body": "La temperatura della macchina ha superato la soglia di sicurezza.",
  "priority": "high"
}
```

### 3. Coda di Reportistica

Questa coda gestisce la generazione di report periodici sulla produzione.

**Funzionalità:**
- Generazione di report giornalieri
- Generazione di report settimanali
- Generazione di report mensili
- Esportazione dei report in diversi formati (PDF, Excel, CSV)

**Esempio di job:**
```json
{
  "type": "daily_report",
  "facilityId": 1,
  "date": "2025-06-25",
  "format": "pdf",
  "recipients": ["manager@example.com"]
}
```

## Configurazione

Il sistema di code può essere configurato tramite file di configurazione o variabili d'ambiente.

### File di Configurazione

```json
{
  "redis": {
    "host": "localhost",
    "port": 6379,
    "password": "your-redis-password"
  },
  "queues": {
    "telemetry": {
      "concurrency": 5,
      "attempts": 3,
      "backoff": {
        "type": "exponential",
        "delay": 1000
      }
    },
    "notifications": {
      "concurrency": 3,
      "attempts": 5,
      "backoff": {
        "type": "fixed",
        "delay": 2000
      }
    },
    "reports": {
      "concurrency": 2,
      "attempts": 2,
      "backoff": {
        "type": "exponential",
        "delay": 5000
      }
    }
  }
}
```

### Variabili d'Ambiente

```
REDIS_HOST=localhost
REDIS_PORT=6379
REDIS_PASSWORD=your-redis-password
TELEMETRY_QUEUE_CONCURRENCY=5
TELEMETRY_QUEUE_ATTEMPTS=3
NOTIFICATIONS_QUEUE_CONCURRENCY=3
NOTIFICATIONS_QUEUE_ATTEMPTS=5
REPORTS_QUEUE_CONCURRENCY=2
REPORTS_QUEUE_ATTEMPTS=2
```

## Utilizzo

### Installazione

```bash
# Clona il repository
git clone https://github.com/your-organization/kaffeio.git
cd kaffeio/backend-queues

# Installa le dipendenze
npm install
```

### Configurazione

Copia il file `.env.example` in `.env` e configura le variabili d'ambiente necessarie.

```bash
cp .env.example .env
```

### Avvio del Sistema di Code

```bash
# Modalità sviluppo
npm run start:dev

# Modalità produzione
npm run build
npm run start:prod
```

## Monitoraggio

Il sistema di code include un'interfaccia di monitoraggio accessibile all'indirizzo `http://localhost:3001/queues` che permette di:

- Visualizzare lo stato delle code
- Visualizzare i job in attesa, in esecuzione, completati e falliti
- Riprovare job falliti
- Visualizzare statistiche sulle prestazioni delle code

## Gestione degli Errori

Il sistema di code implementa diverse strategie per gestire gli errori:

### Tentativi di Ripetizione (Retries)

I job falliti vengono automaticamente riprovati un numero configurabile di volte con un ritardo crescente tra i tentativi.

### Dead Letter Queue

I job che falliscono dopo tutti i tentativi di ripetizione vengono spostati in una "dead letter queue" per analisi successive.

### Circuit Breaker

Il sistema implementa un pattern circuit breaker per evitare di sovraccaricare sistemi esterni in caso di errori ripetuti.

## Scalabilità

Il sistema di code è progettato per essere scalabile orizzontalmente:

1. **Multiple Worker Instances**: È possibile avviare più istanze del servizio di code per elaborare i job in parallelo
2. **Redis Cluster**: Redis può essere configurato in modalità cluster per gestire grandi volumi di dati
3. **Sharding**: Le code possono essere shardizzate per distribuire il carico su più istanze di Redis

## Integrazione con il Backend

Il backend principale comunica con il sistema di code inserendo job nelle code appropriate quando necessario. Ad esempio, quando viene ricevuto un dato di telemetria tramite API, il backend inserisce un job nella coda di telemetria per l'elaborazione asincrona.

### Esempio di Integrazione

```typescript
// Nel backend principale
@Injectable()
export class TelemetryService {
  constructor(
    @InjectQueue('telemetry') private telemetryQueue: Queue,
  ) {}

  async processTelemetryData(data: TelemetryDto) {
    // Salva i dati grezzi nel database
    const telemetry = await this.telemetryRepository.save(data);
    
    // Inserisci un job nella coda per l'elaborazione asincrona
    await this.telemetryQueue.add('process-telemetry', {
      telemetryId: telemetry.id,
      machineId: data.machineId,
      lotId: data.lotId,
      dataType: data.dataType,
      dataValue: data.dataValue,
      timestamp: data.timestamp,
      timeZone: data.timeZone
    });
    
    return telemetry;
  }
}
```

```typescript
// Nel servizio di code
@Processor('telemetry')
export class TelemetryProcessor {
  constructor(
    private readonly telemetryService: TelemetryService,
    private readonly machineService: MachineService,
    private readonly lotService: LotService,
    @InjectQueue('notifications') private notificationsQueue: Queue,
  ) {}

  @Process('process-telemetry')
  async processTelemetry(job: Job<TelemetryJobData>) {
    const { telemetryId, machineId, lotId, dataType, dataValue, timestamp, timeZone } = job.data;
    
    // Elabora i dati di telemetria
    const processedData = await this.telemetryService.processData(telemetryId);
    
    // Aggiorna lo stato della macchina
    await this.machineService.updateMachineStatus(machineId, processedData);
    
    // Aggiorna lo stato del lotto
    await this.lotService.updateLotStatus(lotId, processedData);
    
    // Verifica se ci sono anomalie
    if (this.telemetryService.hasAnomalies(processedData)) {
      // Inserisci un job nella coda di notifiche
      await this.notificationsQueue.add('send-notification', {
        type: 'email',
        recipient: 'manager@example.com',
        subject: `Anomalia rilevata nella macchina ${machineId}`,
        body: `Anomalia rilevata: ${processedData.anomaly}`,
        priority: 'high'
      });
    }
    
    return { processed: true };
  }
}
```

## Considerazioni sulla Sicurezza

Il sistema di code implementa diverse misure di sicurezza:

1. **Autenticazione Redis**: Accesso a Redis protetto da password
2. **TLS/SSL**: Comunicazione crittografata con Redis
3. **Rate Limiting**: Limitazione del numero di job che possono essere inseriti nelle code
4. **Validazione dei Dati**: Validazione dei dati dei job prima dell'elaborazione
5. **Logging**: Registrazione di tutti gli eventi rilevanti per audit trail
