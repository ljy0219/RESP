import { Component, OnInit, Input, Output, ViewChild, ElementRef } from '@angular/core';

import { FileUploadService } from '../services/file-upload.service';
import { saveAs } from 'file-saver';
import { Subscription } from 'rxjs';
import { ResponseObject } from '../models/responseObject';
import { ImportResult } from '../models/ImportResult';
//import { mapChildrenIntoArray } from '@angular/router/src/url_tree';
import { EmitterVisitorContext } from '@angular/compiler';
import { HrisContactService } from '../services/hris-contact.service';

@Component({
  selector: 'app-uploads',
  templateUrl: './uploads.component.html',
  styleUrls: ['./uploads.component.css']
})
export class UploadsComponent implements OnInit {

  files: FileList;
  subscribeFlag: Subscription;
  importProgress = 0;
  loadingFlag = false;
  alertMsg: string;
  importBtn: HTMLElement;
  importModal:HTMLElement;
  @Input() uploadType: string;
  @ViewChild('closeModalBtn') closeModalBtn;
  @ViewChild('fileControl') fileControl:ElementRef;

  constructor(private fileUploadService: FileUploadService, private contactService: HrisContactService) { }

  ngOnInit() {
    this.importBtn = document.getElementById("importBtn") as HTMLElement;
    this.importModal = document.getElementById("importModal") as HTMLElement;
  }

  OnFileSelected(files: FileList) {
    this.files = files;
  }

  OnImport() {
    this.alertMsg = "";
    if (this.files) {
      var fileToUpload = this.files.item(0);
      let formData = new FormData();
      if (!fileToUpload.name.match(/(\.xlsx|\.xls)$/)) {
        this.alertMsg = "The type of the file you chose is invalid";
        return;
      }
      if (fileToUpload.size > 5 * 1024 * 1024) {
        this.alertMsg = "The file should be no more than 5M";
        return;
      }

      // let interValId = setInterval(() => { this.updateProgress(1); }, 15);
      formData.append('RespFile', fileToUpload, fileToUpload.name);
      console.log("import Start: " + (new Date()).toString());
      console.log("fileSize: " + fileToUpload.size);
      this.loadingFlag = true;
      this.importBtn.setAttribute("disabled", "true");
      if (this.uploadType == "Responsibility") {
        this.uploadResponsibilities(formData);
      } else if (this.uploadType = "Contact") {
        this.uploadHRISContact(formData);
      }
      this.importProgress = 0;
    } else {
      this.alertMsg = "Please choose a file first";
    }
  }

  onCancel() {
    this.fileUploadService.CancelImport().subscribe((result) => {
      console.log(result);
    });
    this.subscribeFlag.unsubscribe();
  }


  updateProgress(value: number) {
    if (this.importProgress <= 95 && this.importProgress >= 0) {
      this.importProgress += value;
    }
  }

  uploadResponsibilities(formData: FormData, ) {
    this.subscribeFlag = this.fileUploadService.SendFile(formData).subscribe((result: ResponseObject) => {
      this.loadingFlag = false;
      this.importBtn.removeAttribute("disabled");
      console.log("import End: " + (new Date()).toString());
      console.log("importProgress: " + this.importProgress);

      this.importProgress = 100;
      // clearInterval(interValId);
      if (result.IsSuccess) {

        if (result.Content) {
          let impt: ImportResult = result.Content;
          this.alertMsg = impt.TotalCount + " items were imported to the system<br/> " + impt.ImportedCount + " items were updated successfully<br/> " + impt.FailedCount + " items were failed for validation";
          if (impt.FailedCount > 0) {
            this.getFile(impt.FailedPath,"Exceptional Responsibilities.xlsx");
            this.alertMsg += "<br/>Validation failed items have been auto downloaded, please reference the error message in the last column, you can upload them again after fixing all the problem(s). If no file download, please contact <a href='mailto:marina.meng@emerson.com'>GPS Support</a>";
            // var file = new Blob([result], { type: 'application/vnd.ms-excel' });
            //   var fileURL = URL.createObjectURL(file);
            //   let a = document.createElement("a");
            //   document.body.appendChild(a);
            //   a.style.display = "none";
            //   a.href = fileURL;
            //   a.target = "_blank";
            //   a.download = "Exceptional Responsibilities.xlsx";
            //   a.click();
            //   a.remove();
          }
        }
      } else {
        this.alertMsg = "Error occurred.";
      }
    },
      err => {
        this.importBtn.removeAttribute("disabled");
        this.loadingFlag = false;
        if (err.status == 409) //conflict
        {
          this.alertMsg = "The system is busy now, please try again later.";
        } else {
          this.alertMsg = "Error occurred.";
        }
        console.log("error status:" + err.status);
      });

  }


  uploadHRISContact(formData: FormData) {
    this.contactService.ImportFromExcel(formData).subscribe((result: ResponseObject) => {
      this.loadingFlag = false;
      this.importBtn.removeAttribute("disabled");
      console.log("import End: " + (new Date()).toString());
      if (result.IsSuccess) {
        if (result.Content) {
          let impt: ImportResult = result.Content;

          this.alertMsg = impt.TotalCount + " items were imported to the system<br/> " + impt.ImportedCount + " items were updated successfully<br/> " + impt.FailedCount + " items were failed for validation";
          if (impt.FailedCount > 0) {
            this.getFile(impt.FailedPath,"Exceptional HRIS Contacts.xlsx");
            this.alertMsg += "<br/>Validation failed items have been auto downloaded, please reference the error message in the last column, you can upload them again after fixing all the problem(s). If no file download, please contact <a href='mailto:marina.meng@emerson.com'>GPS Support</a>";
          }
        }
      } else if (!result.IsSuccess) {
        // this.alertMsg = "Import terminated successfully without warnings, you can refresh the page to see the update.";
        this.alertMsg = result.ErrorMsg;
      }
    },
      err => {
        this.importBtn.removeAttribute("disabled");
        this.loadingFlag = false;
        if (err.status == 409) //conflict
        {
          this.alertMsg = "The system is busy now, please try again later.";
        } else {
          this.alertMsg = "Error occurred.";
        }
        console.log("error status:" + err.status);
      });
  }


  getFile(path: string, fileName:string) {
    this.fileUploadService.GetFile(path).subscribe((result: Blob) => {
      if (result.size > 0) {
        saveAs(result, fileName);
      }
    });
  }


  onCloseModal()
  {
    if(!this.loadingFlag)
    {
      this.importModal.style.display="none";
      this.fileControl.nativeElement.value="";
      this.alertMsg="";
      this.loadingFlag=false;
    }
  }

}
