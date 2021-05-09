import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';


@Injectable({
    providedIn: 'root'
  })
export class Config {

  public API_URL="/ResponsibilityApproverMatrixAPI";
  //public API_URL="http://localhost:8080";
  public Page_Size:number=20;
  //  public API_URL="http://localhost:85";
  // public API_URL="http://localhost:59385";

}
