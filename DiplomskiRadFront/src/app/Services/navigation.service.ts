import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { INav } from '../Interfaces/i-nav';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  constructor(private http: HttpClient) {}

  getNav(): Observable<INav[]> {
    return this.http.get<INav[]>(`${config.SERVER}${endpoints.navigation}`);
  }
}
