import { Component, OnInit, Inject, Renderer2 } from '@angular/core';
import { Subscription } from 'rxjs';
import { DOCUMENT } from "@angular/common";
import { ResponsibilityApproverService } from '../../services/responsibility-approver.service';
import { RespApproverMatrixQuery } from '../../models/respApproverMatrixQuery';
import { ResponseObject } from '../../models/ResponseObject';
import { ResponsibilityApproverMap } from '../../models/ResponsibilityApproverMap';
import { DataCollection } from '../../models/DataCollection';
import { UploadsComponent } from '../../uploads/uploads.component';
import { saveAs } from 'file-saver';
import { Config } from 'src/app/config';
//import {viewWrappedDebugError} from '@angular/core/src/view/errors';


@Component({
  selector: 'app-responsibilities',
  templateUrl: './responsibilities.component.html',
  styleUrls: ['./responsibilities.component.css']
})
export class ResponsibilitiesComponent implements OnInit {

  instance: string = "BETSY";
  resp_list: ResponsibilityApproverMap[];
  resp_count: number;
  resp_query = new RespApproverMatrixQuery();
  resp_filter: string = "";
  resp_filter_txt: string = "";
  isLoading: boolean = false;
  exportFlag: boolean = false;


  constructor(private respService: ResponsibilityApproverService, private _renderer2: Renderer2
    , @Inject(DOCUMENT) private _document, private config: Config) { }


  ngOnInit() {
    this.loadJSScript();
    this.resp_query.Instance = this.instance;
    this.resp_query.Page_Index = 1;
    this.resp_query.Page_Size = this.config.Page_Size;
    this.resp_query.GetCountFlag = false;
    this.resp_query.Application = "";
    this.resp_query.Approver = "";
    this.resp_query.Division = "";
    this.resp_query.DoNotUse = "";
    this.resp_query.Responsibility_Name = "";
    this.resp_query.Default = "";

    this.GetRespMatrix(this.resp_query);
  }

  loadJSScript() {
    debugger;
    const _script = this._renderer2.createElement("Script");
    _script.text = `
    $(document ).ready(function() {
        //$("#tblResp").freezeHeader();
        $('#dl_filter').val("");
        $('#input_filter_txt').val("");
    });
      `;

    this._renderer2.appendChild(this._document.body, _script);
  }


  GetRespMatrix(query: RespApproverMatrixQuery) {
    debugger;
    this.isLoading = true;
    this.respService.getResponsibilityApproverMatrix(query).subscribe((obj: ResponseObject) => {
      this.isLoading = false;
      if (obj.IsSuccess) {
        debugger;
        let dataCollection: DataCollection = obj.Content;
        this.resp_list = dataCollection.Collection;
        this.resp_count = dataCollection.TotalCount;
      }
    });
  }

  onInstanceChanged() {
    // this.resp_query.Instance = this.instance;
    debugger;
    this.resp_query.Page_Index = 1;
    this.resp_query.Page_Size = this.config.Page_Size;
    this.resp_query.GetCountFlag = false;
    this.resp_query.Responsibility_Name = "";
    this.resp_query.Approver = "";
    this.resp_query.Division = "";
    this.resp_query.DoNotUse = "";
    this.resp_filter = "";
    this.resp_filter_txt = "";
    this.resp_query.Default = "";
    //this.GetRespMatrix(this.resp_query);
    this.onSearch();
  }

  onSearch() {
    debugger;
    if ((this.resp_filter && this.resp_filter_txt) || (!this.resp_filter && !this.resp_filter_txt.trim())) {
      this.resp_count = 0;
      switch (this.resp_filter) {
        case "Responsibility_Name":
          this.resp_query.Responsibility_Name = this.resp_filter_txt.trim();
          this.resp_query.Approver = "";
          this.resp_query.Division = "";
          this.resp_query.DoNotUse = "";
          this.resp_query.Default = "";
          break;
        case "Approver":
          this.resp_query.Responsibility_Name = "";
          this.resp_query.Approver = this.resp_filter_txt.trim();
          this.resp_query.Division = "";
          this.resp_query.DoNotUse = "";
          this.resp_query.Default = "";
          break;
        case "Division":
          this.resp_query.Responsibility_Name = "";
          this.resp_query.Approver = "";
          this.resp_query.Division = this.resp_filter_txt.trim();
          this.resp_query.DoNotUse = "";
          this.resp_query.Default = "";
          break;
        case "DoNotUse":
          this.resp_query.Responsibility_Name = "";
          this.resp_query.Approver = "";
          this.resp_query.Division = "";
          this.resp_query.DoNotUse = this.resp_filter_txt.trim();
          this.resp_query.Default = "";
          break;
        case "Default":
          this.resp_query.Responsibility_Name = "";
          this.resp_query.Approver = "";
          this.resp_query.Division = "";
          this.resp_query.DoNotUse = "";
          this.resp_query.Default = this.resp_filter_txt.trim();
          break;
        case "":
          this.resp_query.Responsibility_Name = "";
          this.resp_query.Approver = "";
          this.resp_query.Division = "";
          this.resp_query.DoNotUse = "";
          this.resp_query.Default = "";
          break;
        default:
          break;
      }
        this.resp_query.Page_Index = 1;
        this.resp_query.Page_Size = this.config.Page_Size;
      this.GetRespMatrix(this.resp_query);
    }
  }

  onEnterKey(e: KeyboardEvent) {
    if (e.keyCode == 13) {
      this.onSearch();
    }
  }

    onPageIndexChange(p_index: number) {
        debugger;
    this.resp_query.Page_Index = p_index;
    this.resp_query.GetCountFlag = false;
    this.GetRespMatrix(this.resp_query);
  }

    onExportToExcel() {
        debugger;
        let query: RespApproverMatrixQuery = this.resp_query;
        query.Page_Index = 1;
        query.Page_Size = 0;//  set page size as 0 to get all data
        query.GetCountFlag = false;
        this.exportFlag = true;
        let exportBtn: HTMLElement = document.getElementById("exportBtn") as HTMLElement;
        exportBtn.setAttribute("disabled", "true");

        this.respService.exportToExcel(query).subscribe((result: any) => {
            debugger;
            this.exportFlag = false;
            exportBtn.removeAttribute("disabled");
            if (result && result.size) {
              debugger;
                saveAs(result, query.Instance + " Responsibility Approver Matrix.xlsx");
            }
        },
            err => {

            }
        );
    }
}
