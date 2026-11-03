import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class NewsletterService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.newsletter);
  }

  subscribe(email: string): Observable<any> {
    return this.http.post(config.SERVER + endpoints.newsletter, {
      Email: email,
    });
  }

  search(
    keyword?: string,
    includeInactive?: boolean,
    headers?: HttpHeaders,
  ): Observable<any> {
    let newUrl: string = config.SERVER + endpoints.newsletter;
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
}
