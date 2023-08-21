import { Component, OnInit } from '@angular/core';
import { Application } from 'src/app/modules/diagram/shared/application.model';
import { ApplicationDragObject, DATA_TRANSFER_INFO, ResourceDragObject } from 'src/app/modules/diagram/shared/diagram-drag-object.model';
import { DiagramObjectType } from 'src/app/modules/core/fabric/diagram-object-type-enum';
import { Resource } from 'src/app/modules/diagram/shared/resource.model';
import { ApplicationService } from 'src/app/modules/core/services/application-service';
import { ResourceService } from 'src/app/modules/core/services/resource-service';

@Component({
  selector: 'app-diagram-panel',
  templateUrl: './diagram-panel.component.html',
  styleUrls: ['./diagram-panel.component.scss']
})
export class DiagramPanelComponent implements OnInit {
  applications: Application[];
  resources: Resource[];

  constructor(private resourceService: ResourceService, private applicationService: ApplicationService) { }

  ngOnInit(): void {
    this.resourceService.getResources().subscribe((resources: Resource[])=> {
      this.resources = this.modifyImageSrc(resources) as Resource[];
    });
    this.applicationService.getApplication().subscribe((applications: Application[]) => {
       this.applications = this.modifyImageSrc(applications) as Application[];
    });
  }

  appStartDrag(e: DragEvent, application: Application) {
    const diagramObject = {
      type: DiagramObjectType.Application,
      data: application
    } as ApplicationDragObject;

    e.dataTransfer.setData(DATA_TRANSFER_INFO, JSON.stringify(diagramObject));
  }

  resourceStartDrag(e: DragEvent, resource: Resource) {
    const diagramObject = {
      type: DiagramObjectType.Resource,
      data: resource
    } as ResourceDragObject;

    e.dataTransfer.setData(DATA_TRANSFER_INFO, JSON.stringify(diagramObject));
  }

  modifyImageSrc(items: Application[] | Resource[]): Application[] | Resource[] {
    items.forEach(element => {
       element.iconFile =  'assets/svg/' + element.iconFile;
    });

    return items;
  }
}
