import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class PaymentMethodService {
  private path: string = endpoints.paymentMethods;

  constructor(private http: HttpClient) {}

  getPaymentMethods(
    headers?: HttpHeaders,
    includeInactive?: boolean,
  ): Observable<any> {
    let url = `${config.SERVER}${this.path}`;

    if (includeInactive) {
      url += `?includeInactive=true`;
    }

    return this.http.get(url, { headers });
  }

  create(dataToSend: any, headers?: HttpHeaders): Observable<any> {
    return this.http.post(`${config.SERVER}${this.path}`, dataToSend, {
      headers,
    });
  }

  update(id: number, dataToSend: any, headers?: HttpHeaders): Observable<any> {
    return this.http.put(`${config.SERVER}${this.path}/${id}`, dataToSend, {
      headers,
    });
  }

  delete(id: number, headers?: HttpHeaders): Observable<any> {
    return this.http.delete(`${config.SERVER}${this.path}/${id}`, { headers });
  }
}
