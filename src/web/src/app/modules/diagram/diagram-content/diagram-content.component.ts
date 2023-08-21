import { MessageService } from 'primeng/api';
import { CodeDeployment, ParamValue } from './../shared/deployment-command.model';
import { Configuration } from './../shared/configuration.model';
import { EventService } from './../../core/services/event-service';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Guid } from 'guid-typescript';
import { DATA_TRANSFER_INFO, DiagramDragObject } from 'src/app/modules/diagram/shared/diagram-drag-object.model';
import { DiagramObjectType } from 'src/app/modules/core/fabric/diagram-object-type-enum';
import { ApplicationDragObject, ResourceDragObject } from "../shared/diagram-drag-object.model";
import { DiagramManager } from '../../core/fabric/diagram.manager';
import { Project } from '../../project/project.model';
import { UnsubscribeOnDestroy } from '../../core/helpers/unsubscribe-on-destroy';
import { combineLatest, takeUntil } from 'rxjs';
import { DiagramSize } from '../shared/diagram-size';
import { AppDiagramObject, ResourceDiagramObject } from '../shared/diagram-object.model';
import { NamingHelper } from '../../core/helpers/resource-naming-helper';

@Component({
  selector: 'app-diagram-content',
  templateUrl: './diagram-content.component.html',
  styleUrls: ['./diagram-content.component.scss']
})
export class DiagramContentComponent extends UnsubscribeOnDestroy implements OnInit {

  project: Project;
  configuration: Configuration
  diagramSize: DiagramSize;

  constructor(public diagram: DiagramManager, private cd: ChangeDetectorRef, private eventService: EventService, private messageService: MessageService) { super(); }


  ngOnInit(): void {
    combineLatest([
      this.eventService.ProjectSelectedEvent.pipe(takeUntil(this.destroy$)),
      this.eventService.DiagramSizeCalculated.pipe(takeUntil(this.destroy$)),
      this.eventService.ConfigurationLoaded.pipe(takeUntil(this.destroy$))]
    ).subscribe(([project, diagramSize, configuration]) => {
      this.configuration = configuration;
      this.project = project;
      this.diagramSize = diagramSize;
      this.cd.detectChanges();
      this.diagram.initializeDiagram('diagram', this.diagramSize.width, this.diagramSize.height);
      if (this.project.diagram) {
        this.diagram.loadDiagramFromJson(this.project.diagram);
      }
    });
  }

  handleDrop(e: any) {
    e.preventDefault();
    const dragObject = JSON.parse(e.dataTransfer.getData(DATA_TRANSFER_INFO)) as DiagramDragObject;
    const nodeId = Guid.create().toString();
    switch (dragObject.type) {
      case DiagramObjectType.Resource:
        const resourceDiagramObject = this.createResourceDiagrmObject(dragObject, nodeId);
        if (resourceDiagramObject) {
          this.diagram.addNode(e, resourceDiagramObject, NamingHelper.encodeResourceName(resourceDiagramObject.resourceNaming, resourceDiagramObject.name, ''));
        }

        break;
      case DiagramObjectType.Application:
        const appDiagramObject = this.createAppDiagramObject(dragObject, nodeId);
        if (appDiagramObject) {
          this.diagram.addNode(e, appDiagramObject, NamingHelper.encodeAppName(appDiagramObject.name, ''));
        }
        break;
      default:
        break;
    }
    return false;
  }

  private createAppDiagramObject(dragObject: DiagramDragObject, nodeId: string) {
    const application = dragObject as ApplicationDragObject;
    let name = application.data.name.split(' ').join('');
    const existingCount = this.diagram.getAppNodes().filter(x => x.backendName === application.data.name).length;
    if (existingCount > 0) {
      name += existingCount;
    };
    const appDiagramObject = {
      displayName: application.data.displayName,
      iconFile: application.data.iconFile,
      codeDeployment: {
        appId: application.data.id,
        repositoryName: name,
        settingsJson: null
      } as CodeDeployment,
      id: nodeId,
      authorizedApps: [],
      connectedResources: [],
      name: name,
      applicationType: application.data.type,
      redirectUris: {},
      type: DiagramObjectType.Application,
      backendName: application.data.name,
      valid: true,
      environments: this.configuration.availableEnvironments,
      clientSecretName : '',
      clientIdSecretName: ''
    } as AppDiagramObject;
    return appDiagramObject;
  }

  private createResourceDiagrmObject(dragObject: DiagramDragObject, nodeId: string) {
    const dragData = dragObject as ResourceDragObject;
    if (dragData.data.isSecretProvider && this.diagram.getResourceNodes().find(x => x.isSecretProvider)) {
      this.messageService.add({ severity: 'error', summary: "Deployment can contain olny one Secret provider." });

      return null;
    }
    const parameters: Record<string, ParamValue> = {}
    Object.keys(dragData.data.template.parameters).map(x => {
      parameters[x] = { value: '' }
    });
    let name = this.project.name.split(' ').join('');
    const existingCount = this.diagram.getResourceNodes().filter(x => x.backendName === dragData.data.name).length;
    if (existingCount > 0) {
      name = name + existingCount;
    }

    const resourceDiagramObject = {
      name,
      displayName: dragData.data.displayName,
      id: nodeId,
      iconFile: dragData.data.iconFile,
      resourceNaming: dragData.data.resourceNaming,
      isSecretProvider: dragData.data.isSecretProvider,
      type: DiagramObjectType.Resource,
      parameters: parameters,
      secretName: '',
      valid: dragData.data.isSecretProvider ?
        Object.keys(parameters).length > 1 ? false : true
        : false,
      template: dragData.data.template,
      backendName: dragData.data.name,
      resourceId: dragData.data.id
    } as ResourceDiagramObject;

    return resourceDiagramObject;
  }
}
