import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { UploadsComponent } from './uploads/uploads.component';
import { ResponsibilitiesComponent } from './resp/responsibilities/responsibilities.component';
import { EditResponsibilityComponent } from './resp/edit-responsibility/edit-responsibility.component';
import { HrisContactComponent } from './hris-contact/hris-contact.component';
import { EditContactComponent } from './hris-contact/edit-contact/edit-contact.component';
import { CanActivate } from './canActivate';
import { LoginComponent } from './login/login.component';
import { NoaccessComponent } from './noaccess/noaccess.component';

const routes: Routes = [
  // {
  //   path: 'ResponsibilityApproverMatrix', children: [
      { path: '', component: LoginComponent },
      { path: 'responsibilities', component: ResponsibilitiesComponent, canActivate: [CanActivate] },
      { path: 'editresp/:id', component: EditResponsibilityComponent, canActivate: [CanActivate] },
      { path: 'contacts', component: HrisContactComponent, canActivate: [CanActivate] },
      { path: 'editcontact/:id', component: EditContactComponent, canActivate: [CanActivate] },
      { path: 'login', component: LoginComponent },
      { path: 'NoAccess', component: NoaccessComponent},
  //   ]
  // },
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
