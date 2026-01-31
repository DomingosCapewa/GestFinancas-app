import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { decodeToken, isTokenExpired, DecodedToken } from '../../utils/jwt.util';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UsuarioService {
  private apiUrl = `${environment.apiURL}/api/Usuario`;
  constructor(private http: HttpClient) {}

  estaAutenticado(): boolean {
    const token = localStorage.getItem('token');
    if (!token) return false;
    return !isTokenExpired(token);
  }

  getUsuarioLogado(): DecodedToken | null {
    const token = localStorage.getItem('token');
    if (!token) return null;
    return decodeToken(token);
  }

  autorizar(): boolean {
    localStorage.setItem('token', 'true');
    return true;
  }
  cadastrar(account: {
    nome: string;
    email: string;
    senha: string;
  }): Observable<any> {
    console.log('Calling endpoint:', `${this.apiUrl}`);
    return this.http
      .post(`${this.apiUrl}/cadastrar-usuario`, account)
      .pipe(
        map((response: any) => {
          const token = response?.data?.token;
          if (token) {
            localStorage.setItem('token', token);
          }
          return response;
        })
      );
  }

  login(email: string, senha: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { email, senha }).pipe(
      map((response) => {
        const token = response?.data?.token;
        if (token) {
          localStorage.setItem('token', token);
        } else {
          console.warn('Token não encontrado na resposta do backend');
        }

        return response;
      })
    );
  }

  logout(): void {
    return localStorage.removeItem('token');
  }
  resetPassword(data: {
    password: string;
    passwordConfirm: string;
  }): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/resetar-senha`, data)
      .pipe(map((response) => response));
  }

  confirmarResetSenha(data: {
    password: string;
    passwordConfirm: string;
  }): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/confirmar-reset-senha`, data)
      .pipe(map((response) => response));
  }

  refreshToken(): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/refresh-token`, {})
      .pipe(map((response) => response));
  }

  registrarConsentimento(consentType: string, version: string, accepted: boolean): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/consent`, { consentType, version, accepted })
      .pipe(map((response) => response));
  }
  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
