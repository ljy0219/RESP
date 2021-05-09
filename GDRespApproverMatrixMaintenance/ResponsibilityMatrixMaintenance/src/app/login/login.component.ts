import { Component, OnInit } from '@angular/core';
import { LoginService } from '../services/login.service';
import { Config } from '../config';
import { ResponseObject } from '../models/responseObject';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  UserName:string="Logging in";
  constructor(private loginService: LoginService, private config: Config) { }

  ngOnInit() {
    this.Login();
  }

  Login() {
    this.loginService.login().subscribe((result: ResponseObject)=>{
      if(result.IsSuccess)
      {
        debugger;
        this.UserName=result.Content;
        sessionStorage.setItem("NTUserName",this.UserName);
        sessionStorage.setItem("UserName", this.UserName.substring(this.UserName.lastIndexOf('\\')+1));
        window.location.replace("responsibilities");
      }else{
        window.location.replace("NoAccess");

        console.log();
      }

    });
  }



}
