import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
  })

export class CanActivate implements CanActivate{



    canActivate( ) {
        debugger;
        //return true;   
        if(sessionStorage.getItem("NTUserName"))
        {
            return true;
        }else{
            window.location.replace('login');
            return false;
        }

      }
}
