import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ApiService } from './api.service';
import { endpoints } from '../Constants/endpoints';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CartService extends ApiService {
  private cartCountSubject: BehaviorSubject<number> =
    new BehaviorSubject<number>(0);

  getCartCountObservable() {
    return this.cartCountSubject.asObservable();
  }

  updateCartCount(count: number) {
    this.cartCountSubject.next(count);
  }

  constructor(http: HttpClient) {
    super(http, endpoints.cart);
  }

  override get(id: number | string, headers?: HttpHeaders): Observable<any> {
    return super.get(id, headers).pipe(
      tap((data: any) => {
        if (data?.items) {
          this.updateCartCount(data.items.length);
        }
      }),
    );
  }
}
