import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IHeroSlide } from '../Interfaces/i-hero-slide';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class HeroSlideService {
  private path: string = endpoints.heroSlides;

  constructor(private http: HttpClient) {}

  getSlides(
    headers?: HttpHeaders,
    includeInactive?: boolean,
  ): Observable<IHeroSlide[]> {
    let url = `${config.SERVER}${this.path}`;

    if (includeInactive) {
      url += `?includeInactive=true`;
    }

    return this.http.get<IHeroSlide[]>(url, { headers });
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

  upload(
    file: File,
    headers?: HttpHeaders,
  ): Observable<{ path: string; mediaType: 'image' | 'video' }> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{ path: string; mediaType: 'image' | 'video' }>(
      `${config.SERVER}${this.path}/upload`,
      formData,
      { headers },
    );
  }
}
