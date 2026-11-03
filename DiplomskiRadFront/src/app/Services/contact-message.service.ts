import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class ContactMessageService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.contactMessages);
  }

  search(
    keyword?: string,
    includeInactive?: boolean,
    isAnswered?: boolean,
    headers?: HttpHeaders,
  ): Observable<any> {
    let newUrl: string = config.SERVER + endpoints.contactMessages;
    const params: string[] = [];

    if (keyword) {
      params.push(`Keyword=${encodeURIComponent(keyword)}`);
    }

    if (includeInactive) {
      params.push(`IncludeInactive=true`);
    }

    if (isAnswered != null) {
      params.push(`IsAnswered=${isAnswered}`);
    }

    if (params.length > 0) {
      newUrl += `?${params.join('&')}`;
    }

    return this.http.get(newUrl, { headers });
  }

  answer(id: number, answer: string, headers?: HttpHeaders): Observable<any> {
    return this.http.put(
      `${config.SERVER}${endpoints.contactMessages}/${id}/answer`,
      { Answer: answer },
      { headers },
    );
  }
}
