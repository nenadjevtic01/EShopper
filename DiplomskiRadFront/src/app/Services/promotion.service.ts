import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class PromotionService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.promotions);
  }

  search(
    keyword?: string,
    includeInactive?: boolean,
    headers?: HttpHeaders,
  ): Observable<any> {
    let newUrl: string = config.SERVER + endpoints.promotions;
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

  getActive(): Observable<any> {
    return this.http.get(`${config.SERVER}${endpoints.promotions}/active`);
  }

  toggle(id: number, headers?: HttpHeaders): Observable<any> {
    return this.http.put(
      `${config.SERVER}${endpoints.promotions}/${id}/toggle`,
      null,
      { headers },
    );
  }

  updatePromotion(
    id: number,
    dataToSend: any,
    headers?: HttpHeaders,
  ): Observable<any> {
    return this.http.put(
      `${config.SERVER}${endpoints.promotions}/${id}`,
      dataToSend,
      { headers },
    );
  }
}
