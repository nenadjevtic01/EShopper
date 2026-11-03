import { Component } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../Services/user.service';
import { ToastService } from '../../Services/toast.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
})
export class RegisterComponent {
  registerForm!: FormGroup;
  errorMessage: string = '';

  constructor(
    private router: Router,
    private userService: UserService,
    private toastService: ToastService,
  ) {}

  ngOnInit(): void {
    this.registerForm = new FormGroup<any>(
      {
        firstName: new FormControl('', [Validators.required]),
        lastName: new FormControl('', [Validators.required]),
        email: new FormControl('', [
          Validators.required,
          Validators.email,
          Validators.maxLength(100),
        ]),
        userName: new FormControl('', [Validators.required]),
        password: new FormControl('', [
          Validators.required,
          Validators.minLength(6),
        ]),
        confirmPassword: new FormControl('', [Validators.required]),
        address: new FormControl(''),
        city: new FormControl(''),
        postalCode: new FormControl(''),
        country: new FormControl(''),
      },
      { validators: this.passwordsMatch },
    );
  }

  passwordsMatch(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;

    if (!confirmPassword || password === confirmPassword) {
      return null;
    }

    return { passwordsMismatch: true };
  }

  register(): void {
    this.errorMessage = '';

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    if (this.registerForm.valid) {
      const dataToSend = {
        FirstName: this.registerForm.get('firstName')?.value,
        LastName: this.registerForm.get('lastName')?.value,
        Email: this.registerForm.get('email')?.value,
        UserName: this.registerForm.get('userName')?.value,
        Password: this.registerForm.get('password')?.value,
        Address: this.registerForm.get('address')?.value,
        City: this.registerForm.get('city')?.value,
        PostalCode: this.registerForm.get('postalCode')?.value,
        Country: this.registerForm.get('country')?.value,
      };

      this.userService.create(dataToSend).subscribe({
        next: () => {
          this.toastService.success('Account created! You can now log in.');
          this.router.navigate(['/login']);
        },
        error: (err: any) => {
          this.errorMessage =
            err?.error?.errors?.[0]?.error ||
            'Registration failed. Email might already be in use.';
          this.toastService.error(this.errorMessage);
        },
      });
    }
  }
}
