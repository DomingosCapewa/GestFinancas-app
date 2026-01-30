import { Component, ChangeDetectionStrategy, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AiAgentComponent } from '../../components/ai-agent/ai-agent.component';
import { TransactionService, Transaction } from '../../services/transaction.service';
import { UsuarioService } from '../../services/auth/usuario.service';
import { decodeToken } from '../../utils/jwt.util';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink, RouterLinkActive, AiAgentComponent]
})
export class DashboardComponent {
  private transactionService = inject(TransactionService);
  private usuarioService = inject(UsuarioService);
  
  private usuarioLogado = this.usuarioService.getUsuarioLogado();
  
  // Debug: Log do token
  constructor(private router: Router) {
    const token = localStorage.getItem('auth_token');
    if (token) {
      const decodedDebug = decodeToken(token);
      console.log('[DEBUG] Token decodificado:', decodedDebug);
      console.log('[DEBUG] Nome do usuário:', decodedDebug?.name);
    }
    
    if (!this.usuarioService.estaAutenticado()) {
      this.router.navigate(['/login']);
    }
  }
  
  userName = signal(this.usuarioLogado?.name || 'Usuário');
  transactions = this.transactionService.transactions;
  
  recentTransactions = computed(() => this.transactions().slice(0, 5));

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
  
  logout() {
    this.usuarioService.logout();
    this.router.navigate(['/login']);
  }

  getGreeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Bom dia';
    if (hour < 18) return 'Boa tarde';
    return 'Boa noite';
  }
}
