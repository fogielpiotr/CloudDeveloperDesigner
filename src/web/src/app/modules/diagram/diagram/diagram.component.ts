import { DiagramObject, AppDiagramObject } from './../shared/diagram-object.model';
import { UnsubscribeOnDestroy } from './../../core/helpers/unsubscribe-on-destroy';
import { takeUntil } from 'rxjs';
import { EventService } from './../../core/services/event-service';
import { MessageService } from 'primeng/api';
import { DiagramManager } from './../../core/fabric/diagram.manager';
import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Project } from 'src/app/modules/project/project.model';
import { ProjectService } from 'src/app/modules/core/services/project.service';
import { ConfigurationService } from '../../core/services/configuration-service';
import { Configuration } from '../shared/configuration.model';
import { DiagramSize } from '../shared/diagram-size';

@Component({
  selector: 'app-diagram',
  templateUrl: './diagram.component.html',
  styleUrls: ['./diagram.component.scss']
})
export class DiagramComponent extends UnsubscribeOnDestroy implements OnInit {

  @ViewChild('wrapper')
  set wrapper(wrapper: ElementRef) {
    if (wrapper) {
      const wrapperWidth = wrapper.nativeElement.offsetWidth;
      const wrapperHeight = wrapper.nativeElement.offsetHeight;
      this.eventService.DiagramSizeCalculated.emit({ width: wrapperWidth, height: wrapperHeight } as DiagramSize)
      this.cd.detectChanges();
    }
  }

  project: Project;
  projectFormValid = false;
  showDeployment = false;
  configuration: Configuration;
  currentDiagramObjectId: string = '';
  constructor(
    private cd: ChangeDetectorRef,
    private route: ActivatedRoute,
    private configurationService: ConfigurationService,
    private projectService: ProjectService,
    private diagramManager: DiagramManager,
    private messageService: MessageService,
    private eventService: EventService
  ) { super(); }

  canLoad = false;


  ngOnInit() {
    this.route.params.subscribe(params => {
      this.projectService.get(params['id']).subscribe(x => {
        this.project = x;
        this.eventService.ProjectSelectedEvent.emit({ ...x });
      });
    });
    this.configurationService.getConfiguration().subscribe(x => {
      this.configuration = x;
      this.eventService.ConfigurationLoaded.emit({ ...this.configuration });
    });

    this.eventService.ProjectUpdatedEvent.pipe(takeUntil(this.destroy$)).subscribe(x => {
      this.project = x;
    });
    this.eventService.DiagramSelectionChanged.pipe(takeUntil(this.destroy$)).subscribe(x => {
      this.currentDiagramObjectId = x;
    });
    this.eventService.ProjectFormValidationChanged.pipe(takeUntil(this.destroy$)).subscribe(x => {
      this.projectFormValid = x;
    })
  }

  save() {
    this.project.diagram = JSON.stringify(this.diagramManager.getDiagramJson());
    this.projectService.edit(this.project).subscribe(x => {
      this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Project updated' });
    });
  }

  zoomUp() {
    const current = this.diagramManager.getZoom();
    this.diagramManager.setZoom(current + 0.1);
  }

  zoomDown() {
    const current = this.diagramManager.getZoom();
    this.diagramManager.setZoom(current - 0.1);
  }

  deleteItem() {
    const node = this.diagramManager.getActiveNode();
    const nodeData = node.data as DiagramObject;

    this.diagramManager.getAppNodes().filter(x => x.authorizedApps.includes(nodeData.id)).forEach(x => {
      x.authorizedApps = x.authorizedApps.filter(x => x != nodeData.id);
    });
    this.diagramManager.getAppNodes().filter(x => x.connectedResources.includes(nodeData.id)).forEach(x => {
      x.connectedResources = x.connectedResources.filter(x => x != nodeData.id);
    });

    this.diagramManager.remove();
  }

  downloadImage() {
    var element = document.createElement('a');
    var cos = this.diagramManager.getImage();
    element.setAttribute('href', this.diagramManager.getImage());
    element.setAttribute('download', `${this.project.name}-diagram.jpg`);

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
  }
}
