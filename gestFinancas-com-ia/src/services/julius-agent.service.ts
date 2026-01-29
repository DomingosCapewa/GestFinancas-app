import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ChatMessage {
  role: 'user' | 'assistant';
  text: string;
  timestamp?: Date;
}

export interface ChatResponse {
  response: string;
}

@Injectable({
  providedIn: 'root'
})
export class JuliusAgentService {
  private agentBaseUrl = 'http://localhost:8000';

  constructor(private http: HttpClient) {}

  sendMessage(message: string, userId: string): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(`${this.agentBaseUrl}/chat`, {
      message,
      user_id: userId
    });
  }
}
