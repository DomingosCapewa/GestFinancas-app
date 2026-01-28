
import { Component, ChangeDetectionStrategy, signal, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Form, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UsuarioService } from '../../services/auth/usuario.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, ReactiveFormsModule, CommonModule]
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  
  constructor(private router: Router,
              private fb: FormBuilder,
              private usuarioService: UsuarioService
  ) {}

  ngOnInit(): void {
            this.loginForm = this.fb.group({
            email: new FormControl('', [Validators.required, Validators.email]),
            password: new FormControl('', Validators.required),
      });
  }

  get email() {
    return this.loginForm.get('email')!.value;
  }

  get password() {
    return this.loginForm.get('password')!.value;
  }

  set email(value: string) {
    this.loginForm.get('email')!.setValue(value);
  }

  set password(value: string) {
    this.loginForm.get('password')!.setValue(value);
  }

  
  login() {
    if (!this.loginForm.valid) {
      console.warn('Formulário inválido', this.loginForm.errors);
      return;
    }
    const { email, password } = this.loginForm.value;

    if (this.usuarioService.estaAutenticado()) {
      console.log('Usuário já está autenticado');
      this.router.navigate(['/dashboard']);
      return;
    }
     this.usuarioService.login(email, password).subscribe({
      next: (response) => {
        console.log('Login realizado com sucesso', response); 
        this.router.navigate(['/dashboard']); 
      },
      error: (error) => {
        console.error('Erro no login', error);
        alert('Falha no login: ' + (error.error?.message || error.message));
      },
    });
  }
}
