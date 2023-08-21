import { SharedModule } from './../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DiagramComponent } from './diagram/diagram.component';
import { DiagramContentComponent } from './diagram-content/diagram-content.component';
import { DiagramPanelComponent } from './diagram-panel/diagram-panel.component';
import { DiagramInfoComponent } from './diagram-info/diagram-info.component';
import { ResourceNodeInfoComponent } from './diagram-info/resource-node-info/resource-node-info.component';
import { AppNodeInfoComponent } from './diagram-info/app-node-info/app-node-info.component';
import { EditsettingsComponent } from './diagram-info/app-node-info/editsettings/editsettings.component';
import { DeploymentComponent } from './diagram/deployment/deployment.component';
import { PreviousdeploymentComponent } from './diagram/deployment/previousdeployment/previousdeployment.component';
import { DeploymentdetailsComponent } from './diagram/deployment/previousdeployment/deploymentdetails/deploymentdetails.component';

@NgModule({
  declarations: [
    DiagramComponent,
    DiagramContentComponent,
    DiagramPanelComponent,
    DiagramInfoComponent,
    ResourceNodeInfoComponent,
    AppNodeInfoComponent,
    EditsettingsComponent,
    DeploymentComponent,
    PreviousdeploymentComponent,
    DeploymentdetailsComponent
  ],
  imports: [
    CommonModule,
    SharedModule
  ]
})
export class DiagramModule { }
