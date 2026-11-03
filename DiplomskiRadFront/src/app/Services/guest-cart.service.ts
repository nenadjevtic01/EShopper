import { Injectable } from '@angular/core';
import { CartService } from './cart.service';

export interface IGuestCartItem {
  productId: number;
  productName: string;
  picture: { src: string; alt: string };
  sizeId: number;
  size: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

@Injectable({
  providedIn: 'root',
})
export class GuestCartService {
  private readonly key = 'guestCart';

  constructor(private cartService: CartService) {}

  getItems(): IGuestCartItem[] {
    try {
      return JSON.parse(localStorage.getItem(this.key) || '[]');
    } catch {
      return [];
    }
  }

  private save(items: IGuestCartItem[]): void {
    localStorage.setItem(this.key, JSON.stringify(items));
    this.cartService.updateCartCount(items.length);
  }

  addItem(item: {
    productId: number;
    productName: string;
    picture: { src: string; alt: string };
    sizeId: number;
    size: string;
    quantity: number;
    unitPrice: number;
  }): void {
    const items = this.getItems();
    const existing = items.find(
      (x) => x.productId === item.productId && x.sizeId === item.sizeId,
    );

    if (existing) {
      existing.quantity += item.quantity;
      existing.totalPrice = existing.quantity * existing.unitPrice;
    } else {
      items.push({ ...item, totalPrice: item.quantity * item.unitPrice });
    }

    this.save(items);
  }

  changeQuantity(productId: number, sizeId: number, delta: number): void {
    const items = this.getItems();
    const item = items.find(
      (x) => x.productId === productId && x.sizeId === sizeId,
    );

    if (!item) {
      return;
    }

    item.quantity += delta;

    if (item.quantity <= 0) {
      this.removeItem(productId, sizeId);
      return;
    }

    item.totalPrice = item.quantity * item.unitPrice;
    this.save(items);
  }

  removeItem(productId: number, sizeId: number): void {
    const items = this.getItems().filter(
      (x) => !(x.productId === productId && x.sizeId === sizeId),
    );
    this.save(items);
  }

  clear(): void {
    localStorage.removeItem(this.key);
    this.cartService.updateCartCount(0);
  }

  refreshCount(): void {
    this.cartService.updateCartCount(this.getItems().length);
  }

  total(): number {
    return this.getItems().reduce((sum, x) => sum + x.totalPrice, 0);
  }
}
