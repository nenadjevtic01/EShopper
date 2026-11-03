import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PasswordResetService } from '../../Services/password-reset.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
})
export class ForgotPasswordComponent {
  form!: FormGroup;
  submitted: boolean = false;
  loading: boolean = false;

  constructor(private passwordResetService: PasswordResetService) {}

  ngOnInit(): void {
    this.form = new FormGroup<any>({
      email: new FormControl('', [Validators.required, Validators.email]),
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;

    this.passwordResetService
      .forgotPassword(this.form.get('email')?.value)
      .subscribe({
        next: () => {
          this.loading = false;
          this.submitted = true;
        },
        error: () => {
          this.loading = false;
          this.submitted = true;
        },
      });
  }
}
