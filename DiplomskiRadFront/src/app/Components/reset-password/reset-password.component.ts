import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PasswordResetService } from '../../Services/password-reset.service';
import { ToastService } from '../../Services/toast.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
})
export class ResetPasswordComponent implements OnInit {
  form!: FormGroup;
  token: string | null = null;
  loading: boolean = false;
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private passwordResetService: PasswordResetService,
    private toastService: ToastService,
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParamMap.get('token');

    this.form = new FormGroup<any>({
      newPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
      ]),
      confirmPassword: new FormControl('', [Validators.required]),
    });

    if (!this.token) {
      this.errorMessage =
        'This reset link is missing its token. Please request a new one.';
    }
  }

  submit(): void {
    if (!this.token) {
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (
      this.form.get('newPassword')?.value !==
      this.form.get('confirmPassword')?.value
    ) {
      this.toastService.error('Passwords do not match.');
      return;
    }

    this.loading = true;

    this.passwordResetService
      .resetPassword(this.token, this.form.get('newPassword')?.value)
      .subscribe({
        next: () => {
          this.loading = false;
          this.toastService.success('Password updated! You can now log in.');
          this.router.navigate(['/login']);
        },
        error: (err: any) => {
          this.loading = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error ||
              'This reset link is invalid or has expired.',
          );
        },
      });
  }
}
