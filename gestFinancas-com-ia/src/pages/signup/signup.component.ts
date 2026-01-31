
import { Component, ChangeDetectionStrategy, signal, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UsuarioService } from '../../services/auth/usuario.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, ReactiveFormsModule, CommonModule]
})
export class SignupComponent implements OnInit {
 signupForm! : FormGroup;

  constructor(private router: Router,
              private usuarioService: UsuarioService,
  ) {}
  ngOnInit(): void {
   this.signupForm = new FormGroup({
                      name: new FormControl('', Validators.required),
                      email: new FormControl('', [Validators.required, Validators.email]),
            password: new FormControl('', Validators.required),
            acceptTerms: new FormControl(false, Validators.requiredTrue),
                });
  }

  signup() {
    if (this.signupForm.valid) {
      const { name, email, password } = this.signupForm.value;
      const userObj = {
        nome: name,
        email: email,
        senha: password,
      };

      this.usuarioService.cadastrar(userObj).subscribe({
        next: (response) => {
          console.log('Cadastro realizado com sucesso', response);
          this.usuarioService.registrarConsentimento('TermsAndPrivacy', 'v1', true).subscribe({
            next: () => this.router.navigate(['/login']),
            error: (error) => {
              console.error('Erro ao registrar consentimento', error);
              this.router.navigate(['/login']);
            },
          });
        },
        error: (error) => {
          console.error('Erro no cadastro', error);
        },
      });
    } else {
      console.warn('Formulário inválido', this.signupForm.errors);
      return;
    }
  }
}