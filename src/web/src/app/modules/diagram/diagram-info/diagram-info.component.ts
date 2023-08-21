import { combineLatest, takeUntil } from 'rxjs';
import { EventService } from './../../core/services/event-service';
import { UnsubscribeOnDestroy } from './../../core/helpers/unsubscribe-on-destroy';
import { Configuration } from './../shared/configuration.model';
import { Component, OnInit, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { DiagramManager } from '../../core/fabric/diagram.manager';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Project } from '../../project/project.model';
import { DiagramObjectType } from '../../core/fabric/diagram-object-type-enum';
import { noWhitespaceValidator } from '../../core/helpers/noWhitespaceValidator';
import { DiagramObject } from '../shared/diagram-object.model';
import { lowerCaseValidator } from '../../core/helpers/lowerCaseValidator';

@Component({
  selector: 'app-diagram-info',
  templateUrl: './diagram-info.component.html',
  styleUrls: ['./diagram-info.component.scss']
})
export class DiagramInfoComponent extends UnsubscribeOnDestroy implements OnInit, AfterViewInit {

  diagramForm: FormGroup;
  mandatoryTagsFormGroup = new FormGroup({});
  configuration: Configuration;
  project: Project;
  diagramObjectType = DiagramObjectType;
  currentDiagramObjectType: DiagramObjectType;
  currentDiagramObject: fabric.Object;
  diagramActiveId = '';

  constructor(
    private diagramManager: DiagramManager,
    private eventService: EventService,
    private cd: ChangeDetectorRef
  ) {
    super();
    this.diagramForm = new FormGroup({
      resourceGroup: new FormControl('', [Validators.required, noWhitespaceValidator, lowerCaseValidator]),
      location: new FormControl('', Validators.required),
      environments: new FormControl('', Validators.required),
      mandatoryTags: this.mandatoryTagsFormGroup
    }
    );
  }

  ngOnInit(): void {
    combineLatest([
      this.eventService.ProjectSelectedEvent.pipe(takeUntil(this.destroy$)),
      this.eventService.ConfigurationLoaded.pipe(takeUntil(this.destroy$))]
    ).subscribe(([project, configuration]) => {
      this.project = project;
      if (!this.project.resourceGroup) {
        this.project.resourceGroup = this.project.name.split(' ').join('');
      }
      this.configuration = configuration;
      if (configuration.mandatoryTags) {
        configuration.mandatoryTags.map(x => {
          this.mandatoryTagsFormGroup.addControl(x, new FormControl(this.project.mandatoryTags?.[x] ?? '', Validators.required), { emitEvent: false })
        });

      };
      this.diagramForm.patchValue(this.project, { emitEvent: false });
      this.eventService.ProjectFormValidationChanged.emit(this.diagramForm.valid);
    });
    this.eventService.DiagramSelectionChanged.pipe(takeUntil(this.destroy$)).subscribe(x => {
      this.diagramActiveId = null;
      this.cd.detectChanges();
      this.diagramActiveId = x;
      this.currentDiagramObject = this.diagramManager.getActiveNode();
      this.currentDiagramObjectType = this.diagramManager.getActiveNodeType();
      if (this.diagramActiveId == null) {
        this.diagramManager.getSelectableObject()?.forEach(x => {
          if (x.data) {
            const typedData = x.data as DiagramObject;
            if (typedData.valid) {
              x.stroke = null;
              x.strokeDashArray = null;
              x.strokeWidth = null;
            } else {
              x.stroke = 'red';
              x.strokeWidth = 1 / x.scaleX;
            }
          }
        });
        this.diagramManager.renderDiagram();
      }
    });
  }

  ngAfterViewInit(): void {
    this.diagramForm.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((x: Project) => {
      this.project.resourceGroup = x.resourceGroup;
      this.project.location = x.location;
      this.project.environments = x.environments;
      this.project.mandatoryTags = x.mandatoryTags;
      this.eventService.ProjectUpdatedEvent.emit({ ...this.project });
      this.eventService.ProjectFormValidationChanged.emit(this.diagramForm.valid);
    });
  }
}
