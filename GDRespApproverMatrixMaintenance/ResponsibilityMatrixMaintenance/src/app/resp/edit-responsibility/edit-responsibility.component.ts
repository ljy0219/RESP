import { Component, OnInit } from '@angular/core';
import { Route, ActivatedRoute, Params } from '@angular/router';

import { ResponsibilityApproverMap } from '../../models/responsibilityApproverMap';
import { ResponsibilityApproverService } from '../../services/responsibility-approver.service';
import { ResponseObject } from '../../models/responseObject';
// import { setTimeout } from 'timers';

@Component({
  selector: 'app-edit-responsibility',
  templateUrl: './edit-responsibility.component.html',
  styleUrls: ['./edit-responsibility.component.css']
})
export class EditResponsibilityComponent implements OnInit {
  resp_id: number;
  resp: ResponsibilityApproverMap = new ResponsibilityApproverMap();
  errorMsg: string;
  successMsg: string;
  constructor(private respService: ResponsibilityApproverService, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.resp_id = Number.parseInt(this.activatedRoute.snapshot.paramMap.get('id'));

    debugger;
    this.respService.getSingleRespApproverMap(this.resp_id).subscribe((result: ResponseObject) => {
      if (result.IsSuccess && result.Content) {
        debugger;
        this.resp = result.Content;
        this.resp.Instance = this.resp.Instance.toUpperCase();
      }
    });
  }


    updateSingleRespApproverMap() {
        debugger;
    this.errorMsg = "";
    this.successMsg = "";
    let requiredMsg: string = "";
    if (!this.resp.Responsibility_Name || !this.resp.Responsibility_Name.trim()) {
      requiredMsg = "Responsibility Name";
    }

    if (!this.resp.Application || !this.resp.Application.trim()) {
      requiredMsg += requiredMsg ? ", " : "";
      requiredMsg += "Application";
    }
    if (!this.resp.Instance || !this.resp.Instance.trim()) {
      requiredMsg += requiredMsg ? ", " : "";
      requiredMsg += "Instance";
    }

    //if (!this.resp.Ap_Group || !this.resp.Ap_Group.trim()) {
    //  requiredMsg += requiredMsg ? ", " : "";
    //  requiredMsg += "Ap Group";
    //}

    //if (!this.resp.Division || !this.resp.Division.trim()) {
    //  requiredMsg += requiredMsg ? ", " : "";
    //  requiredMsg += "Division";
    //}

        if (this.resp.Default != "Yes") {
            if (!this.resp.Primary_Approver || !this.resp.Primary_Approver.trim()) {
                requiredMsg += requiredMsg ? ", " : "";
                requiredMsg += "Primary Approver";
            }

            if (!this.resp.Secondary_Approver || !this.resp.Secondary_Approver.trim()) {
                requiredMsg += requiredMsg ? ", " : "";
                requiredMsg += "Secondary Approver";
            }

            requiredMsg = requiredMsg ? "<li>Required field(s): " + requiredMsg + "</li>" : "";

            let emailFormatMsg: string = "";
            const reg = new RegExp(/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/);
            if (this.resp.Primary_Approver && this.resp.Primary_Approver.trim()) {
                if (!reg.test(this.resp.Primary_Approver.trim())) {
                    emailFormatMsg += "<li>Invalid Primary Approver email format</li>";
                }
            }

            if (this.resp.Secondary_Approver && this.resp.Secondary_Approver.trim()) {
                if (!reg.test(this.resp.Secondary_Approver.trim())) {
                    emailFormatMsg += "<li>Invalid Secondary Approver email format</li>";
                }
            }

            if (this.resp.Final_Approver && this.resp.Final_Approver.trim()) {
                if (!reg.test(this.resp.Final_Approver.trim())) {
                    emailFormatMsg += "<li>Invalid Final Approver email format</li>";
                }
            }


            let sameAppvMsg: string = "";
            //if (this.resp.Primary_Approver && this.resp.Secondary_Approver) {
            //    if (this.resp.Primary_Approver.trim() && this.resp.Primary_Approver.trim().toLowerCase() == this.resp.Secondary_Approver.trim().toLowerCase()) {
            //        sameAppvMsg += "<li>Primary Approver and Secondary Approver should not be same</li>";
            //    }
            //}

            //if (this.resp.Primary_Approver && this.resp.Final_Approver) {
            //    if (this.resp.Primary_Approver.trim() && this.resp.Primary_Approver.trim().toLowerCase() == this.resp.Final_Approver.trim().toLowerCase()) {
            //        sameAppvMsg += "<li>Primary Approver and Final Approver should not be same</li>";
            //    }
            //}

            //if (this.resp.Secondary_Approver && this.resp.Final_Approver) {
            //    if (this.resp.Secondary_Approver.trim() && this.resp.Secondary_Approver.trim().toLowerCase() == this.resp.Final_Approver.trim().toLowerCase()) {
            //        sameAppvMsg += "<li>Secondary Approver and Final Approver should not be same</li>";
            //    }
            //}

            this.errorMsg = requiredMsg + emailFormatMsg + sameAppvMsg;

            if (this.errorMsg) {
                this.errorMsg = "<ul>" + this.errorMsg + "</ul>";
                return false;
            }

            // let invalidAppvMsg: string = "";
            // this.resp.Final_Approver = this.resp.Final_Approver ? this.resp.Final_Approver : null;
            // this.respService.checkIfApproverExists(this.resp.Primary_Approver, this.resp.Secondary_Approver, (this.resp.Final_Approver ? this.resp.Final_Approver : "")).subscribe((result: ResponseObject) => {
            //     if (result.IsSuccess) {
            //         if (result.Content) {
            //             result.Content.forEach(e => {
            //                 if (!e.Value) {
            //                     invalidAppvMsg += "<li>" + e.Key + " is invalid in Global Directory</li>";
            //                 }
            //             });
            //         }
            //             // this.errorMsg = requiredMsg + emailFormatMsg + sameAppvMsg + invalidAppvMsg;
            //             this.errorMsg = invalidAppvMsg;
            //             if (this.errorMsg) {
            //                 this.errorMsg = "<ul>" + this.errorMsg + "</ul>";
            //                 return false;
            //             }                  
            //     } else {
            //         // error occurred
            //         invalidAppvMsg = "<li>Error occurred when checking approvers</li>";
            //         return false;
            //     }
            // });

        }

        // update or add
        this.respService.updateSingleRespApproverMap(this.resp).subscribe((result: ResponseObject) => {
            if (!result.IsSuccess) {
                this.errorMsg = result.ErrorMsg;
            } else {
                debugger;
                this.successMsg = "Your submit has been done successfully";
                this.resp = result.Content;
            }
        });

  }

  onDelConfirm(confirmation) {
    if (confirmation) {
      // call service to delete
      this.respService.deleteRespApproverMap(this.resp.ID).subscribe((result:ResponseObject)=>{
        if(result.IsSuccess)
        {
          this.successMsg = "The item has been deleted successfully";
          setTimeout(() => {
            window.location.replace("responsibilities")
          }, 3000);
        }
      });
    } else {
      return;
    }
  }

}
