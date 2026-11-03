import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiService } from './api.service';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class LoginService extends ApiService {
  constructor(http: HttpClient) {
    super(http, endpoints.login);
  }
}
