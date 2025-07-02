# Project Work - Cloud Monitoring della Produzione per CoffeeMek S.p.A.

## Obiettivo del progetto

[cite_start]Progettare e sviluppare una piattaforma cloud-native su Microsoft Azure per il monitoraggio, la schedulazione e la simulazione della produzione industriale di CoffeeMek S.p.A., azienda manifatturiera leader nella produzione di macchine da caffè professionali. [cite: 1] [cite_start]L'obiettivo è fornire una web application per la gestione delle commesse e dei lotti di produzione, integrata con un backend capace di ricevere e processare in tempo reale i dati provenienti dai macchinari delle diverse sedi produttive, distribuite su più fusi orari. [cite: 2]

## Contesto operativo

[cite_start]CoffeeMek S.p.A. ha sedi produttive in Italia, Brasile e Vietnam. [cite: 3] [cite_start]Ogni impianto è dotato di macchinari connessi (fresatrici, torni CNC, linee di assemblaggio e linee di test), che inviano periodicamente dati di produzione, necessari per tenere sotto controllo l'avanzamento dei lotti e la qualità del prodotto. [cite: 4]

## Tipologie di macchinari e dati inviati

| Macchinario           | Dati trasmessi                                                               |
|-----------------------|------------------------------------------------------------------------------|
| Fresa CNC             | Codice lotto, tempo ciclo, profondità taglio, vibrazione, allarmi utensile  |
| Tornio automatico     | Stato macchina, velocità rotazione, temperatura mandrino, numero pezzi completati |
| Linea assemblaggio    | Lotto associato, tempo medio per stazione, numero operatori, anomalie rilevate |
| Linea di test         | Esiti test funzionali, pressione e temperatura caldaia, consumo energetico  |
[cite_start][cite: 5, 6]

[cite_start]Tutti i dati devono essere associati al fuso orario della sede di provenienza, ma riportati anche in formato UTC per una visualizzazione aggregata. [cite: 7] Per la produzione di una macchina da caffè è previso il passaggio per:

* 1 lavorazione con Fresa CNC
* 1 lavorazione con Tornio
* 1 lavorazione in Linea di assemblaggio
* 1 lavorazione in Linea di test

[cite_start](utile per sapere lo stato di lavorazione del lotto in corso) [cite: 8]

## Requisiti applicativi

### Web Application (Frontend)

* [cite_start]Sviluppata in ASP.NET Core o Node.js [cite: 8]
* [cite_start]Possibilità di separare la parte client-side utilizzando tecnologie come React, Angular o Vue. [cite: 9]
* [cite_start]Funzionalità richieste: [cite: 10]
    * [cite_start]CRUD dei clienti e dei lotti di produzione [cite: 10]
    * [cite_start]Schedulazione lotti (data, ora, sede, commessa) [cite: 10]
    * [cite_start]Visualizzazione stato avanzamento e storico [cite: 10]
    * [cite_start]Dashboard con filtri per sede, stato, tipo macchina e cliente [cite: 10]

### Backend & API

* [cite_start]API RESTful per: [cite: 10]
    * [cite_start]Clienti [cite: 10]
    * [cite_start]Commesse [cite: 10]
    * [cite_start]Lotti [cite: 10]
    * [cite_start]Macchinari e dati di telemetria [cite: 10]
* [cite_start]Database su Azure SQL [cite: 10]
* [cite_start]Elaborazione asincrona dei dati ricevuti [cite: 10]

### Simulatore

* [cite_start]Microservizio o script in Node.js o .NET [cite: 10]
* [cite_start]Simula macchinari reali: [cite: 10]
    * [cite_start]Genera dati secondo intervalli realistici [cite: 10]
    * [cite_start]Log di tutti gli eventi [cite: 10]
    * [cite_start]Include fusi orari e timestamp precisi [cite: 10]