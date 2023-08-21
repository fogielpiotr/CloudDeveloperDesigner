import { NgModule } from '@angular/core';
import { ProjectOverviewComponent } from './project-overview/project-overview.component';
import { ProjectNewComponent } from './project-new/project-new.component';
import { SharedModule } from '../shared/shared.module';
import { ProjectEditComponent } from './project-edit/project-edit.component';

@NgModule({
  declarations: [
    ProjectOverviewComponent,
    ProjectNewComponent,
    ProjectEditComponent
  ],
  imports: [
    SharedModule
  ]
})
export class ProjectModule { }
