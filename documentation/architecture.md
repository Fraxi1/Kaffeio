# Architettura di Kaffeio

## Panoramica dell'Architettura

Kaffeio è una piattaforma cloud-native progettata per il monitoraggio, la schedulazione e la simulazione della produzione industriale di CoffeeMek S.p.A. L'architettura è basata su microservizi per garantire scalabilità, resilienza e manutenibilità.

## Componenti del Sistema

![Architettura di Sistema](../assets/architecture-diagram.png)

### 1. Backend API (NestJS)

Il backend API è il componente principale che gestisce le richieste HTTP, l'autenticazione e le operazioni sul database. È sviluppato utilizzando NestJS, un framework progressivo per Node.js.

**Responsabilità principali:**
- Gestione delle richieste API RESTful
- Autenticazione e autorizzazione tramite JWT
- Validazione dei dati in ingresso
- Operazioni CRUD su entità come utenti, strutture, macchine e lotti
- Ricezione dei dati di telemetria dalle macchine

### 2. Backend Queues (NestJS + Bull)

Il servizio di code gestisce l'elaborazione asincrona dei dati di telemetria ricevuti dalle macchine. Utilizza Bull, una libreria di code basata su Redis, per gestire i job in modo affidabile.

**Responsabilità principali:**
- Elaborazione asincrona dei dati di telemetria
- Aggiornamento dello stato dei lotti in base ai dati ricevuti
- Generazione di notifiche e avvisi
- Calcolo di metriche e statistiche

### 3. Machine Simulator

Il simulatore di macchine è un componente che genera dati realistici per simulare il comportamento delle macchine in diverse strutture e fusi orari. È utile per test, demo e sviluppo.

**Responsabilità principali:**
- Simulazione di diversi tipi di macchine (fresatrici, torni, linee di assemblaggio, linee di test)
- Generazione di dati di telemetria realistici
- Simulazione di scenari di produzione
- Gestione dei fusi orari per le diverse sedi produttive

### 4. Database (PostgreSQL)

PostgreSQL è utilizzato come database principale per memorizzare tutti i dati dell'applicazione.

**Entità principali:**
- Utenti
- Strutture (Facilities)
- Macchine
- Lotti
- Dati di telemetria
- Clienti
- Commesse

### 5. Redis

Redis è utilizzato per la gestione delle code e il caching.

**Utilizzi principali:**
- Code di elaborazione per i dati di telemetria
- Caching per migliorare le prestazioni
- Gestione delle sessioni

## Flusso dei Dati

1. **Generazione dei dati:**
   - I dati vengono generati dalle macchine reali o dal simulatore
   - Ogni dato include informazioni sul lotto, sulla macchina e timestamp con fuso orario

2. **Ricezione e validazione:**
   - I dati vengono inviati al Backend API
   - L'API valida i dati e li salva nel database

3. **Elaborazione asincrona:**
   - I dati vengono inseriti in una coda per l'elaborazione
   - Il servizio Backend Queues elabora i dati in modo asincrono

4. **Aggiornamento dello stato:**
   - Lo stato dei lotti viene aggiornato in base ai dati elaborati
   - Le metriche e le statistiche vengono calcolate

5. **Accesso ai dati:**
   - Gli utenti accedono ai dati tramite l'API
   - I dati possono essere filtrati per struttura, macchina, lotto, cliente, ecc.

## Stack Tecnologico

### Backend
- **Framework:** NestJS
- **Linguaggio:** TypeScript
- **Database:** PostgreSQL
- **ORM:** TypeORM
- **Autenticazione:** JWT
- **Documentazione API:** Swagger/OpenAPI

### Code e Messaggistica
- **Sistema di code:** Bull
- **Store:** Redis

### Simulatore
- **Framework:** NestJS
- **Linguaggio:** TypeScript
- **Generazione dati:** Faker.js

### Containerizzazione e Deployment
- **Containerizzazione:** Docker
- **Orchestrazione:** Docker Compose
- **Cloud:** Microsoft Azure

## Considerazioni sulla Scalabilità

L'architettura di Kaffeio è progettata per essere scalabile orizzontalmente:

1. **Backend API:**
   - Può essere scalato orizzontalmente aggiungendo più istanze
   - Bilanciamento del carico tramite un load balancer

2. **Backend Queues:**
   - Scalabile aggiungendo più worker per elaborare le code
   - Redis cluster per gestire grandi volumi di dati

3. **Database:**
   - Sharding per distribuire il carico su più istanze
   - Replica per alta disponibilità

## Considerazioni sulla Sicurezza

1. **Autenticazione e Autorizzazione:**
   - JWT per l'autenticazione
   - Ruoli e permessi per l'autorizzazione
   - Refresh token per sessioni sicure

2. **Sicurezza dei Dati:**
   - Crittografia dei dati sensibili
   - HTTPS per tutte le comunicazioni
   - Sanitizzazione degli input

3. **Protezione dell'Infrastruttura:**
   - Firewall e gruppi di sicurezza
   - VPC e subnet private
   - Monitoraggio e logging

## Diagrammi

### Diagramma ER

![Diagramma ER](../assets/er-diagram.png)

### Diagramma di Sequenza

![Diagramma di Sequenza](../assets/sequence-diagram.png)

### Diagramma di Deployment

![Diagramma di Deployment](../assets/deployment-diagram.png)
