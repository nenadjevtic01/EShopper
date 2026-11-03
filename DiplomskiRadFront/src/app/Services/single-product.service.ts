import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpClient } from '@angular/common/http';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class SingleProductService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.products);
  }
}
