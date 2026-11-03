import { Component, OnInit } from '@angular/core';
import { CartService } from '../../Services/cart.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { HttpHeaders } from '@angular/common/http';
import { ToastService } from '../../Services/toast.service';
import { GuestCartService } from '../../Services/guest-cart.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class CartComponent implements OnInit {
  dataLoaded: boolean = false;
  cart: any;
  headers: any;
  userId: number = 0;
  cartTotal: number = 0;

  get shippingFee(): number {
    return Math.round(this.cartTotal * 20) / 100;
  }

  constructor(
    private jwtHelper: JwtHelperService,
    private cartService: CartService,
    private toastService: ToastService,
    private guestCartService: GuestCartService,
  ) {}

  ngOnInit() {
    let token: string | null = localStorage.getItem('token');

    if (!token) {
      this.showGuestCart();
    } else {
      this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      let decodedToken = this.jwtHelper.decodeToken(token);
      this.userId = decodedToken.UserId;

      if (this.jwtHelper.isTokenExpired(token)) {
        localStorage.removeItem('token');
        this.showGuestCart();
      } else {
        this.showCart();
      }
    }
  }

  isLoggedIn(): boolean {
    return localStorage.getItem('token') != null;
  }

  showGuestCart() {
    const items = this.guestCartService.getItems();
    this.cart = { cartId: null, items: items };
    this.cartTotal = this.guestCartService.total();
    this.dataLoaded = true;
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

  increaseQuantity(productId: number, size: number) {
    if (!this.isLoggedIn()) {
      this.guestCartService.changeQuantity(productId, size, 1);
      this.showGuestCart();
      return;
    }

    let cartItem = this.cart.items.find(
      (x: any) => x.productId == productId && x.sizeId == size,
    );

    if (cartItem) {
      let dataToSend = {
        CartItemId: cartItem.cartItemId,
        CartId: cartItem.cartId,
        productId: cartItem.productId,
        sizeId: cartItem.sizeId,
        quantity: cartItem.quantity + 1,
      };

      this.cartService.update(dataToSend, this.headers).subscribe({
        next: () => {
          this.showCart();
        },
        error: (err: any) => {
          this.toastService.errorFrom(err, 'Could not update quantity.');
        },
      });
    }
  }

  decreaseQuantity(productId: number, size: number) {
    if (!this.isLoggedIn()) {
      this.guestCartService.changeQuantity(productId, size, -1);
      this.showGuestCart();
      return;
    }

    let cartItem = this.cart.items.find(
      (x: any) => x.productId == productId && x.sizeId == size,
    );

    if (cartItem) {
      if (cartItem.quantity == 1) {
        this.removeCartItem(productId, size);
      } else {
        let dataToSend = {
          CartItemId: cartItem.cartItemId,
          CartId: cartItem.cartId,
          productId: cartItem.productId,
          sizeId: cartItem.sizeId,
          quantity: cartItem.quantity - 1,
        };

        this.cartService.update(dataToSend, this.headers).subscribe({
          next: () => {
            this.showCart();
          },
          error: (err: any) => {
            this.toastService.errorFrom(err, 'Could not update quantity.');
          },
        });
      }
    }
  }

  removeCartItem(productId: number, size: number) {
    if (!this.isLoggedIn()) {
      this.guestCartService.removeItem(productId, size);
      this.showGuestCart();
      this.toastService.success('Item removed from cart.');
      return;
    }

    let cartItem = this.cart.items.find(
      (x: any) => x.productId == productId && x.sizeId == size,
    );

    if (cartItem) {
      this.cartService.delete(cartItem.cartItemId, this.headers).subscribe({
        next: () => {
          this.showCart();
          this.toastService.success('Item removed from cart.');
        },
        error: () => {
          this.toastService.error('Could not remove item from cart.');
        },
      });
    }
  }
}
