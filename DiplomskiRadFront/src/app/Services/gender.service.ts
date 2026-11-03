import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IGender } from '../Interfaces/i-gender';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class GenderService {
  constructor(private http: HttpClient) {}

  getGender(): Observable<IGender[]> {
    return this.http.get<IGender[]>(`${config.SERVER}${endpoints.genders}`);
  }
}
