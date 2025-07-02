# API Reference - Lotti e Telemetria

## Lotti (Lots)

I lotti rappresentano le unità di produzione che passano attraverso le diverse fasi di lavorazione nelle strutture produttive di CoffeeMek S.p.A.

### Ottieni Tutti i Lotti

```
GET /api/lots
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Risposta:**
```json
{
  "success": true,
  "data": [
    {
      "Id": 1,
      "Code": "LOT-001",
      "Description": "Macchine da caffè modello Pro X1",
      "CreatedAt": "2025-06-25T10:30:00.000Z",
      "Status": "Created",
      "CurrentMachine": {
        "id": 1,
        "name": "Fresa CNC 001",
        "type": "Fresa CNC",
        "status": "Running",
        "facilityId": 1
      }
    },
    {
      "Id": 2,
      "Code": "LOT-002",
      "Description": "Macchine da caffè modello Compact S2",
      "CreatedAt": "2025-06-25T11:45:00.000Z",
      "Status": "Processing",
      "CurrentMachine": null
    }
  ],
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/lots"
}
```

### Ottieni Lotto per ID

```
GET /api/lots/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "Id": 1,
    "Code": "LOT-001",
    "Description": "Macchine da caffè modello Pro X1",
    "CreatedAt": "2025-06-25T10:30:00.000Z",
    "Status": "Created",
    "CurrentMachine": {
      "id": 1,
      "name": "Fresa CNC 001",
      "type": "Fresa CNC",
      "status": "Running",
      "facilityId": 1
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/lots/1"
}
```

### Ottieni Lotto per Codice

```
GET /api/lots/code/:code
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "Id": 1,
    "Code": "LOT-001",
    "Description": "Macchine da caffè modello Pro X1",
    "CreatedAt": "2025-06-25T10:30:00.000Z",
    "Status": "Created",
    "CurrentMachine": {
      "id": 1,
      "name": "Fresa CNC 001",
      "type": "Fresa CNC",
      "status": "Running",
      "facilityId": 1
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/lots/code/LOT-001"
}
```

### Crea Lotto

```
POST /api/lots
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "Code": "LOT-003",
  "Description": "Macchine da caffè modello Barista Pro",
  "Status": "Created",
  "CurrentMachineId": 1
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "Id": 3,
    "Code": "LOT-003",
    "Description": "Macchine da caffè modello Barista Pro",
    "CreatedAt": "2025-06-25T14:00:00.000Z",
    "Status": "Created",
    "CurrentMachine": {
      "id": 1,
      "name": "Fresa CNC 001",
      "type": "Fresa CNC",
      "status": "Running",
      "facilityId": 1
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/lots"
}
```

### Aggiorna Lotto

```
PUT /api/lots/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "Description": "Macchine da caffè modello Pro X1 - Versione aggiornata",
  "Status": "Processing"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "Id": 1,
    "Code": "LOT-001",
    "Description": "Macchine da caffè modello Pro X1 - Versione aggiornata",
    "CreatedAt": "2025-06-25T10:30:00.000Z",
    "Status": "Processing",
    "CurrentMachine": {
      "id": 1,
      "name": "Fresa CNC 001",
      "type": "Fresa CNC",
      "status": "Running",
      "facilityId": 1
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/lots/1"
}
```

### Elimina Lotto

```
DELETE /api/lots/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "message": "Lotto con ID 1 è stato eliminato"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/lots/1"
}
```

### Stati dei Lotti

I lotti possono avere i seguenti stati:

- **Created**: Il lotto è stato creato ma non è ancora in lavorazione
- **Processing**: Il lotto è attualmente in lavorazione
- **Completed**: Il lotto ha completato tutte le fasi di lavorazione
- **OnHold**: La lavorazione del lotto è stata temporaneamente sospesa
- **Rejected**: Il lotto non ha superato i controlli di qualità

## Telemetria (Telemetry)

La telemetria rappresenta i dati inviati dalle macchine durante la lavorazione dei lotti.

### Ottieni Tutta la Telemetria (Paginata)

```
GET /api/telemetry?page=1&limit=100
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "data": [
      {
        "id": 1,
        "machineId": 1,
        "lotId": 1,
        "timestamp": "2025-06-25T12:30:00.000Z",
        "dataType": "temperature",
        "dataValue": "195.5",
        "machine": {
          "id": 1,
          "name": "Fresa CNC 001",
          "type": "Fresa CNC",
          "status": "Running",
          "facilityId": 1
        },
        "lot": {
          "Id": 1,
          "Code": "LOT-001",
          "Description": "Macchine da caffè modello Pro X1",
          "Status": "Processing"
        }
      },
      {
        "id": 2,
        "machineId": 1,
        "lotId": 1,
        "timestamp": "2025-06-25T12:31:00.000Z",
        "dataType": "vibration",
        "dataValue": "0.42",
        "machine": {
          "id": 1,
          "name": "Fresa CNC 001",
          "type": "Fresa CNC",
          "status": "Running",
          "facilityId": 1
        },
        "lot": {
          "Id": 1,
          "Code": "LOT-001",
          "Description": "Macchine da caffè modello Pro X1",
          "Status": "Processing"
        }
      }
    ],
    "total": 150,
    "page": 1,
    "limit": 100
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/telemetry"
}
```

### Ottieni Telemetria per ID

```
GET /api/telemetry/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "machineId": 1,
    "lotId": 1,
    "timestamp": "2025-06-25T12:30:00.000Z",
    "dataType": "temperature",
    "dataValue": "195.5",
    "machine": {
      "id": 1,
      "name": "Fresa CNC 001",
      "type": "Fresa CNC",
      "status": "Running",
      "facilityId": 1
    },
    "lot": {
      "Id": 1,
      "Code": "LOT-001",
      "Description": "Macchine da caffè modello Pro X1",
      "Status": "Processing"
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/telemetry/1"
}
```

### Ottieni Telemetria per Macchina (Paginata)

```
GET /api/telemetry/machine/:machineId?page=1&limit=100
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "data": [
      {
        "id": 1,
        "machineId": 1,
        "lotId": 1,
        "timestamp": "2025-06-25T12:30:00.000Z",
        "dataType": "temperature",
        "dataValue": "195.5",
        "machine": {
          "id": 1,
          "name": "Fresa CNC 001",
          "type": "Fresa CNC",
          "status": "Running",
          "facilityId": 1
        },
        "lot": {
          "Id": 1,
          "Code": "LOT-001",
          "Description": "Macchine da caffè modello Pro X1",
          "Status": "Processing"
        }
      }
    ],
    "total": 75,
    "page": 1,
    "limit": 100
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/telemetry/machine/1"
}
```

### Ottieni Telemetria per Lotto (Paginata)

```
GET /api/telemetry/lot/:lotId?page=1&limit=100
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "data": [
      {
        "id": 1,
        "machineId": 1,
        "lotId": 1,
        "timestamp": "2025-06-25T12:30:00.000Z",
        "dataType": "temperature",
        "dataValue": "195.5",
        "machine": {
          "id": 1,
          "name": "Fresa CNC 001",
          "type": "Fresa CNC",
          "status": "Running",
          "facilityId": 1
        },
        "lot": {
          "Id": 1,
          "Code": "LOT-001",
          "Description": "Macchine da caffè modello Pro X1",
          "Status": "Processing"
        }
      }
    ],
    "total": 50,
    "page": 1,
    "limit": 100
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/telemetry/lot/1"
}
```

### Ottieni Telemetria per Intervallo di Date (Paginata)

```
GET /api/telemetry/daterange?startDate=2025-06-25T00:00:00.000Z&endDate=2025-06-25T23:59:59.999Z&page=1&limit=100
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "data": [
      {
        "id": 1,
        "machineId": 1,
        "lotId": 1,
        "timestamp": "2025-06-25T12:30:00.000Z",
        "dataType": "temperature",
        "dataValue": "195.5",
        "machine": {
          "id": 1,
          "name": "Fresa CNC 001",
          "type": "Fresa CNC",
          "status": "Running",
          "facilityId": 1
        },
        "lot": {
          "Id": 1,
          "Code": "LOT-001",
          "Description": "Macchine da caffè modello Pro X1",
          "Status": "Processing"
        }
      }
    ],
    "total": 120,
    "page": 1,
    "limit": 100
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/telemetry/daterange"
}
```

### Crea Telemetria

```
POST /api/telemetry
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "machineId": 1,
  "lotId": 1,
  "dataType": "pressure",
  "dataValue": "12.5"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 3,
    "machineId": 1,
    "lotId": 1,
    "timestamp": "2025-06-25T14:10:00.000Z",
    "dataType": "pressure",
    "dataValue": "12.5",
    "machine": {
      "id": 1,
      "name": "Fresa CNC 001",
      "type": "Fresa CNC",
      "status": "Running",
      "facilityId": 1
    },
    "lot": {
      "Id": 1,
      "Code": "LOT-001",
      "Description": "Macchine da caffè modello Pro X1",
      "Status": "Processing"
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/telemetry"
}
```

### Tipi di Dati di Telemetria

I tipi di dati di telemetria variano in base al tipo di macchina:

#### Fresa CNC
- **temperature**: Temperatura dell'utensile (°C)
- **vibration**: Livello di vibrazione (mm/s)
- **depth**: Profondità di taglio (mm)
- **tool_alarm**: Allarmi relativi all'utensile (boolean/string)

#### Tornio Automatico
- **rotation_speed**: Velocità di rotazione (RPM)
- **spindle_temperature**: Temperatura del mandrino (°C)
- **completed_pieces**: Numero di pezzi completati (integer)

#### Linea Assemblaggio
- **station_time**: Tempo medio per stazione (secondi)
- **operators**: Numero di operatori (integer)
- **anomalies**: Anomalie rilevate (string)

#### Linea di Test
- **test_result**: Esito del test (pass/fail)
- **pressure**: Pressione della caldaia (bar)
- **temperature**: Temperatura della caldaia (°C)
- **energy_consumption**: Consumo energetico (kWh)
