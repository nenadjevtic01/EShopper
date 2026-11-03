import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class UserService extends ApiService {
  avatarChanged = new Subject<string | null>();

  constructor(http: HttpClient) {
    super(http, endpoints.user);
  }

  adminCreate(dataToSend: any, headers?: HttpHeaders): Observable<any> {
    return this.http.post(
      `${config.SERVER}${endpoints.user}/admin-create`,
      dataToSend,
      { headers },
    );
  }

  uploadAvatar(file: File, headers?: HttpHeaders): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(
      `${config.SERVER}${endpoints.user}/upload-avatar`,
      formData,
      { headers },
    );
  }

  updateAvatar(src: string, headers?: HttpHeaders): Observable<any> {
    return this.http.put(
      `${config.SERVER}${endpoints.user}/avatar`,
      { Src: src },
      { headers },
    );
  }
}
