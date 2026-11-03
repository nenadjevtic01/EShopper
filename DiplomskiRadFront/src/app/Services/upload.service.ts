import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { config } from '../Constants/config';
import { endpoints } from '../Constants/endpoints';

@Injectable({
  providedIn: 'root',
})
export class UploadService {
  private path: string = endpoints.uploads;

  constructor(private http: HttpClient) {}

  discard(paths: string[], headers?: HttpHeaders): void {
    if (paths.length === 0) {
      return;
    }

    this.http
      .post(`${config.SERVER}${this.path}/discard`, { paths }, { headers })
      .subscribe({ error: () => {} });
  }
}
