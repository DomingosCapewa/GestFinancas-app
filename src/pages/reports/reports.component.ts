
import { Component, ChangeDetectionStrategy, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { TransactionService, Transaction } from '../../services/transaction.service';

interface CategorySpending {
  category: string;
  amount: number;
  percentage: number;
  color: string;
}

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink, RouterLinkActive]
})
export class ReportsComponent {
  private transactionService = inject(TransactionService);
  private router = inject(Router);
  
  userName = signal('Domingos');
  transactions = this.transactionService.transactions;
  
  totalIncome = computed(() => 
    this.transactions()
      .filter(t => t.type === 'income')
      .reduce((sum, t) => sum + t.amount, 0)
  );

  totalExpense = computed(() => 
    this.transactions()
      .filter(t => t.type === 'expense')
      .reduce((sum, t) => sum + t.amount, 0)
  );

  balance = computed(() => this.totalIncome() - this.totalExpense());

  spendingByCategory = computed<CategorySpending[]>(() => {
    const expenses = this.transactions().filter(t => t.type === 'expense');
    const total = this.totalExpense();
    if (total === 0) return [];

    const categoryMap = new Map<string, number>();
    for (const expense of expenses) {
      const currentAmount = categoryMap.get(expense.category) || 0;
      categoryMap.set(expense.category, currentAmount + expense.amount);
    }

    const colors = ['#3b82f6', '#10b981', '#ef4444', '#f97316', '#8b5cf6', '#ec4899', '#6b7280'];
    let colorIndex = 0;

    return Array.from(categoryMap.entries())
      .map(([category, amount]) => ({
        category,
        amount,
        percentage: (amount / total) * 100,
        color: colors[colorIndex++ % colors.length]
      }))
      .sort((a, b) => b.amount - a.amount);
  });

  maxSpending = computed(() => {
    const amounts = this.spendingByCategory().map(c => c.amount);
    return Math.max(...amounts, 0);
  });

  logout() {
    this.router.navigate(['/login']);
  }
}
