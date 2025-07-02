export interface MachineData {
  macchina: string;
  esito_test: string;
  pressione_caldaia: number;
  temperatura_caldaia: number;
  consumo_energetico: number;
  luogo: string;
  timestamp_locale: string;
  timestamp_utc: string;
  blocco_macchina: boolean;
  motivo_blocco: string | null;
  ultima_manutenzione: string;
}
