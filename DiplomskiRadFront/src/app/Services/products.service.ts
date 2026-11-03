import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

export interface IProductSearch {
  keyword?: string;
  categoryId?: number;
  brandId?: number;
  categoryIds?: number[];
  brandIds?: number[];
  genderIds?: number[];
  sizeIds?: number[];
  onlyInStock?: boolean;
  includeInactive?: boolean;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  page?: number;
  perPage?: number;
}

@Injectable({
  providedIn: 'root',
})
export class ProductsService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.products);
  }

  search(filters: IProductSearch, headers?: HttpHeaders): Observable<any> {
    let params = new HttpParams();

    const add = (
      name: string,
      value: string | number | boolean | undefined,
    ) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.append(name, String(value));
      }
    };

    add('Keyword', filters.keyword);
    add('CategoryId', filters.categoryId);
    add('BrandId', filters.brandId);
    (filters.categoryIds || []).forEach((id) => add('CategoryIds', id));
    (filters.brandIds || []).forEach((id) => add('BrandIds', id));
    (filters.genderIds || []).forEach((id) => add('GenderIds', id));
    (filters.sizeIds || []).forEach((id) => add('SizeIds', id));

    if (filters.onlyInStock) {
      add('OnlyInStock', true);
    }

    if (filters.includeInactive) {
      add('IncludeInactive', true);
    }

    add('SortBy', filters.sortBy);
    add('SortDirection', filters.sortDirection);
    add('Page', filters.page);
    add('PerPage', filters.perPage);

    return this.http.get(`${config.SERVER}${endpoints.products}`, {
      params,
      headers,
    });
  }

  uploadImages(
    files: File[],
    category: string,
    headers?: HttpHeaders,
  ): Observable<any> {
    const formData = new FormData();
    files.forEach((file) => formData.append('files', file, file.name));

    return this.http.post(
      `${config.SERVER}${endpoints.products}/upload-images?category=${encodeURIComponent(category)}`,
      formData,
      { headers },
    );
  }

  changePrice(
    id: number,
    price: number,
    headers?: HttpHeaders,
  ): Observable<any> {
    return this.http.post(
      `${config.SERVER}${endpoints.products}/${id}/price`,
      { Price: price },
      { headers },
    );
  }

  getAllForPicker(headers?: HttpHeaders): Observable<any> {
    return this.http.get(`${config.SERVER}${endpoints.products}?PerPage=500`, {
      headers,
    });
  }
}
