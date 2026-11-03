import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ICategory } from '../Interfaces/i-category';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private path: string = endpoints.categories;

  constructor(private http: HttpClient) {}

  private static toCategory(dto: any): ICategory {
    return {
      id: dto.id,
      name: dto.name,
      img: { src: dto.picture?.src || '', alt: dto.picture?.alt || dto.name },
      isActive: dto.isActive,
    };
  }

  getCategory(
    includeInactive?: boolean,
    headers?: HttpHeaders,
  ): Observable<ICategory[]> {
    let url = `${config.SERVER}${this.path}?PerPage=1000`;

    if (includeInactive) {
      url += `&IncludeInactive=true`;
    }

    return this.http
      .get<any>(url, { headers })
      .pipe(
        map((response) =>
          (response.data || []).map((dto: any) =>
            CategoryService.toCategory(dto),
          ),
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

  uploadImage(
    file: File,
    name: string,
    headers?: HttpHeaders,
    categoryId?: number,
  ): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    let query = `name=${encodeURIComponent(name || '')}`;

    if (categoryId) {
      query += `&categoryId=${categoryId}`;
    }

    return this.http.post(
      `${config.SERVER}${this.path}/upload-image?${query}`,
      formData,
      { headers },
    );
  }
}
