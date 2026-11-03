import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class CouponService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.coupons);
  }

  search(
    keyword?: string,
    includeInactive?: boolean,
    headers?: HttpHeaders,
  ): Observable<any> {
    let newUrl: string = config.SERVER + endpoints.coupons;
    const params: string[] = [];

    if (keyword) {
      params.push(`Keyword=${encodeURIComponent(keyword)}`);
    }

    if (includeInactive) {
      params.push(`IncludeInactive=true`);
    }

    if (params.length > 0) {
      newUrl += `?${params.join('&')}`;
    }

    return this.http.get(newUrl, { headers });
  }

  updateCoupon(
    id: number,
    dataToSend: any,
    headers?: HttpHeaders,
  ): Observable<any> {
    return this.http.put(
      `${config.SERVER}${endpoints.coupons}/${id}`,
      dataToSend,
      { headers },
    );
  }

  validate(code: string, headers?: HttpHeaders): Observable<any> {
    return this.http.post(
      `${config.SERVER}${endpoints.coupons}/validate`,
      { Code: code },
      { headers },
    );
  }
}
