export interface TransactionForm {
  description: string;
  amount: number | null;
  date: string;
  type: 'income' | 'expense';
  category: string;
}

export interface DraftTransaction extends TransactionForm {
  id?: string;
  confidence?: number;
  note?: string; 
}
