import { Component, OnInit } from '@angular/core';
import { Route, ActivatedRoute, Params } from '@angular/router';
import { ResponseObject } from '../../models/responseObject'
import { HRISContact } from 'src/app/models/HRISContact';
import { HrisContactService } from '../../services/hris-contact.service';

@Component({
  selector: 'app-edit-contact',
  templateUrl: './edit-contact.component.html',
  styleUrls: ['./edit-contact.component.css']
})
export class EditContactComponent implements OnInit {

  contact: HRISContact = new HRISContact();
  id: number = 0;
  errorMsg: string = "";
  successMsg: string = "";
  constructor(private activatedRoute: ActivatedRoute, private contactService: HrisContactService) { }

  ngOnInit() {
    this.id = Number.parseInt(this.activatedRoute.snapshot.paramMap.get('id'));
    this.getHRISContact();
  }


  getHRISContact() {
    debugger;
    if (this.id > 0) {
      this.contactService.getSingleHRISContact(this.id).subscribe((result: ResponseObject) => {
        if (result.IsSuccess) {
          this.contact = result.Content;
        }
      });
    }
  }

  updateSingleHRISContact() {
    this.errorMsg = "";
    this.successMsg = "";
    let requiredMsg = "";
    let numberFormatMsg = "";
    if (!this.contact.RespID) {
      requiredMsg += "RespID";
    }
    if (!this.contact.Responsibility_Name) {
      requiredMsg += requiredMsg.length > 0 ? ", " : "";
      requiredMsg += "Responsibility Name";
    }

    if (!this.contact.Contact1 || !this.contact.Contact1.trim()) {
      requiredMsg += requiredMsg.length > 0 ? ", " : "";
      requiredMsg += "Contact1";
    }

    this.errorMsg += requiredMsg.length > 0 ? "<li>Required field(s): " + requiredMsg + "</li>" : "";

    const reg = new RegExp(/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/);

    if (this.contact.Contact1 && this.contact.Contact1.trim()) {
      let emailArr =this.contact.Contact1.split(";");
      emailArr.forEach(el => {
        if(el && el.trim() && !reg.test(el.trim()))
        {
          this.errorMsg += "<li>Contact1 should be email address(es)</li>";
        }
      });
    }
    if (this.contact.Contact2 && this.contact.Contact2.trim()) {
      let emailArr=this.contact.Contact2.split(";");
      emailArr.forEach(el=>{
        if( el && el.trim() && !reg.test(el.trim()) )
        {
          this.errorMsg += "<li>Contact2 should be email address(es)</li>";
        }
      });
    }

    let reg1 = new RegExp(/^[0-9]*$/);

    if (this.contact.RespID && !reg1.test(this.contact.RespID.toString().trim())) {
      this.errorMsg += "<li>RespID should be a number</li>";
    }
    if (this.contact.OU_Org_ID && !reg1.test(this.contact.OU_Org_ID.toString().trim())) {
      this.errorMsg += "<li>OU Org ID should be a number</li>";
    }

    if (this.contact.BG_ID && !reg1.test(this.contact.BG_ID.toString().trim())) {
      this.errorMsg += "<li>BG ID should be a number</li>";
    }

    if (this.errorMsg) {
      this.errorMsg = "<ul>" + this.errorMsg + "</ul>";
      return;
    } else {
      if(!this.contact.Contact2)
      {
        this.contact.Contact2="";
      }

      this.contactService.checkHRISContact(this.contact.Contact1,this.contact.Contact2).subscribe( (result:ResponseObject)=>{
        if(result.IsSuccess)
        {
          result.Content.forEach(e=>{
            if(!e.Value)
            {
              this.errorMsg += "<li>" + e.Key + " is invalid in Global Directory</li>";
            }
          });
        }else{
          this.errorMsg=result.ErrorMsg;
        }

        if(this.errorMsg)
        {
          return ;
        }else{
          this.updateHRISContact();
        }
      },
      err=>{
        this.errorMsg="<li>Error occurred when checking Contact(s)</li>";
      });

    }


  }


  private updateHRISContact() {
    debugger;
    this.contactService.updateHRISContact(this.contact).subscribe((result: ResponseObject) => {
      if (result.IsSuccess) {
        this.successMsg = "Your submit has been done successfully"; 
        this.id = this.contact.ID;
        this.contact = result.Content;
      }
      else {
        this.errorMsg = result.ErrorMsg;
      }
    });
  }

  onDelConfirm(confirmation:boolean)
  {
    if(confirmation)
    {
      this.contactService.deleteHRISContact(this.id).subscribe((result:ResponseObject)=>{
        if(result.IsSuccess)
        {
          this.successMsg="The item has been deleted successfully";
          setTimeout(()=>{
            window.location.replace('contacts');
          },3000);

        }
      });
    }
  }
}
