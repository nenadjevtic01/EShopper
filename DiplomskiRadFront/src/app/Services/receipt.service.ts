import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class ReceiptService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.receipt);
  }

  search(
    userId?: number,
    includeInactive?: boolean,
    headers?: HttpHeaders,
  ): Observable<any> {
    let newUrl: string = config.SERVER + endpoints.receipt;
    const params: string[] = [];

    if (userId) {
      params.push(`UserId=${userId}`);
    }

    if (includeInactive) {
      params.push(`IncludeInactive=true`);
    }

    if (params.length > 0) {
      newUrl += `?${params.join('&')}`;
    }

    return this.http.get(newUrl, { headers });
  }

  confirmOrder(
    userId: number,
    paymentMethodId: number,
    billing: {
      firstName: string;
      lastName: string;
      email: string;
      address: string;
      city: string;
      country: string;
      postalCode: string;
    },
    couponCode?: string,
    headers?: HttpHeaders,
  ): Observable<any> {
    return this.http.post(
      `${config.SERVER}${endpoints.receipt}/${userId}`,
      {
        PaymentMethodId: paymentMethodId,
        BillingFirstName: billing.firstName,
        BillingLastName: billing.lastName,
        BillingEmail: billing.email,
        BillingAddress: billing.address,
        BillingCity: billing.city,
        BillingCountry: billing.country,
        BillingPostalCode: billing.postalCode,
        CouponCode: couponCode,
      },
      { headers },
    );
  }
}
