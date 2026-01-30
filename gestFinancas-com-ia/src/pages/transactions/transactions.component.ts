
import { Component, ChangeDetectionStrategy, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TransactionService, Transaction, AddTransactionArgs } from '../../services/transaction.service';
import { TransactionForm, DraftTransaction } from '../../models/transactions';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink, RouterLinkActive, FormsModule]
})
export class TransactionsComponent implements OnInit {
  private transactionService = inject(TransactionService);
  private router = inject(Router);

  ngOnInit(): void {
    // Recarregar transações quando o componente for inicializado
    this.transactionService.loadTransactions();
  }

  userName = signal('Domingos');
  isModalOpen = signal(false);
  isEditMode = signal(false);
  
  transactionId = signal<number | null>(null);
  description = signal('');
  amount = signal<number | null>(null);
  date = signal('');
  type = signal<'income' | 'expense'>('expense');
  category = signal('');

  // sinal para distinguir formulário manual vs rascunhos gerados pela IA
  manualForm = signal<TransactionForm>({
    description: '',
    amount: null,
    date: '',
    type: 'expense',
    category: ''
  });

  aiDrafts = signal<DraftTransaction[]>([]);

  categories = signal(['Moradia', 'Alimentação', 'Contas', 'Lazer', 'Salário', 'Freelance', 'Transporte', 'Saúde', 'Outros']);

  transactions = this.transactionService.transactions;

  filterText = signal('');
  filterType = signal<'all' | 'income' | 'expense'>('all');

  filteredTransactions = computed(() => {
    const text = this.filterText().toLowerCase();
    const type = this.filterType();

    return this.transactions().filter(t => {
      const typeMatch = type === 'all' || t.type === type;
      const textMatch = t.description.toLowerCase().includes(text) || t.category.toLowerCase().includes(text);
      return typeMatch && textMatch;
    }).sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
  });

  logout() {
    this.router.navigate(['/login']);
  }

  private resetForm() {
    this.transactionId.set(null);
    this.description.set('');
    this.amount.set(null);
    this.date.set('');
    this.type.set('expense');
    this.category.set('');
    this.manualForm.set({
      description: '',
      amount: null,
      date: '',
      type: 'expense',
      category: ''
    });
  }

  openAddModal() {
    this.isEditMode.set(false);
    this.resetForm();
    const today = new Date().toISOString().split('T')[0];
    this.date.set(today);
    this.manualForm.update(f => ({ ...f, date: today }));
    this.isModalOpen.set(true);
  }

  openEditModal(transaction: Transaction) {
    this.isEditMode.set(true);
    this.transactionId.set(transaction.id);
    this.description.set(transaction.description);
    this.amount.set(transaction.amount);
    this.date.set(transaction.date);
    this.type.set(transaction.type);
    this.category.set(transaction.category);
    this.manualForm.set({
      description: transaction.description,
      amount: transaction.amount,
      date: transaction.date,
      type: transaction.type,
      category: transaction.category
    });
    this.isModalOpen.set(true);
  }

  closeModal() {
    this.isModalOpen.set(false);
  }

  saveTransaction() {
    if (!this.description() || !this.amount() || !this.date() || !this.category()) {
      alert('Por favor, preencha todos os campos.');
      return;
    }
    
    if (this.isEditMode()) {
      const updatedTransaction: Transaction = {
        id: this.transactionId()!,
        description: this.description(),
        amount: this.amount()!,
        date: this.date(),
        type: this.type(),
        category: this.category(),
      };
      this.transactionService.updateTransaction(updatedTransaction);
    } else {
      const newTransaction: AddTransactionArgs = {
        description: this.description(),
        amount: this.amount()!,
        date: this.date(),
        type: this.type(),
        category: this.category()
      };
      this.transactionService.addTransaction(newTransaction);
    }

    this.closeModal();
  }

  deleteTransaction(id: number) {
    if (confirm('Tem certeza que deseja excluir esta transação?')) {
      this.transactionService.deleteTransaction(id);
    }
  }
}
