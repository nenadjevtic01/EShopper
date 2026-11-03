import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ISize } from '../Interfaces/i-size';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class SizeService {
  constructor(private http: HttpClient) {}

  getSizes(): Observable<ISize[]> {
    return this.http.get<ISize[]>(`${config.SERVER}${endpoints.sizes}`);
  }
}
