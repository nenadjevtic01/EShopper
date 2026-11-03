import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoginService } from '../../Services/login.service';
import { Router } from '@angular/router';
import { ToastService } from '../../Services/toast.service';
import { HttpHeaders } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { CartService } from '../../Services/cart.service';
import { GuestCartService } from '../../Services/guest-cart.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})
export class LoginComponent {
  loginData: any = {};
  loginForm!: FormGroup;

  constructor(
    private router: Router,
    private loginService: LoginService,
    private toastService: ToastService,
    private jwtHelper: JwtHelperService,
    private cartService: CartService,
    private guestCartService: GuestCartService,
  ) {}

  ngOnInit(): void {
    this.loginForm = new FormGroup<any>({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
    });
  }

  login(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    if (this.loginForm.valid) {
      let email = this.loginForm.get('email')?.value;
      let password = this.loginForm.get('password')?.value;

      this.loginData = {
        Email: email,
        Password: password,
      };

      this.loginService.create(this.loginData).subscribe({
        next: async (data: any) => {
          localStorage.setItem('token', data.token);
          await this.transferGuestCart(data.token);
          this.toastService.success('Welcome back!');
          this.router.navigate(['/home']);
        },
        error: () => {
          this.toastService.error(
            'User with provided email and password does not exist!',
          );
        },
      });
    }
  }

  private async transferGuestCart(token: string): Promise<void> {
    const guestItems = this.guestCartService.getItems();

    if (guestItems.length === 0) {
      return;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);

    if (Number(decodedToken.Role) === 1) {
      this.guestCartService.clear();
      return;
    }

    const userId = Number(decodedToken.UserId);
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    try {
      const existingCart: any = await firstValueFrom(
        this.cartService.get(userId, headers),
      );

      if (existingCart?.items?.length) {
        for (const item of existingCart.items) {
          await firstValueFrom(
            this.cartService.delete(item.cartItemId, headers),
          );
        }
      }

      for (const item of guestItems) {
        await firstValueFrom(
          this.cartService.create(
            {
              cartId: existingCart.cartId,
              productId: item.productId,
              sizeId: item.sizeId,
              quantity: item.quantity,
            },
            headers,
          ),
        );
      }

      this.guestCartService.clear();
      await firstValueFrom(this.cartService.get(userId, headers));
    } catch {}
  }
}
