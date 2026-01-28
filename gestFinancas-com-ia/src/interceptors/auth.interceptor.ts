import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
  HttpResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, switchMap, take, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { UsuarioService } from '../services/auth/usuario.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    private usuarioService: UsuarioService,
    private router: Router
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authReq = this.addAuthHeader(request);

    return next.handle(authReq).pipe(
      tap((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
        }
      }),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          return this.handle401Error(authReq, next);
        } else if (error.status === 403) {
          return this.handle403Error(error);
        } else {
          return this.handleOtherErrors(error);
        }
      })
    );
  }

  private addAuthHeader(request: HttpRequest<any>): HttpRequest<any> {
    const token = this.usuarioService.getToken();

    if (!token || request.url.includes('/auth/')) {
      return request;
    }

    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.usuarioService.refreshToken().pipe(
        switchMap((newToken: string) => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(newToken);
          return next.handle(this.addAuthHeader(request));
        }),
        catchError((err) => {
          this.isRefreshing = false;
          this.usuarioService.logout();
          this.router.navigate(['/login']);
          return throwError(err);
        })
      );
    } else {
      return this.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(token => {
          return next.handle(this.addAuthHeader(request));
        })
      );
    }
  }

  private handle403Error(error: HttpErrorResponse): Observable<HttpEvent<any>> {
    this.router.navigate(['/acesso-negado']);
    return throwError(error);
  }

  private handleOtherErrors(error: HttpErrorResponse): Observable<HttpEvent<any>> {
    const errorResponse = {
      timestamp: new Date().toISOString(),
      path: error.url,
      message: error.error?.message || error.message,
      details: error.error?.errors || null,
      status: error.status
    };

    console.error('HTTP Error:', errorResponse);

    return throwError(errorResponse);
  }
}
