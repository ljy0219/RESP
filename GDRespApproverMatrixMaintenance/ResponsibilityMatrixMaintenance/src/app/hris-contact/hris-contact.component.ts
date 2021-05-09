import { Component, OnInit } from '@angular/core';
import { HRISContact } from 'src/app/models/HRISContact';
import { HRISContactQuery } from 'src/app/models/HRISContactQuery';
import { HrisContactService } from '../services/hris-contact.service';
import { ResponseObject } from '../models/responseObject'
import { saveAs } from 'file-saver';
import { Config } from '../config';


@Component({
  selector: 'app-hris-contact',
  templateUrl: './hris-contact.component.html',
  styleUrls: ['./hris-contact.component.css']
})
export class HrisContactComponent implements OnInit {

  contact_filter: string = "";
  contact_filter_txt: string = "";
  contact_list: HRISContact[];
  contact_query: HRISContactQuery = new HRISContactQuery();
  contact_count: number = 0;
  isLoading: boolean = false;
  exportFlag: boolean = false;
  constructor(private contactService: HrisContactService, private config: Config) { }

  ngOnInit() {
    this.contact_query.Page_Index = 1;
    this.contact_query.Page_Size = this.config.Page_Size;
    this.getContactList();
  }


  getContactList() {
    this.isLoading = true;
    this.contactService.getHRISContact(this.contact_query).subscribe((result: ResponseObject) => {
      this.isLoading = false;
      if (result.IsSuccess) {
        this.contact_list = result.Content.Collection;
        this.contact_count = result.Content.TotalCount;
      }
    });
  }

  onPageIndexChange(index: number) {
    this.contact_query.Page_Index = index;
    this.getContactList()
  }

  onSearch() {

    if ((this.contact_filter && this.contact_filter_txt.trim()) || (!this.contact_filter && !this.contact_filter_txt.trim())) {
      switch (this.contact_filter.toLowerCase()) {
        case "responsibility_name":
          this.contact_query.Responsibility_Name = this.contact_filter_txt.trim();
          break;
        case "org_name":
          this.contact_query.Org_Name = this.contact_filter_txt.trim();
          break;
        case "contact":
          this.contact_query.Contact = this.contact_filter_txt.trim();
          break;

        default:
          this.contact_query.Responsibility_Name = "";
          this.contact_query.Org_Name = "";
          this.contact_query.Contact = "";

          break;
      }
      this.contact_query.Page_Index = 1;
      this.getContactList();
    }
  }

  onEnterKey(e: KeyboardEvent) {
    if (e.keyCode == 13) {
      this.onSearch();
    }
  }

  onExportToExcel() {
    this.exportFlag = true;
    let exportBtn: HTMLElement = document.getElementById("exportBtn") as HTMLElement;
    exportBtn.setAttribute("disabled", "true");
    this.contactService.exportToExcel(this.contact_query.Responsibility_Name, this.contact_query.Org_Name, this.contact_query.Contact).subscribe((result: any) => {
      this.exportFlag = false;
      exportBtn.removeAttribute("disabled");
      if (result.size > 0) {
        saveAs(result, "HRIS Contacts.xlsx");
      }
    })
  }
}
