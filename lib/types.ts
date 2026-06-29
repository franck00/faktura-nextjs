export type Theme = 'light' | 'dark';
export type Lang  = 'fr' | 'en';
export type InvoiceStatus = 'paid' | 'pending' | 'overdue';
export type InvoiceTab    = 'all' | 'pending' | 'paid';
export type ScenarioKey   = 'invoice' | 'multi' | 'devis' | 'histo';

export interface Invoice {
  id:          string;
  client:      string;
  amount:      string;
  currency:    string;
  status:      InvoiceStatus;
  date:        string;
  desc:        string;
}

export type ChatMessage =
  | { kind: 'user';   text: string }
  | { kind: 'typing' }
  | { kind: 'bot';    text: string }
  | { kind: 'pdf';    pdfName: string; pdfSize: string };

export interface Scenario {
  msgs:   ChatMessage[];
  delays: number[];
}
