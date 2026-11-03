import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';

@Injectable({
  providedIn: 'root',
})
export class StatsService {
  constructor(private http: HttpClient) {}

  getStats(headers?: HttpHeaders): Observable<any> {
    return this.http.get(`${config.SERVER}/Stats`, { headers });
  }
}
