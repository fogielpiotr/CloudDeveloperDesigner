import { DeploymentService } from './services/deployment-service';
import { EventService } from './services/event-service';
import { ConfigurationService } from './services/configuration-service';
import { ProjectService } from './services/project.service';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ConfirmationService, MessageService } from 'primeng/api';
import { AppHttpInterceptor } from './interceptors/app-http-interceptor';
import { ResourceService } from './services/resource-service';
import { ApplicationService } from './services/application-service';
import { DiagramManager } from './fabric/diagram.manager';

@NgModule({
  imports: [
    HttpClientModule
  ],
  providers: [
    ProjectService,
    ConfirmationService,
    ConfigurationService,
    MessageService,
    ResourceService,
    ApplicationService,
    DiagramManager, 
    EventService,
    DeploymentService,
    { provide: HTTP_INTERCEPTORS, useClass: AppHttpInterceptor, multi: true }]
})
export class CoreModule { }
