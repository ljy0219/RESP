import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ResponsibilityApproverMap } from '../models/responsibilityApproverMap'
import { Config } from '../config'
import { ResponseObject } from '../models/ResponseObject'

import { RespApproverMatrixQuery } from '../models/respApproverMatrixQuery'

@Injectable({
  providedIn: 'root'
})
export class ResponsibilityApproverService {

  constructor(private httpClient: HttpClient, private config: Config) { }

  getResponsibilityApproverMatrix(query: RespApproverMatrixQuery) {
    let headers: HttpHeaders = new HttpHeaders();
    headers = headers.append('Accept', 'application/json');
    headers = headers.append('Access-Control-Allow-Origi', '*');
    return this.httpClient.post(`${this.config.API_URL}/api/v1/Resp/GetRespApproverMatrix`, query, { headers: headers });
  }

  getSingleRespApproverMap(id: number) {
    return this.httpClient.get(`${this.config.API_URL}/api/v1/Resp/GetSingleRespApproverMap?id=${id}`);
  }

  updateSingleRespApproverMap(map: ResponsibilityApproverMap) {
    return this.httpClient.post(`${this.config.API_URL}/api/v1/Resp/UpdateRespApproverMap`, map);
  }

  checkIfApproverExists(p_mail: string, s_mail: string, f_mail: string) {
    return this.httpClient.get(`${this.config.API_URL}/api/v1/Resp/CheckIfUserExistsInGD?p_email=${p_mail}&s_email=${s_mail}&f_email=${f_mail}`);
  }

  deleteRespApproverMap(id: number) {
    return this.httpClient.post(`${this.config.API_URL}/api/v1/Resp/DeleteRespApproverMap?id=${id}`,'');
  }

  exportToExcel(query: RespApproverMatrixQuery) {
    return this.httpClient.post<Blob>(`${this.config.API_URL}/api/v1/Resp/ExportToExcel`, query, { responseType: 'blob' as 'json' });
  }

}
