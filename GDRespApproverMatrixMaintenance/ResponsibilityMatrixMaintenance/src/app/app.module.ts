import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { UploadsComponent } from './uploads/uploads.component';
import { ReactiveFormsModule, FormsModule} from '@angular/forms';
import { HttpClientModule} from '@angular/common/http';
import { AppRoutingModule } from './/app-routing.module';
import { ResponsibilitiesComponent } from './resp/responsibilities/responsibilities.component';
import { NgxPaginationModule} from 'ngx-pagination';
import { EditResponsibilityComponent } from './resp/edit-responsibility/edit-responsibility.component';
import { ConfirmationComponent } from './confirmation/confirmation.component';
import { HrisContactComponent } from './hris-contact/hris-contact.component';
import { EditContactComponent } from './hris-contact/edit-contact/edit-contact.component';
import { LoginComponent } from './login/login.component';
import { NoaccessComponent } from './noaccess/noaccess.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';


@NgModule({
  declarations: [
    AppComponent,
    UploadsComponent,
    ResponsibilitiesComponent,
    EditResponsibilityComponent,
    ConfirmationComponent,
    HrisContactComponent,
    EditContactComponent,
    LoginComponent,
    NoaccessComponent
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    NgxPaginationModule,
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
