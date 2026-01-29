import { Component, ChangeDetectionStrategy, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JuliusAgentService, DraftTransaction } from '../../services/julius-agent.service';

export interface Message {
  role: 'user' | 'model';
  text: string;
}

@Component({
  selector: 'app-ai-agent',
  templateUrl: './ai-agent.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule]
})
export class AiAgentComponent implements OnInit {
  private juliusService = inject(JuliusAgentService);
  
  drafts = signal<DraftTransaction[]>([]);
  
  userInput = signal('');
  messages = signal<Message[]>([
    { role: 'model', text: 'Escuta aqui, o que você fez com o meu dinheiro? Cada centavo conta. Me diga o que foi.' }
  ]);
  isLoading = signal(false);  
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  // ID do usuário (pegar do localStorage ou auth service)
  // NOTA: Backend espera GUID. Ajuste para o ID real do usuário logado
  private userId: string = '00000000-0000-0000-0000-000000000002';

  constructor() {
    const stored = localStorage.getItem('userId');
    if (stored) {
      this.userId = stored;
    }
  }

  ngOnInit() {
    this.loadDrafts();
  }

  loadDrafts() {
    console.log('🔍 Carregando drafts para userId:', this.userId);
    this.juliusService.getDraftsByUser(this.userId).subscribe({
      next: (drafts) => {
        console.log('Drafts recebidos:', drafts);
        console.log('Quantidade de drafts:', drafts.length);
        this.drafts.set(drafts);
      },
      error: (error) => {
        console.error('Erro ao carregar drafts:', error);
        console.error('Detalhes do erro:', error.error);
      }
    });
  }

  sendMessage() {
    const userMessage = this.userInput().trim();
    if (!userMessage) return;

    this.messages.update(m => [...m, { role: 'user', text: userMessage }]);
    this.userInput.set('');
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.juliusService.sendMessage(userMessage, this.userId).subscribe({
      next: (response) => {
        this.messages.update(m => [...m, { role: 'model', text: response.response }]);
        this.isLoading.set(false);
        this.loadDrafts();
      },
      error: (error) => {
        console.error('Erro ao comunicar com Julius:', error);
        this.errorMessage.set('Erro ao comunicar com o agente Julius. Verifique se ele está rodando.');
        this.messages.update(m => [...m, { 
          role: 'model', 
          text: 'Julius: Deu ruim aqui, Chris. Tenta de novo mais tarde. (Verifique se o agente está rodando em http://localhost:8000)' 
        }]);
        this.isLoading.set(false);
      }
    });
  }

  onConfirmTransaction(draftId: string) {
    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.juliusService.confirmDraft(draftId).subscribe({
      next: () => {
        this.successMessage.set('Transação confirmada com sucesso!');
        this.loadDrafts();
        this.isLoading.set(false);
                setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Erro ao confirmar transação:', error);
        this.errorMessage.set('Erro ao confirmar transação. Tente novamente.');
        this.isLoading.set(false);
      }
    });
  }

  onRejectTransaction(draftId: string) {
    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.juliusService.rejectDraft(draftId).subscribe({
      next: () => {
        this.successMessage.set('Transação rejeitada.');
        this.loadDrafts();
        this.isLoading.set(false);
                setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Erro ao rejeitar transação:', error);
        this.errorMessage.set('Erro ao rejeitar transação. Tente novamente.');
        this.isLoading.set(false);
      }
    });
  }

  testCreateDraft() {
    const testDraft = {
      userId: this.userId,
      amount: 50.00,
      description: 'Teste de Draft - Uber',
      category: 'Transporte',
      type: 'Expense',
      date: new Date().toISOString()
    };

    console.log(' Criando draft de teste:', testDraft);
    
    this.juliusService.createDraft(testDraft).subscribe({
      next: (response) => {
        console.log(' Draft criado:', response);
        this.successMessage.set('Draft de teste criado!');
        this.loadDrafts();
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Erro ao criar draft:', error);
        this.errorMessage.set('Erro ao criar draft de teste');
      }
    });
  }
}
