import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-terms',
  templateUrl: './terms.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, CommonModule]
})
export class TermsComponent {}
