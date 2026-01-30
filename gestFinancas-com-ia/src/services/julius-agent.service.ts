import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export interface ChatMessage {
  role: 'user' | 'assistant';
  text: string;
  timestamp?: Date;
}

export interface ChatResponse {
  response: string;
}

export interface DraftTransaction {
  id: string;
  userId: string;
  amount: number;
  category: string;
  type: 'Income' | 'Expense';
  description: string;
  confirmed: boolean;
  date: string;
}

@Injectable({
  providedIn: 'root'
})
export class JuliusAgentService {
  private agentBaseUrl = 'http://localhost:8000';
  private apiUrl = environment.apiURL;

  constructor(private http: HttpClient) {}

  sendMessage(message: string, userId: string): Observable<ChatResponse> {
    const token = localStorage.getItem('auth_token');
    return this.http.post<ChatResponse>(`${this.agentBaseUrl}/chat`, {
      message,
      user_id: userId,
      token: token
    });
  }

  getDraftsByUser(userId: string): Observable<DraftTransaction[]> {
    return this.http.get<DraftTransaction[]>(`${this.apiUrl}/ai/Transaction/drafts/${userId}`);
  }

  createDraft(draft: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/ai/Transaction/draft`, draft);
  }

  confirmDraft(draftId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/ai/Transaction/confirm/${draftId}`, {});
  }

  rejectDraft(draftId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/ai/Transaction/reject/${draftId}`, {});
  }
}
