export interface TransactionForm {
  description: string;
  amount: number | null;
  date: string;
  type: 'income' | 'expense';
  category: string;
}

export interface DraftTransaction extends TransactionForm {
  id?: string;
  confidence?: number; // opcional: score vindo do assistente
  note?: string; // observação ou origem do rascunho
}
