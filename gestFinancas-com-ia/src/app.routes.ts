
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(c => c.LoginComponent)
  },
  {
    path: 'signup',
    loadComponent: () => import('./pages/signup/signup.component').then(c => c.SignupComponent)
  },
  {
    path: 'forgot-password',
    loadComponent: () => import('./pages/forgot-password/forgot-password.component').then(c => c.ForgotPasswordComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(c => c.DashboardComponent),
    // canActivate: [() => inject(AuthService).isAuthenticated()]
  },
  {
    path: 'transactions',
    loadComponent: () => import('./pages/transactions/transactions.component').then(c => c.TransactionsComponent)
  },
  {
    path: 'reports',
    loadComponent: () => import('./pages/reports/reports.component').then(c => c.ReportsComponent)
  },
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/login'
  }
];
