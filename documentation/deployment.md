# Deployment e Configurazione - Kaffeio

## Panoramica

Questo documento descrive il processo di deployment e configurazione del sistema Kaffeio, una piattaforma cloud-native progettata per il monitoraggio, la schedulazione e la simulazione della produzione industriale di CoffeeMek S.p.A.

## Architettura di Deployment

Il sistema Kaffeio è composto da diversi componenti che possono essere deployati insieme o separatamente in base alle esigenze:

1. **Backend API** - Servizio NestJS che espone le API REST
2. **Backend Queues** - Servizio NestJS che gestisce le code di elaborazione asincrona
3. **Machine Simulator** - Servizio che simula le macchine industriali
4. **Frontend** - Applicazione web per l'interfaccia utente
5. **Database** - PostgreSQL per la persistenza dei dati
6. **Redis** - Per la gestione delle code e caching

![Architettura di Deployment](../assets/deployment-architecture.png)

## Requisiti di Sistema

### Ambiente di Produzione
- **CPU**: Minimo 4 core
- **RAM**: Minimo 8GB
- **Storage**: Minimo 50GB SSD
- **Sistema Operativo**: Linux (Ubuntu 20.04 LTS o superiore consigliato)
- **Docker**: Versione 20.10.x o superiore
- **Docker Compose**: Versione 2.x o superiore

### Ambiente di Sviluppo
- **CPU**: Minimo 2 core
- **RAM**: Minimo 4GB
- **Storage**: Minimo 20GB
- **Sistema Operativo**: Linux, macOS o Windows
- **Node.js**: Versione 16.x o superiore
- **npm**: Versione 8.x o superiore
- **Docker**: Versione 20.10.x o superiore
- **Docker Compose**: Versione 2.x o superiore

## Deployment con Docker Compose

Il metodo consigliato per il deployment del sistema Kaffeio è utilizzare Docker Compose, che permette di gestire facilmente tutti i componenti del sistema.

### Struttura del Repository

```
kaffeio/
├── backend/             # Backend API
├── backend-queues/      # Backend Queues
├── machine-simulator/   # Machine Simulator
├── frontend/            # Frontend
├── docker/              # File di configurazione Docker
│   ├── docker-compose.yml
│   ├── docker-compose.dev.yml
│   ├── docker-compose.prod.yml
│   └── .env.example
└── documentation/       # Documentazione
```

### Configurazione dell'Ambiente

1. Clonare il repository:

```bash
git clone https://github.com/your-organization/kaffeio.git
cd kaffeio/docker
```

2. Creare il file `.env` a partire dal file `.env.example`:

```bash
cp .env.example .env
```

3. Modificare il file `.env` con le configurazioni appropriate:

```
# Database
POSTGRES_USER=kaffeio
POSTGRES_PASSWORD=your-secure-password
POSTGRES_DB=kaffeio
POSTGRES_HOST=postgres
POSTGRES_PORT=5432

# Redis
REDIS_HOST=redis
REDIS_PORT=6379
REDIS_PASSWORD=your-secure-redis-password

# JWT
JWT_SECRET=your-jwt-secret
JWT_EXPIRATION_TIME=24h

# API
API_PORT=3000
API_PREFIX=/api
API_CORS_ORIGIN=https://your-frontend-domain.com

# Queue Service
QUEUE_SERVICE_PORT=3001

# Simulator
SIMULATOR_PORT=3030
SIMULATOR_API_BASE_URL=http://backend:3000/api
SIMULATOR_INTERVAL_MS=5000

# Frontend
FRONTEND_PORT=80
FRONTEND_API_URL=https://your-api-domain.com/api

# TLS/SSL
USE_HTTPS=true
SSL_CERT_PATH=/path/to/certificate.pem
SSL_KEY_PATH=/path/to/private-key.pem
```

### Avvio del Sistema in Produzione

Per avviare il sistema in ambiente di produzione:

```bash
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Avvio del Sistema in Sviluppo

Per avviare il sistema in ambiente di sviluppo:

```bash
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d
```

### Verifica del Deployment

Dopo l'avvio, è possibile verificare che tutti i servizi siano in esecuzione:

```bash
docker-compose ps
```

## Deployment Manuale

È possibile deployare manualmente ciascun componente del sistema Kaffeio, anche se questo approccio è consigliato solo per ambienti di sviluppo o test.

### Backend API

```bash
cd backend

# Installazione delle dipendenze
npm install

# Compilazione
npm run build

# Avvio in produzione
NODE_ENV=production npm run start:prod
```

### Backend Queues

```bash
cd backend-queues

# Installazione delle dipendenze
npm install

# Compilazione
npm run build

# Avvio in produzione
NODE_ENV=production npm run start:prod
```

### Machine Simulator

```bash
cd machine-simulator

# Installazione delle dipendenze
npm install

# Compilazione
npm run build

# Avvio in produzione
NODE_ENV=production npm run start:prod
```

### Frontend

```bash
cd frontend

# Installazione delle dipendenze
npm install

# Compilazione
npm run build

# Servire i file statici con Nginx o un altro web server
```

## Deployment su Kubernetes

Per ambienti di produzione più complessi o che richiedono alta disponibilità e scalabilità, è consigliato il deployment su Kubernetes.

### Prerequisiti

- Cluster Kubernetes (versione 1.19 o superiore)
- kubectl configurato per accedere al cluster
- Helm (versione 3.x o superiore)

### Deployment con Helm

1. Aggiungere il repository Helm di Kaffeio:

```bash
helm repo add kaffeio https://your-helm-repo.com
helm repo update
```

2. Creare un file `values.yaml` con le configurazioni personalizzate:

```yaml
# values.yaml
global:
  environment: production
  imageRegistry: your-registry.com

database:
  host: your-postgres-host
  port: 5432
  user: kaffeio
  password: your-secure-password
  database: kaffeio

redis:
  host: your-redis-host
  port: 6379
  password: your-secure-redis-password

api:
  replicas: 3
  resources:
    limits:
      cpu: 1
      memory: 1Gi
    requests:
      cpu: 500m
      memory: 512Mi
  jwt:
    secret: your-jwt-secret
    expirationTime: 24h

queueService:
  replicas: 2
  resources:
    limits:
      cpu: 1
      memory: 1Gi
    requests:
      cpu: 500m
      memory: 512Mi

simulator:
  replicas: 1
  resources:
    limits:
      cpu: 500m
      memory: 512Mi
    requests:
      cpu: 200m
      memory: 256Mi
  interval: 5000

frontend:
  replicas: 2
  resources:
    limits:
      cpu: 500m
      memory: 512Mi
    requests:
      cpu: 200m
      memory: 256Mi
  apiUrl: https://your-api-domain.com/api

ingress:
  enabled: true
  annotations:
    kubernetes.io/ingress.class: nginx
    cert-manager.io/cluster-issuer: letsencrypt-prod
  hosts:
    - host: api.your-domain.com
      paths:
        - path: /
          service: api
    - host: www.your-domain.com
      paths:
        - path: /
          service: frontend
  tls:
    - secretName: api-tls
      hosts:
        - api.your-domain.com
    - secretName: frontend-tls
      hosts:
        - www.your-domain.com
```

3. Installare il chart Helm:

```bash
helm install kaffeio kaffeio/kaffeio -f values.yaml
```

4. Verificare il deployment:

```bash
kubectl get pods
kubectl get services
kubectl get ingress
```

## Configurazione

### Variabili d'Ambiente

Il sistema Kaffeio utilizza variabili d'ambiente per la configurazione. Di seguito sono elencate le principali variabili d'ambiente supportate:

#### Backend API

| Variabile | Descrizione | Default |
|-----------|-------------|---------|
| `NODE_ENV` | Ambiente di esecuzione (development, production, test) | `development` |
| `PORT` | Porta su cui esporre il servizio | `3000` |
| `API_PREFIX` | Prefisso per le rotte API | `/api` |
| `DATABASE_URL` | URL di connessione al database | `postgresql://kaffeio:password@localhost:5432/kaffeio` |
| `JWT_SECRET` | Chiave segreta per la firma dei token JWT | - |
| `JWT_EXPIRATION_TIME` | Tempo di scadenza dei token JWT | `24h` |
| `CORS_ORIGIN` | Origine consentita per le richieste CORS | `*` |
| `REDIS_HOST` | Host Redis | `localhost` |
| `REDIS_PORT` | Porta Redis | `6379` |
| `REDIS_PASSWORD` | Password Redis | - |
| `USE_HTTPS` | Abilitare HTTPS | `false` |
| `SSL_CERT_PATH` | Percorso del certificato SSL | - |
| `SSL_KEY_PATH` | Percorso della chiave privata SSL | - |

#### Backend Queues

| Variabile | Descrizione | Default |
|-----------|-------------|---------|
| `NODE_ENV` | Ambiente di esecuzione (development, production, test) | `development` |
| `PORT` | Porta su cui esporre il servizio | `3001` |
| `DATABASE_URL` | URL di connessione al database | `postgresql://kaffeio:password@localhost:5432/kaffeio` |
| `REDIS_HOST` | Host Redis | `localhost` |
| `REDIS_PORT` | Porta Redis | `6379` |
| `REDIS_PASSWORD` | Password Redis | - |
| `TELEMETRY_QUEUE_CONCURRENCY` | Concorrenza della coda di telemetria | `5` |
| `NOTIFICATIONS_QUEUE_CONCURRENCY` | Concorrenza della coda di notifiche | `3` |
| `REPORTS_QUEUE_CONCURRENCY` | Concorrenza della coda di reportistica | `2` |

#### Machine Simulator

| Variabile | Descrizione | Default |
|-----------|-------------|---------|
| `NODE_ENV` | Ambiente di esecuzione (development, production, test) | `development` |
| `PORT` | Porta su cui esporre il servizio | `3030` |
| `SIMULATOR_API_BASE_URL` | URL base dell'API | `http://localhost:3000/api` |
| `SIMULATOR_API_AUTH_TOKEN` | Token di autenticazione per l'API | - |
| `SIMULATOR_INTERVAL_MS` | Intervallo di generazione dei dati (ms) | `5000` |

### File di Configurazione

Oltre alle variabili d'ambiente, è possibile utilizzare file di configurazione per ambienti specifici:

#### Backend API

```typescript
// config/production.ts
export default {
  database: {
    host: 'production-db-host',
    port: 5432,
    username: 'kaffeio',
    password: 'your-secure-password',
    database: 'kaffeio',
  },
  jwt: {
    secret: 'your-jwt-secret',
    expirationTime: '24h',
  },
  cors: {
    origin: 'https://your-frontend-domain.com',
  },
  // ...
};
```

## Backup e Ripristino

### Backup del Database

Per eseguire un backup del database PostgreSQL:

```bash
# Con Docker Compose
docker-compose exec postgres pg_dump -U kaffeio kaffeio > backup.sql

# Manualmente
pg_dump -h your-postgres-host -U kaffeio -d kaffeio -F c -f backup.dump
```

### Ripristino del Database

Per ripristinare un backup del database:

```bash
# Con Docker Compose
cat backup.sql | docker-compose exec -T postgres psql -U kaffeio -d kaffeio

# Manualmente
pg_restore -h your-postgres-host -U kaffeio -d kaffeio -c backup.dump
```

## Monitoraggio

### Logs

I log del sistema sono configurati per essere inviati a stdout/stderr, permettendo di utilizzare strumenti di logging come ELK Stack, Fluentd, o semplicemente Docker logs.

```bash
# Visualizzare i log di un servizio specifico
docker-compose logs -f backend

# Visualizzare i log di tutti i servizi
docker-compose logs -f
```

### Metriche

Il sistema espone metriche in formato Prometheus che possono essere raccolte e visualizzate con strumenti come Grafana.

Endpoint delle metriche:
- Backend API: `/api/metrics`
- Backend Queues: `/metrics`
- Machine Simulator: `/metrics`

### Health Checks

Ogni componente espone endpoint di health check per verificare lo stato del servizio:

- Backend API: `/api/health`
- Backend Queues: `/health`
- Machine Simulator: `/health`

## Troubleshooting

### Problemi Comuni e Soluzioni

#### Il backend non si connette al database

1. Verificare che il database sia in esecuzione:
```bash
docker-compose ps postgres
```

2. Verificare le credenziali nel file `.env`

3. Verificare i log del database:
```bash
docker-compose logs postgres
```

#### Il sistema di code non elabora i job

1. Verificare che Redis sia in esecuzione:
```bash
docker-compose ps redis
```

2. Verificare i log del servizio di code:
```bash
docker-compose logs backend-queues
```

3. Verificare la connessione a Redis:
```bash
docker-compose exec redis redis-cli -a your-redis-password ping
```

#### Il simulatore non invia dati

1. Verificare che il simulatore sia in esecuzione:
```bash
docker-compose ps machine-simulator
```

2. Verificare i log del simulatore:
```bash
docker-compose logs machine-simulator
```

3. Verificare la connessione al backend API:
```bash
docker-compose exec machine-simulator curl -I http://backend:3000/api/health
```

## Aggiornamenti e Manutenzione

### Aggiornamento del Sistema

Per aggiornare il sistema Kaffeio:

```bash
# Fermare i servizi
docker-compose down

# Aggiornare il repository
git pull

# Ricostruire le immagini
docker-compose build

# Riavviare i servizi
docker-compose up -d
```

### Manutenzione del Database

Per eseguire la manutenzione del database:

```bash
# Eseguire VACUUM
docker-compose exec postgres psql -U kaffeio -d kaffeio -c "VACUUM ANALYZE;"

# Eseguire la ricostruzione degli indici
docker-compose exec postgres psql -U kaffeio -d kaffeio -c "REINDEX DATABASE kaffeio;"
```

## Sicurezza

Per le considerazioni sulla sicurezza, fare riferimento al documento [security.md](./security.md).
