import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Config } from '../config';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private httpClient: HttpClient, private config: Config) {

  }

  login()
  {
    return this.httpClient.get(`${this.config.API_URL}/api/v1/base/Login`);
  }
}
