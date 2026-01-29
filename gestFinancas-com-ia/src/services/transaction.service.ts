
import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../src/environments/environment';
import { Observable } from 'rxjs';

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
  private apiUrl = `${environment.apiURL}/api/Transaction`;
  
  readonly transactions = signal<Transaction[]>([]);
  readonly draftTransactions = signal<DraftTransaction[]>([]);

  constructor(private http: HttpClient) {
    this.loadTransactions();
  }

  loadTransactions(): void {
    this.http.get<any>(`${this.apiUrl}`).subscribe({
      next: (response) => {
        const transactionsData = Array.isArray(response) ? response : response?.data || [];
        const mappedTransactions = transactionsData.map((t: any) => ({
          id: t.id || t.Id,
          description: t.description || t.Description,
          amount: t.amount || t.Amount,
          date: t.date || t.Date,
          type: (t.type || t.Type)?.toLowerCase() === 'income' ? 'income' : 'expense',
          category: t.category || t.Category || 'Sem categoria'
        }));
        this.transactions.set(mappedTransactions);
      },
      error: (error) => {
        console.error('Erro ao carregar transações:', error);
        this.transactions.set([]);
      }
    });
  }

  addTransaction(args: AddTransactionArgs): void {
    const newTransaction: Transaction = {
      id: Math.max(...this.transactions().map(t => t.id), 0) + 1,
      description: args.description,
      amount: args.amount,
      date: args.date || new Date().toISOString().split('T')[0],
      type: args.type,
      category: args.category
    };

    // Enviar para o backend
    const payload = {
      description: newTransaction.description,
      amount: newTransaction.amount,
      date: newTransaction.date,
      type: newTransaction.type === 'income' ? 'Income' : 'Expense',
      category: newTransaction.category
    };

    this.http.post(`${this.apiUrl}`, payload).subscribe({
      next: (response: any) => {
        const createdTransaction: Transaction = {
          id: response?.data?.id || response?.id || newTransaction.id,
          description: newTransaction.description,
          amount: newTransaction.amount,
          date: newTransaction.date,
          type: newTransaction.type,
          category: newTransaction.category
        };
        this.transactions.update(transactions => 
          [...transactions, createdTransaction].sort((a, b) => 
            new Date(b.date).getTime() - new Date(a.date).getTime()
          )
        );
      },
      error: (error) => {
        console.error('Erro ao criar transação:', error);
      }
    });
  }

  createDraftTransaction(args: AddTransactionArgs) {
    const newDraft: DraftTransaction = {
      id: Date.now(),
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
  
  updateTransaction(updatedTransaction: Transaction): void {
    this.http.put(`${this.apiUrl}/${updatedTransaction.id}`, updatedTransaction).subscribe({
      next: () => {
        this.transactions.update(transactions => 
          transactions.map(t => t.id === updatedTransaction.id ? updatedTransaction : t)
        );
      },
      error: (error) => {
        console.error('Erro ao atualizar transação:', error);
      }
    });
  }

  deleteTransaction(id: number): void {
    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      next: () => {
        this.transactions.update(transactions => transactions.filter(t => t.id !== id));
      },
      error: (error) => {
        console.error('Erro ao deletar transação:', error);
      }
    });
  }
}
