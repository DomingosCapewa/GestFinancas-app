
import { Injectable, signal } from '@angular/core';

export interface Transaction {
  id: number;
  description: string;
  amount: number;
  date: string;
  type: 'income' | 'expense';
  category: string;
}

export interface AddTransactionArgs {
  description: string;
  amount: number;
  type: 'income' | 'expense';
  category: string;
  date?: string;
}

export interface DraftTransaction extends AddTransactionArgs {
  id: number;
}

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  readonly transactions = signal<Transaction[]>([
    { id: 1, description: 'Salário Mensal', amount: 5000, date: '2024-07-01', type: 'income', category: 'Salário' },
    { id: 2, description: 'Aluguel', amount: 1500, date: '2024-07-05', type: 'expense', category: 'Moradia' },
    { id: 3, description: 'Supermercado', amount: 450, date: '2024-07-08', type: 'expense', category: 'Alimentação' },
    { id: 4, description: 'Freelance Website', amount: 800, date: '2024-07-10', type: 'income', category: 'Freelance' },
    { id: 5, description: 'Conta de Luz', amount: 120, date: '2024-07-12', type: 'expense', category: 'Contas' },
    { id: 6, description: 'Jantar fora', amount: 85, date: '2024-07-15', type: 'expense', category: 'Lazer' },
    { id: 7, description: 'Transporte', amount: 150, date: '2024-07-18', type: 'expense', category: 'Transporte' },
    { id: 8, description: 'Farmácia', amount: 60, date: '2024-07-22', type: 'expense', category: 'Saúde' },
  ]);

  readonly draftTransactions = signal<DraftTransaction[]>([]);

  addTransaction(args: AddTransactionArgs) {
    const newTransaction: Transaction = {
      id: Math.max(...this.transactions().map(t => t.id), 0) + 1,
      description: args.description,
      amount: args.amount,
      date: args.date || new Date().toISOString().split('T')[0],
      type: args.type,
      category: args.category
    };
    this.transactions.update(transactions => [...transactions, newTransaction].sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()));
  }

  createDraftTransaction(args: AddTransactionArgs) {
    const newDraft: DraftTransaction = {
      id: Date.now(), // Use timestamp for a simple unique ID
      ...args
    };
    this.draftTransactions.update(drafts => [...drafts, newDraft]);
  }

  confirmTransaction(draftId: number) {
    const draft = this.draftTransactions().find(d => d.id === draftId);
    if (draft) {
      this.addTransaction(draft);
      this.draftTransactions.update(drafts => drafts.filter(d => d.id !== draftId));
    }
  }

  rejectTransaction(draftId: number) {
    this.draftTransactions.update(drafts => drafts.filter(d => d.id !== draftId));
  }
  
  updateTransaction(updatedTransaction: Transaction) {
      this.transactions.update(transactions => 
        transactions.map(t => t.id === updatedTransaction.id ? updatedTransaction : t)
      );
  }

  deleteTransaction(id: number) {
      this.transactions.update(transactions => transactions.filter(t => t.id !== id));
  }
}
