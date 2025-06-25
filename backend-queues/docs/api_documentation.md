# Kaffeio API Documentation

## Panoramica

Questa documentazione fornisce informazioni dettagliate sulle API disponibili nel backend di Kaffeio. Tutte le richieste API utilizzano il formato JSON per l'invio e la ricezione dei dati.

## Informazioni Generali

- **URL Base**: `http://localhost:3000/api`
- **Autenticazione**: La maggior parte degli endpoint richiede un token JWT nell'header della richiesta
- **Formato Header di Autenticazione**: `Authorization: Bearer <token>`
- **Formato Risposta**: Tutte le risposte sono in formato JSON

## Autenticazione

### Login

Autentica un utente e restituisce un token JWT.

- **URL**: `/auth/login`
- **Metodo**: `POST`
- **Richiede Autenticazione**: No
- **Parametri Body**:
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
- **Risposta di Successo**:
  ```json
  {
    "access_token": "string",
    "user": {
      "id": "string",
      "email": "string",
      "firstName": "string",
      "lastName": "string"
    }
  }
  ```
- **Codici di Errore**:
  - `401 Unauthorized`: Credenziali non valide

## Gestione Utenti

### Crea Utente

Crea un nuovo utente nel sistema.

- **URL**: `/users`
- **Metodo**: `POST`
- **Richiede Autenticazione**: Dipende dall'implementazione (potrebbe richiedere privilegi di amministratore)
- **Parametri Body**:
  ```json
  {
    "email": "string",
    "password": "string",
    "firstName": "string",
    "lastName": "string"
  }
  ```
- **Risposta di Successo**:
  ```json
  {
    "id": "string",
    "email": "string",
    "firstName": "string",
    "lastName": "string",
    "createdAt": "string",
    "updatedAt": "string"
  }
  ```
- **Codici di Errore**:
  - `400 Bad Request`: Dati utente non validi
  - `409 Conflict`: Email già in uso

### Ottieni Tutti gli Utenti

Recupera un elenco di tutti gli utenti.

- **URL**: `/users`
- **Metodo**: `GET`
- **Richiede Autenticazione**: Sì
- **Parametri Query** (opzionali):
  - `page`: Numero di pagina (default: 1)
  - `limit`: Numero di elementi per pagina (default: 10)
- **Risposta di Successo**:
  ```json
  [
    {
      "id": "string",
      "email": "string",
      "firstName": "string",
      "lastName": "string",
      "createdAt": "string",
      "updatedAt": "string"
    }
  ]
  ```

### Ottieni Utente per ID

Recupera un utente specifico tramite il suo ID.

- **URL**: `/users/:id`
- **Metodo**: `GET`
- **Richiede Autenticazione**: Sì
- **Parametri URL**:
  - `id`: ID dell'utente
- **Risposta di Successo**:
  ```json
  {
    "id": "string",
    "email": "string",
    "firstName": "string",
    "lastName": "string",
    "createdAt": "string",
    "updatedAt": "string"
  }
  ```
- **Codici di Errore**:
  - `404 Not Found`: Utente non trovato

### Aggiorna Utente

Aggiorna i dati di un utente esistente.

- **URL**: `/users/:id`
- **Metodo**: `PUT`
- **Richiede Autenticazione**: Sì
- **Parametri URL**:
  - `id`: ID dell'utente
- **Parametri Body** (tutti opzionali):
  ```json
  {
    "email": "string",
    "firstName": "string",
    "lastName": "string",
    "password": "string"
  }
  ```
- **Risposta di Successo**:
  ```json
  {
    "id": "string",
    "email": "string",
    "firstName": "string",
    "lastName": "string",
    "updatedAt": "string"
  }
  ```
- **Codici di Errore**:
  - `400 Bad Request`: Dati utente non validi
  - `404 Not Found`: Utente non trovato
  - `409 Conflict`: Email già in uso

### Elimina Utente

Elimina un utente dal sistema.

- **URL**: `/users/:id`
- **Metodo**: `DELETE`
- **Richiede Autenticazione**: Sì
- **Parametri URL**:
  - `id`: ID dell'utente
- **Risposta di Successo**:
  ```json
  {
    "message": "Utente eliminato con successo"
  }
  ```
- **Codici di Errore**:
  - `404 Not Found`: Utente non trovato

## Note sull'Autenticazione

- Il token JWT deve essere incluso nell'header `Authorization` con il prefisso `Bearer`
- Il token JWT ha una durata limitata (configurabile, default 24 ore)
- Quando il token scade, è necessario effettuare nuovamente il login

## Gestione degli Errori

Tutti gli errori restituiscono un oggetto JSON con la seguente struttura:

```json
{
  "statusCode": 400,
  "message": "Descrizione dell'errore",
  "error": "Nome dell'errore"
}
```

## Requisiti di Sicurezza

- Tutte le richieste devono essere effettuate tramite HTTPS in ambiente di produzione
- I dati sensibili (come le password) vengono sempre crittografati prima di essere memorizzati nel database
- Le API implementano protezioni contro attacchi comuni come CSRF, XSS e injection
