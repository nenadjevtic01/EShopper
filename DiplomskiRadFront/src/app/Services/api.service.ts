import { Inject, Injectable } from '@angular/core';
import { config } from '../Constants/config';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private server = config.SERVER;

  constructor(
    protected http: HttpClient,
    @Inject('path') protected path: string,
  ) {}

  getAll(
    keyword?: string,
    minPrice?: number,
    maxPrice?: number,
    categoryId?: number,
    headers?: HttpHeaders,
    pageNumber?: number,
    includeInactive?: boolean,
    brandId?: number,
  ): Observable<any> {
    let newUrl: string = config.SERVER + this.path;
    let query: string[] = [];

    if (keyword) {
      query.push(`Keyword=${keyword}`);
    }

    if (minPrice) {
      query.push(`MinimumPrice=${minPrice}`);
    }

    if (maxPrice) {
      query.push(`MaximumPrice=${maxPrice}`);
    }

    if (categoryId) {
      query.push(`CategoryId=${categoryId}`);
    }

    if (brandId) {
      query.push(`BrandId=${brandId}`);
    }

    if (pageNumber) {
      query.push(`Page=${pageNumber}`);
    }

    if (includeInactive) {
      query.push(`IncludeInactive=true`);
    }

    if (query.length > 0) {
      newUrl += `?${query.join('&')}`;
    }

    return this.http.get(newUrl, { headers });
  }

  get(id: number | string, headers?: HttpHeaders): Observable<any> {
    return this.http.get(this.server + this.path + '/' + id, { headers });
  }

  create(dataToSend: any, headers?: HttpHeaders): Observable<any> {
    return this.http.post(this.server + this.path, dataToSend, { headers });
  }

  update(dataToSend: any, headers?: HttpHeaders): Observable<any> {
    return this.http.put(config.SERVER + this.path, dataToSend, { headers });
  }

  delete(id: number | string, headers?: HttpHeaders): Observable<any> {
    return this.http.delete(this.server + this.path + '/' + id, { headers });
  }
}
