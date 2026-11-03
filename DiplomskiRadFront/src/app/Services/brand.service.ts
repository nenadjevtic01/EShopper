import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { IBrand } from '../Interfaces/i-brand';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class BrandService {
  private path: string = endpoints.brands;

  constructor(private http: HttpClient) {}

  private static toBrand(dto: any): IBrand {
    return {
      id: dto.id,
      name: dto.name,
      img: { src: dto.picture?.src || '', alt: dto.picture?.alt || dto.name },
      isActive: dto.isActive,
    };
  }

  getBrands(
    includeInactive?: boolean,
    headers?: HttpHeaders,
  ): Observable<IBrand[]> {
    let url = `${config.SERVER}${this.path}?PerPage=1000`;

    if (includeInactive) {
      url += `&IncludeInactive=true`;
    }

    return this.http
      .get<any>(url, { headers })
      .pipe(
        map((response) =>
          (response.data || []).map((dto: any) => BrandService.toBrand(dto)),
        ),
      );
  }

  create(dataToSend: any, headers?: HttpHeaders): Observable<any> {
    return this.http.post(`${config.SERVER}${this.path}`, dataToSend, {
      headers,
    });
  }

  update(dataToSend: any, headers?: HttpHeaders): Observable<any> {
    return this.http.put(`${config.SERVER}${this.path}`, dataToSend, {
      headers,
    });
  }

  delete(id: number, headers?: HttpHeaders): Observable<any> {
    return this.http.delete(`${config.SERVER}${this.path}/${id}`, { headers });
  }

  uploadImage(file: File, headers?: HttpHeaders): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post(
      `${config.SERVER}${this.path}/upload-image`,
      formData,
      { headers },
    );
  }
}
