import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class AuditLogService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.auditLogs);
  }

  search(
    keyword?: string,
    isAuthorized?: boolean,
    user?: string,
    sortBy?: string,
    sortDirection?: string,
    page?: number,
    headers?: HttpHeaders,
  ): Observable<any> {
    let newUrl: string = config.SERVER + endpoints.auditLogs;
    const params: string[] = [];

    if (keyword) {
      params.push(`Keyword=${encodeURIComponent(keyword)}`);
    }

    if (isAuthorized !== undefined) {
      params.push(`IsAuthorized=${isAuthorized}`);
    }

    if (user) {
      params.push(`User=${encodeURIComponent(user)}`);
    }

    if (sortBy) {
      params.push(`SortBy=${sortBy}`);
    }

    if (sortDirection) {
      params.push(`SortDirection=${sortDirection}`);
    }

    if (page) {
      params.push(`Page=${page}`);
    }

    if (params.length > 0) {
      newUrl += `?${params.join('&')}`;
    }

    return this.http.get(newUrl, { headers });
  }
}
