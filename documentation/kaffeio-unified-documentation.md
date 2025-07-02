# Kaffeio - Documentazione Unificata

## Indice
- [Kaffeio - Documentazione Unificata](#kaffeio---documentazione-unificata)
  - [Indice](#indice)
  - [Panoramica del Progetto](#panoramica-del-progetto)
    - [Obiettivi del Progetto](#obiettivi-del-progetto)
    - [Struttura del Progetto](#struttura-del-progetto)
    - [Stack Tecnologico](#stack-tecnologico)
    - [Funzionalità Principali](#funzionalità-principali)
  - [Architettura](#architettura)
    - [Panoramica dell'Architettura](#panoramica-dellarchitettura)
    - [Componenti Architetturali](#componenti-architetturali)
      - [Backend API](#backend-api)
      - [Database](#database)
      - [Sistema di Code](#sistema-di-code)
      - [Simulatore di Macchine](#simulatore-di-macchine)
      - [Frontend](#frontend)
    - [Flusso dei Dati](#flusso-dei-dati)
    - [Considerazioni sulla Scalabilità](#considerazioni-sulla-scalabilità)
    - [Considerazioni sulla Sicurezza](#considerazioni-sulla-sicurezza)
    - [Diagrammi](#diagrammi)
      - [Diagramma dell'Architettura](#diagramma-dellarchitettura)
      - [Diagramma del Flusso dei Dati](#diagramma-del-flusso-dei-dati)

## Panoramica del Progetto

Kaffeio è una piattaforma cloud-native sviluppata per CoffeeMek S.p.A., progettata per monitorare, schedulare e simulare la produzione industriale di macchine da caffè attraverso diverse strutture produttive globali.

### Obiettivi del Progetto

- Monitoraggio in tempo reale delle macchine industriali
- Gestione dei lotti di produzione
- Raccolta e analisi dei dati di telemetria
- Simulazione di scenari produttivi
- Supporto per strutture produttive in diversi fusi orari

### Struttura del Progetto

Il progetto Kaffeio è organizzato nei seguenti componenti principali:

- **Backend API**: Servizio NestJS che espone le API REST
- **Backend Queues**: Servizio NestJS che gestisce le code di elaborazione asincrona
- **Machine Simulator**: Servizio che simula le macchine industriali
- **Frontend**: Applicazione web per l'interfaccia utente
- **Database**: PostgreSQL per la persistenza dei dati
- **Redis**: Per la gestione delle code e caching

### Stack Tecnologico

- **Backend**: NestJS (Node.js), TypeScript
- **Database**: PostgreSQL
- **Code**: Bull + Redis
- **Autenticazione**: JWT
- **Containerizzazione**: Docker, Docker Compose
- **Orchestrazione**: Kubernetes (opzionale)
- **Frontend**: React, TypeScript

### Funzionalità Principali

1. **Gestione Utenti e Autenticazione**
   - Registrazione e login utenti
   - Autenticazione JWT
   - Controllo accessi basato su ruoli

2. **Gestione Strutture Produttive**
   - CRUD operazioni per strutture
   - Gestione dei fusi orari

3. **Gestione Macchine**
   - CRUD operazioni per macchine
   - Monitoraggio dello stato delle macchine
   - Tipi di macchine personalizzabili

4. **Gestione Lotti**
   - CRUD operazioni per lotti
   - Tracciamento dello stato dei lotti
   - Associazione lotti-macchine

5. **Telemetria**
   - Raccolta dati di telemetria dalle macchine
   - Elaborazione asincrona dei dati
   - Analisi e reportistica

6. **Simulazione**
   - Simulazione di macchine industriali
   - Generazione di dati di telemetria realistici
   - Simulazione di scenari di produzione e anomalie

## Architettura

### Panoramica dell'Architettura

L'architettura di Kaffeio è progettata per essere scalabile, resiliente e adatta a gestire grandi volumi di dati di telemetria provenienti da diverse strutture produttive globali.

### Componenti Architetturali

#### Backend API

Il backend API è il componente centrale del sistema che espone le API REST per l'interazione con il sistema. È implementato utilizzando NestJS, un framework Node.js che fornisce un'architettura modulare e scalabile.

**Caratteristiche principali:**
- API RESTful
- Autenticazione JWT
- Validazione e sanitizzazione degli input
- Gestione degli errori standardizzata
- Documentazione API integrata (Swagger)

#### Database

PostgreSQL è utilizzato come database principale per la persistenza dei dati. Il database memorizza tutte le informazioni relative a utenti, strutture, macchine, lotti e telemetria.

**Schema del database:**
- Users: Informazioni sugli utenti del sistema
- Facilities: Strutture produttive
- Machines: Macchine industriali
- Lots: Lotti di produzione
- Telemetry: Dati di telemetria dalle macchine

#### Sistema di Code

Il sistema di code è implementato utilizzando Bull, una libreria di code basata su Redis. Questo componente gestisce l'elaborazione asincrona dei dati di telemetria e altre operazioni che richiedono tempo.

**Code principali:**
- Telemetry Queue: Elaborazione dei dati di telemetria
- Notification Queue: Invio di notifiche
- Report Queue: Generazione di report

#### Simulatore di Macchine

Il simulatore di macchine è un componente che simula il comportamento delle macchine reali presenti nelle strutture produttive. Genera dati di telemetria realistici che vengono inviati al backend API.

**Tipi di macchine simulate:**
- Fresa CNC
- Tornio automatico
- Linea assemblaggio
- Linea di test

#### Frontend

Il frontend è un'applicazione web che fornisce un'interfaccia utente per interagire con il sistema. È implementato utilizzando React e TypeScript.

**Funzionalità principali:**
- Dashboard per il monitoraggio in tempo reale
- Gestione di strutture, macchine e lotti
- Visualizzazione dei dati di telemetria
- Configurazione del sistema

### Flusso dei Dati

1. **Raccolta dei Dati**
   - Le macchine reali o simulate inviano dati di telemetria al backend API
   - I dati vengono validati e salvati nel database

2. **Elaborazione dei Dati**
   - I dati di telemetria vengono inseriti nella coda di telemetria
   - Il sistema di code elabora i dati in modo asincrono
   - I risultati dell'elaborazione vengono salvati nel database

3. **Visualizzazione dei Dati**
   - Il frontend richiede i dati elaborati dal backend API
   - I dati vengono visualizzati in dashboard e report

### Considerazioni sulla Scalabilità

L'architettura di Kaffeio è progettata per essere scalabile orizzontalmente:

- **Backend API**: Può essere scalato orizzontalmente aggiungendo più istanze
- **Sistema di Code**: Supporta multiple worker instances per l'elaborazione parallela
- **Database**: Può essere configurato in modalità cluster o con replica
- **Redis**: Può essere configurato in modalità cluster

### Considerazioni sulla Sicurezza

La sicurezza è un aspetto fondamentale dell'architettura di Kaffeio:

- **Autenticazione**: JWT con scadenza configurabile
- **Autorizzazione**: Controllo accessi basato su ruoli
- **Protezione dei Dati**: Crittografia dei dati sensibili
- **Comunicazione**: HTTPS/TLS per tutte le comunicazioni
- **Input Validation**: Validazione e sanitizzazione di tutti gli input

### Diagrammi

#### Diagramma dell'Architettura

```
+----------------+      +----------------+      +----------------+
|                |      |                |      |                |
|    Frontend    |<---->|   Backend API  |<---->|    Database    |
|                |      |                |      |                |
+----------------+      +----------------+      +----------------+
                              ^   ^
                              |   |
                              v   v
+----------------+      +----------------+
|                |      |                |
|  Simulatore    |<---->| Sistema Code   |
|                |      |                |
+----------------+      +----------------+
```

#### Diagramma del Flusso dei Dati

```
+----------------+      +----------------+      +----------------+
|                |      |                |      |                |
|   Macchine     |----->|   Backend API  |----->|    Database    |
|                |      |                |      |                |
+----------------+      +----------------+      +----------------+
                              |
                              v
                        +----------------+
                        |                |
                        | Sistema Code   |
                        |                |
                        +----------------+
                              |
                              v
                        +----------------+
                        |                |
                        |    Frontend    |
                        |                |
                        +----------------+
```
