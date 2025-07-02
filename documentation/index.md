# Kaffeio - Documentazione Completa

## Indice della Documentazione

Benvenuti nella documentazione completa del progetto Kaffeio, la piattaforma cloud-native per il monitoraggio, la schedulazione e la simulazione della produzione industriale di CoffeeMek S.p.A.

### Documentazione Generale

- [README - Panoramica del Progetto](./README.md)
- [Architettura](./architecture.md)
- [Deployment e Configurazione](./deployment.md)
- [Sicurezza](./security.md)

### API Reference

- [API - Autenticazione e Utenti](./api.md)
- [API - Strutture e Macchine](./api-facilities-machines.md)
- [API - Lotti e Telemetria](./api-lots-telemetry.md)

### Componenti del Sistema

- [Simulatore di Macchine](./machine-simulator.md)
- [Sistema di Code](./backend-queues.md)

## Panoramica della Documentazione

### Documentazione Generale

La sezione di documentazione generale fornisce una panoramica completa del progetto Kaffeio, della sua architettura, del processo di deployment e delle misure di sicurezza implementate.

- **README**: Fornisce una panoramica del progetto, della sua struttura, dei componenti e delle tecnologie utilizzate.
- **Architettura**: Descrive in dettaglio l'architettura del sistema, i componenti, il flusso dei dati e le considerazioni di scalabilità e sicurezza.
- **Deployment e Configurazione**: Fornisce istruzioni dettagliate per il deployment del sistema in diversi ambienti (sviluppo, produzione, Kubernetes) e la configurazione di ciascun componente.
- **Sicurezza**: Descrive le misure di sicurezza implementate nel sistema, inclusi autenticazione, autorizzazione, protezione dei dati e best practices.

### API Reference

La sezione API Reference fornisce una documentazione dettagliata di tutte le API REST esposte dal backend del sistema Kaffeio.

- **API - Autenticazione e Utenti**: Documenta le API per l'autenticazione e la gestione degli utenti.
- **API - Strutture e Macchine**: Documenta le API per la gestione delle strutture produttive e delle macchine.
- **API - Lotti e Telemetria**: Documenta le API per la gestione dei lotti di produzione e dei dati di telemetria.

### Componenti del Sistema

La sezione Componenti del Sistema fornisce documentazione dettagliata sui componenti specifici del sistema Kaffeio.

- **Simulatore di Macchine**: Descrive il componente che simula il comportamento delle macchine reali presenti nelle strutture produttive.
- **Sistema di Code**: Descrive il componente che gestisce l'elaborazione asincrona dei dati di telemetria e altre operazioni asincrone.

## Guida Rapida

### Installazione e Avvio

```bash
# Clonare il repository
git clone https://github.com/your-organization/kaffeio.git
cd kaffeio

# Configurare l'ambiente
cd docker
cp .env.example .env
# Modificare il file .env con le configurazioni appropriate

# Avviare il sistema in modalità sviluppo
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d
```

### Accesso al Sistema

- **Backend API**: http://localhost:3000/api
- **Documentazione API (Swagger)**: http://localhost:3000/api/docs
- **Frontend**: http://localhost:80
- **Monitoraggio Code**: http://localhost:3001/queues
- **Simulatore di Macchine**: http://localhost:3030

## Contribuire alla Documentazione

Se desideri contribuire alla documentazione del progetto Kaffeio, segui queste linee guida:

1. Assicurati che la documentazione sia chiara, concisa e completa
2. Utilizza il formato Markdown per tutti i documenti
3. Segui la struttura esistente della documentazione
4. Aggiungi esempi di codice e screenshot quando appropriato
5. Aggiorna l'indice della documentazione quando aggiungi nuovi documenti

## Contatti e Supporto

Per domande o supporto riguardo al progetto Kaffeio, contatta:

- **Email**: support@coffeemek.com
- **Issue Tracker**: https://github.com/your-organization/kaffeio/issues
