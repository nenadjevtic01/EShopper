import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { config } from '../Constants/config';

@Injectable({
  providedIn: 'root',
})
export class PasswordResetService {
  constructor(private http: HttpClient) {}

  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${config.SERVER}/User/forgot-password`, {
      Email: email,
    });
  }

  resetPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(`${config.SERVER}/User/reset-password`, {
      Token: token,
      NewPassword: newPassword,
    });
  }
}
