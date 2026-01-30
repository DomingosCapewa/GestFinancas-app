import { Component, ChangeDetectionStrategy, signal, computed, inject, OnInit } from '@angular/core';
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
export class DashboardComponent implements OnInit {
  private transactionService = inject(TransactionService);
  private usuarioService = inject(UsuarioService);
  
  constructor(private router: Router) {
    if (!this.usuarioService.estaAutenticado()) {
      this.router.navigate(['/login']);
    }
  }

  userName = signal<string>('Domingos');
  
  ngOnInit(): void {
    const usuarioAtualizado = this.usuarioService.getUsuarioLogado();
    console.log('[Dashboard] Usuário logado:', usuarioAtualizado);
    
    if (usuarioAtualizado?.name) {
      this.userName.set(usuarioAtualizado.name);
      console.log('[Dashboard] Nome do usuário atualizado:', usuarioAtualizado.name);
    } else {
      console.warn('[Dashboard] Nome do usuário não encontrado no token');
    }
    
    // Recarregar transações quando o dashboard for inicializado
    this.transactionService.loadTransactions();
  }
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
