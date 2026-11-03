import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ICountry } from '../Interfaces/i-country';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class CountryService {
  constructor(private http: HttpClient) {}

  getCountries(headers?: HttpHeaders): Observable<ICountry[]> {
    return this.http.get<ICountry[]>(`${config.SERVER}${endpoints.countries}`, {
      headers,
    });
  }
}
