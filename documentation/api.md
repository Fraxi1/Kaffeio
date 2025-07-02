# API Reference - Kaffeio

## Panoramica

Questa documentazione fornisce informazioni dettagliate sulle API disponibili nel backend di Kaffeio. Tutte le richieste API utilizzano il formato JSON per l'invio e la ricezione dei dati.

## Informazioni Generali

- **URL Base**: `http://localhost:3000/api`
- **Autenticazione**: La maggior parte degli endpoint richiede un token JWT nell'header della richiesta
- **Formato Header di Autenticazione**: `Authorization: Bearer <token>`
- **Formato Risposta**: Tutte le risposte sono in formato JSON

### Formato Standard delle Risposte

Tutte le risposte API seguono questo formato standard:

```json
{
    "success": true,
    "data": [], // Contiene i dati effettivi della risposta
    "timestamp": "2025-06-29T10:03:53.602Z", // Timestamp ISO di quando è stata generata la risposta
    "path": "/api/endpoint" // L'endpoint API che è stato chiamato
}
```

In caso di errori, la risposta avrà `success: false` e includerà un messaggio di errore:

```json
{
    "success": false,
    "message": "Descrizione dell'errore",
    "timestamp": "2025-06-29T10:03:53.602Z",
    "path": "/api/endpoint"
}
```

## Indice

- [Autenticazione](#autenticazione)
- [Utenti](#utenti)
- [Strutture](#strutture)
- [Macchine](#macchine)
- [Lotti](#lotti)
- [Telemetria](#telemetria)

## Autenticazione

### Registrazione di un Nuovo Utente

```
POST /api/auth/register
```

**Corpo della Richiesta:**
```json
{
  "email": "user@example.com",
  "password": "securePassword123",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/auth/register"
}
```

### Login

```
POST /api/auth/login
```

**Corpo della Richiesta:**
```json
{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 1,
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe"
    }
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/auth/login"
}
```

## Utenti

### Ottieni Tutti gli Utenti

```
GET /api/users
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
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe"
    },
    {
      "id": 2,
      "email": "user2@example.com",
      "firstName": "Jane",
      "lastName": "Smith"
    }
  ],
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/users"
}
```

### Ottieni Utente per ID

```
GET /api/users/:id
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
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/users/1"
}
```

### Crea Utente

```
POST /api/users
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "email": "newuser@example.com",
  "password": "securePassword123",
  "firstName": "New",
  "lastName": "User"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 3,
    "email": "newuser@example.com",
    "firstName": "New",
    "lastName": "User"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/users"
}
```

### Aggiorna Utente

```
PUT /api/users/:id
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Corpo della Richiesta:**
```json
{
  "firstName": "Updated",
  "lastName": "Name"
}
```

**Risposta:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "Updated",
    "lastName": "Name"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/users/1"
}
```

### Elimina Utente

```
DELETE /api/users/:id
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
    "message": "User with ID 1 has been deleted"
  },
  "timestamp": "2025-06-29T10:03:53.602Z",
  "path": "/api/users/1"
}
```
