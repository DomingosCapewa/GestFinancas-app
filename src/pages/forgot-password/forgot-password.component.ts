
import { Component, ChangeDetectionStrategy, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, FormsModule, CommonModule]
})
export class ForgotPasswordComponent {
  email = signal('');
  
  constructor(private router: Router) {}

  resetPassword() {
    console.log('Password reset attempt for:', this.email());
    // Simulate successful password reset request
    alert('Um link para redefinição de senha foi enviado para o seu email.');
    this.router.navigate(['/login']);
  }
}
