import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Config } from '../config';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  constructor(private httpClient: HttpClient, private config:Config) {

  }

  SendFile(data: FormData) {
    return this.httpClient.post(`${this.config.API_URL}/api/v1/Resp/ImportFromExcel?userName=` + sessionStorage.getItem("UserName"), data);
  }

  CancelImport()
  {
    return this.httpClient.get(`${this.config.API_URL}/api/v1/Resp/CancelImportFromExcel` );
  }


  GetFile(path:string)
  {
    return this.httpClient.get<Blob>(`${this.config.API_URL}/api/v1/base/GetFile?path=${path}`,{responseType: 'blob' as 'json'});
  }
}
