# API Reference - Strutture e Macchine

## Strutture (Facilities)

Le strutture rappresentano le sedi produttive di CoffeeMek S.p.A. distribuite in diverse parti del mondo (Italia, Brasile e Vietnam).

### Ottieni Tutte le Strutture

```
GET /api/facilities
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
      "id": 1,
      "name": "Stabilimento Italia",
      "location": "Milano",
      "timeZone": "Europe/Rome"
    },
    {
      "id": 2,
      "name": "Stabilimento Brasile",
      "location": "San Paolo",
      "timeZone": "America/Sao_Paulo"
    },
    {
      "id": 3,
      "name": "Stabilimento Vietnam",
      "location": "Ho Chi Minh",
      "timeZone": "Asia/Ho_Chi_Minh"
    }
  ],
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/facilities"
}
```

### Ottieni Struttura per ID

```
GET /api/facilities/:id
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
    "name": "Stabilimento Italia",
    "location": "Milano",
    "timeZone": "Europe/Rome"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/facilities/1"
}
```

### Crea Struttura

```
POST /api/facilities
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "name": "Nuovo Stabilimento",
  "location": "Torino",
  "timeZone": "Europe/Rome"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 4,
    "name": "Nuovo Stabilimento",
    "location": "Torino",
    "timeZone": "Europe/Rome"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/facilities"
}
```

### Aggiorna Struttura

```
PUT /api/facilities/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "name": "Stabilimento Italia Aggiornato",
  "location": "Roma",
  "timeZone": "Europe/Rome"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Stabilimento Italia Aggiornato",
    "location": "Roma",
    "timeZone": "Europe/Rome"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/facilities/1"
}
```

### Elimina Struttura

```
DELETE /api/facilities/:id
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
    "message": "Struttura con ID 1 è stata eliminata"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/facilities/1"
}
```

## Macchine (Machines)

Le macchine rappresentano i diversi tipi di macchinari presenti nelle strutture produttive di CoffeeMek S.p.A.

### Tipi di Macchine

- **Fresa CNC**: Utilizzata per la lavorazione di componenti metallici
- **Tornio automatico**: Utilizzato per la lavorazione di componenti cilindrici
- **Linea assemblaggio**: Utilizzata per l'assemblaggio delle macchine da caffè
- **Linea di test**: Utilizzata per il controllo qualità delle macchine assemblate

### Ottieni Tutte le Macchine

```
GET /api/machines
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
      "id": 1,
      "name": "Fresa CNC 001",
      "type": "Fresa CNC",
      "status": "Idle",
      "facilityId": 1,
      "facility": {
        "id": 1,
        "name": "Stabilimento Italia",
        "location": "Milano",
        "timeZone": "Europe/Rome"
      }
    },
    {
      "id": 2,
      "name": "Tornio 002",
      "type": "Tornio automatico",
      "status": "Running",
      "facilityId": 1,
      "facility": {
        "id": 1,
        "name": "Stabilimento Italia",
        "location": "Milano",
        "timeZone": "Europe/Rome"
      }
    }
  ],
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/machines"
}
```

### Ottieni Macchine per Struttura

```
GET /api/machines/facility/:facilityId
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
      "id": 1,
      "name": "Fresa CNC 001",
      "type": "Fresa CNC",
      "status": "Idle",
      "facilityId": 1,
      "facility": {
        "id": 1,
        "name": "Stabilimento Italia",
        "location": "Milano",
        "timeZone": "Europe/Rome"
      }
    },
    {
      "id": 2,
      "name": "Tornio 002",
      "type": "Tornio automatico",
      "status": "Running",
      "facilityId": 1,
      "facility": {
        "id": 1,
        "name": "Stabilimento Italia",
        "location": "Milano",
        "timeZone": "Europe/Rome"
      }
    }
  ],
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/machines/facility/1"
}
```

### Ottieni Macchina per ID

```
GET /api/machines/:id
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
    "name": "Fresa CNC 001",
    "type": "Fresa CNC",
    "status": "Idle",
    "facilityId": 1,
    "facility": {
      "id": 1,
      "name": "Stabilimento Italia",
      "location": "Milano",
      "timeZone": "Europe/Rome"
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/machines/1"
}
```

### Crea Macchina

```
POST /api/machines
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "name": "Nuova Linea Test",
  "type": "Linea di test",
  "facilityId": 1,
  "status": "Idle"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 3,
    "name": "Nuova Linea Test",
    "type": "Linea di test",
    "status": "Idle",
    "facilityId": 1,
    "facility": {
      "id": 1,
      "name": "Stabilimento Italia",
      "location": "Milano",
      "timeZone": "Europe/Rome"
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/machines"
}
```

### Aggiorna Macchina

```
PUT /api/machines/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "name": "Fresa CNC 001 Aggiornata",
  "status": "Maintenance"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Fresa CNC 001 Aggiornata",
    "type": "Fresa CNC",
    "status": "Maintenance",
    "facilityId": 1,
    "facility": {
      "id": 1,
      "name": "Stabilimento Italia",
      "location": "Milano",
      "timeZone": "Europe/Rome"
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/machines/1"
}
```

### Aggiorna Stato Macchina

```
PATCH /api/machines/:id/status
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "status": "Running"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Fresa CNC 001",
    "type": "Fresa CNC",
    "status": "Running",
    "facilityId": 1,
    "facility": {
      "id": 1,
      "name": "Stabilimento Italia",
      "location": "Milano",
      "timeZone": "Europe/Rome"
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/machines/1/status"
}
```

### Elimina Macchina

```
DELETE /api/machines/:id
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
    "message": "Macchina con ID 1 è stata eliminata"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/machines/1"
}
```

## Stati delle Macchine

Le macchine possono avere i seguenti stati:

- **Idle**: La macchina è accesa ma non sta lavorando
- **Running**: La macchina sta lavorando attivamente
- **Maintenance**: La macchina è in manutenzione
- **Error**: La macchina ha riscontrato un errore
- **Offline**: La macchina è spenta o non raggiungibile
