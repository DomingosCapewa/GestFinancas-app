import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { UsuarioService } from '../services/auth/usuario.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const usuarioService = inject(UsuarioService);
  const router = inject(Router);

  const token = usuarioService.getToken();

  // Não adicionar token em rotas de autenticação
  if (!token || req.url.includes('/login') || req.url.includes('/cadastrar')) {
    return next(req);
  }

  // Clonar a requisição e adicionar o header de autorização
  const authReq = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Token expirado ou inválido
        usuarioService.logout();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};
