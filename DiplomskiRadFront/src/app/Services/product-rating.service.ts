import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class ProductRatingService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.productRatings);
  }

  rate(
    productId: number,
    rating: number,
    comment: string,
    headers?: HttpHeaders,
  ): Observable<any> {
    return this.http.post(
      config.SERVER + endpoints.productRatings,
      {
        ProductId: productId,
        Rating: rating,
        Comment: comment,
      },
      { headers },
    );
  }
}
