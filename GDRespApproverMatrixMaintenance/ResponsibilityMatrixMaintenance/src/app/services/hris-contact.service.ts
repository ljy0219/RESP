import { Injectable } from '@angular/core';
import { HRISContactQuery } from '../models/HRISContactQuery';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Config } from '../config';
import { HRISContact } from '../models/HRISContact';

@Injectable({
  providedIn: 'root'
})
export class HrisContactService {

  constructor(private httpClient: HttpClient, private config: Config) { }

  ImportFromExcel(formdata: any) {
    return this.httpClient.post(`${this.config.API_URL}/api/v1/contact/ImportFromExcel?userName=marina`, formdata); //, { responseType: 'blob' as 'json' }
  }

  getHRISContact(query: any) {
    return this.httpClient.post(`${this.config.API_URL}/api/v1/contact/GetHRISContact`, query);
  }

  exportToExcel(respName: string = "", typeName: string = "",contact:string ="") {
    return this.httpClient.get(`${this.config.API_URL}/api/v1/contact/ExportToExcel?resp=${respName}&org=${typeName}&contact=${contact}`, { responseType: 'blob' as 'json' });
  }

  getSingleHRISContact(id:number)
  {
    return this.httpClient.get(`${this.config.API_URL}/api/v1/contact/GetSingleHRISContact?id=${id}`);
  }


  updateHRISContact(contact:HRISContact)
  {
    return this.httpClient.post(`${this.config.API_URL}/api/v1/contact/UpdateHRISContact`,contact);
  }

  deleteHRISContact(id:number)
  {
   return this.httpClient.delete(`${this.config.API_URL}/api/v1/contact/DeleteHRISContact?id=${id}`);
  }

  checkHRISContact(contact1:string, contact2:string)
  {
    return this.httpClient.get(`${this.config.API_URL}/api/v1/contact/CheckHRISContact?c1=${contact1}&c2=${contact2}`);
  }

}

