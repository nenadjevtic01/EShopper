import { Component, OnInit } from '@angular/core';
import { CartService } from '../../Services/cart.service';
import { Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { UserService } from '../../Services/user.service';
import { ReceiptService } from '../../Services/receipt.service';
import { ToastService } from '../../Services/toast.service';
import { PaymentMethodService } from '../../Services/payment-method.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CouponService } from '../../Services/coupon.service';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent implements OnInit {
  cart: any;
  headers: any;
  userId: number = 0;
  user: any;
  dataLoaded: boolean = false;
  userDataLoaded: boolean = false;
  cartTotal: number = 0;
  paymentMethod: number = 0;
  paymentChecked: boolean = false;
  paymentMethods: any[] = [];
  billingForm!: FormGroup;

  couponCode: string = '';
  couponApplied: boolean = false;
  couponMessage: string = '';
  couponValid: boolean = false;
  applyingCoupon: boolean = false;
  discountPercentage: number = 0;

  constructor(
    private jwtHelper: JwtHelperService,
    private router: Router,
    private cartService: CartService,
    private userService: UserService,
    private receiptService: ReceiptService,
    private toastService: ToastService,
    private paymentMethodService: PaymentMethodService,
    private couponService: CouponService,
  ) {}

  ngOnInit(): void {
    this.billingForm = new FormGroup<any>({
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      email: new FormControl('', [
        Validators.required,
        Validators.email,
        Validators.maxLength(100),
      ]),
      address: new FormControl('', [Validators.required]),
      city: new FormControl('', [Validators.required]),
      country: new FormControl('', [Validators.required]),
      postalCode: new FormControl('', [Validators.required]),
    });

    let token: string | null = localStorage.getItem('token');

    if (!token) {
      this.router.navigate(['/home']);
    } else {
      this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      let decodedToken = this.jwtHelper.decodeToken(token);
      this.userId = decodedToken.UserId;

      if (this.jwtHelper.isTokenExpired(token)) {
        localStorage.removeItem('token');
        this.router.navigate(['/home']);
      } else {
        this.showCart();
        this.fillFields();
        this.loadPaymentMethods();
      }
    }
  }

  loadPaymentMethods() {
    this.paymentMethodService.getPaymentMethods(this.headers).subscribe({
      next: (data: any) => {
        this.paymentMethods = data;
      },
      error: (err: any) => {
        this.toastService.errorFrom(err, 'Could not load payment methods.');
      },
    });
  }

  showCart() {
    this.cartService.get(this.userId, this.headers).subscribe({
      next: (data: any) => {
        this.cart = data;
        this.cartTotal = 0;

        this.cart.items.forEach((item: any) => {
          this.cartTotal = this.cartTotal + item.totalPrice;
        });

        this.dataLoaded = true;
      },
      error: (err: any) => {
        this.toastService.errorFrom(err, 'Could not load your cart.');
      },
    });
  }

  checkDisabled() {
    this.paymentChecked = this.paymentMethod != 0;
  }

  get shippingFee(): number {
    return Math.round(this.cartTotal * 20) / 100;
  }

  get discountAmount(): number {
    return this.couponApplied
      ? Math.round(this.cartTotal * this.discountPercentage) / 100
      : 0;
  }

  applyCoupon(): void {
    if (!this.couponCode) {
      return;
    }

    this.applyingCoupon = true;

    this.couponService.validate(this.couponCode, this.headers).subscribe({
      next: (data: any) => {
        this.applyingCoupon = false;
        this.couponValid = data.valid;
        this.couponMessage = data.message;
        this.couponApplied = data.valid;
        this.discountPercentage = data.valid ? data.discountPercentage : 0;
      },
      error: () => {
        this.applyingCoupon = false;
        this.couponValid = false;
        this.couponApplied = false;
        this.couponMessage = 'Could not check this coupon. Please try again.';
      },
    });
  }

  removeCoupon(): void {
    this.couponCode = '';
    this.couponApplied = false;
    this.couponMessage = '';
    this.discountPercentage = 0;
  }

  fillFields() {
    this.userService.get(this.userId, this.headers).subscribe({
      next: (data: any) => {
        this.user = data;
        this.userDataLoaded = true;

        this.billingForm.patchValue({
          firstName: data.firstName,
          lastName: data.lastName,
          email: data.email,
          address: data.info?.address,
          city: data.info?.city,
          country: data.info?.country,
          postalCode: data.info?.postalCode,
        });
      },
      error: (err: any) => {
        if (err.status === 403) {
          this.toastService.error('You do not have access to that account.');
          this.router.navigate(['/home']);
          return;
        }

        this.toastService.errorFrom(err, 'Could not load your details.');
      },
    });
  }

  placeOrder() {
    if (this.billingForm.invalid) {
      this.billingForm.markAllAsTouched();
      this.toastService.error('Please fill in your billing details.');
      return;
    }

    if (!this.paymentChecked) {
      this.toastService.error('Please select a payment method.');
      return;
    }

    const billing = {
      firstName: this.billingForm.get('firstName')?.value,
      lastName: this.billingForm.get('lastName')?.value,
      email: this.billingForm.get('email')?.value,
      address: this.billingForm.get('address')?.value,
      city: this.billingForm.get('city')?.value,
      country: this.billingForm.get('country')?.value,
      postalCode: this.billingForm.get('postalCode')?.value,
    };

    this.receiptService
      .confirmOrder(
        this.userId,
        this.paymentMethod,
        billing,
        this.couponApplied ? this.couponCode : undefined,
        this.headers,
      )
      .subscribe({
        next: (data: any) => {
          this.cart.items = [];
          this.cartService.updateCartCount(0);
          this.toastService.success(
            'Order placed! Thank you for shopping with us.',
          );
          this.router.navigate(['order-confirmation', data.receiptId]);
        },
        error: (err: any) => {
          this.toastService.errorFrom(
            err,
            'Could not place order. Please try again.',
          );
        },
      });
  }
}
