import { ResourceDragObject } from './../../shared/diagram-drag-object.model';
import { EventService } from './../../../core/services/event-service';
import { DeploymentService } from './../../../core/services/deployment-service';
import { UnsubscribeOnDestroy } from './../../../core/helpers/unsubscribe-on-destroy';
import { takeUntil } from 'rxjs';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { EnvironmentDeploymentCommand, CodeDeployment, ApplicationIdentityDeployment, ResourceDeployment, ResourceGroup, ParamValue } from './../../shared/deployment-command.model';
import { DiagramManager } from 'src/app/modules/core/fabric/diagram.manager';
import { Component, Input, OnInit, AfterViewInit } from '@angular/core';
import { Project } from 'src/app/modules/project/project.model';
import { DiagramObject, ResourceDiagramObject } from '../../shared/diagram-object.model';
import { CreateDeploymentCommand } from '../../shared/deployment-command.model';
import { NamingHelper } from 'src/app/modules/core/helpers/resource-naming-helper';
import * as JSONEditor from 'jsoneditor';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-deployment',
  templateUrl: './deployment.component.html',
  styleUrls: ['./deployment.component.scss']
})
export class DeploymentComponent extends UnsubscribeOnDestroy implements OnInit, AfterViewInit {

  @Input()
  project: Project;

  @Input()
  projectValid: boolean = false;

  activeIndex = 0;

  deploymentForm: FormGroup = new FormGroup({
    codeDeployment: new FormControl(true, Validators.required),
    applicationIdentityDeployment: new FormControl(true, Validators.required),
    resourceDeployment: new FormControl(true, Validators.required)
  });
  editor;
  createDeploymentCommand: CreateDeploymentCommand;
  deploymentObjectsValid: boolean = true;
  stateOptions = [{ label: 'On', value: true }, { label: 'Off', value: false }];

  constructor(
    private diagramManager: DiagramManager,
    private deploymentService: DeploymentService,
    private eventService: EventService,
    private messageService: MessageService) { super(); }

  ngOnInit(): void {
    this.diagramManager.getSelectableObject()?.forEach(x => {
      if (x.data) {
        const typedData = x.data as DiagramObject;
        if (!typedData.valid) {
          this.deploymentObjectsValid = false;
        }
      }
    });
    if (this.canDeploy) {
      this.createDeploymentCommand = {
        projectId: this.project.id,
        environmentDeployments: this.project.environments.map(env => {
          return {
            applicationIdentityDeployments: [],
            environment: env,
            resourceGroup: {},
            resourceDeployments: []
          } as EnvironmentDeploymentCommand;
        }) as Array<EnvironmentDeploymentCommand>,
        codeRepositoryDeployments: new Array<CodeDeployment>()
      } as CreateDeploymentCommand;
    }

    this.deploymentForm.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(x => {
      if (x.codeDeployment) {
        this.createDeploymentCommand.codeRepositoryDeployments = this.diagramManager.getAppNodes().map(app => { return app.codeDeployment });
      } else {
        this.createDeploymentCommand.codeRepositoryDeployments = [];
      }
      if (x.applicationIdentityDeployment) {
        this.createDeploymentCommand.environmentDeployments.forEach(env => {
          env.applicationIdentityDeployments = this.diagramManager.getAppNodes().map(app => {
            return {
              authorizedApps: app.authorizedApps.map(authApp => NamingHelper.encodeAppName(this.diagramManager.getNodeById(authApp).data.name, env.environment)),
              name: NamingHelper.encodeAppName(app.name, env.environment),
              applicationType: app.applicationType,
              redirectUris: app.redirectUris[env.environment],
              clientIdSecretName: app.clientIdSecretName,
              clientSecretName: app.clientSecretName
            } as ApplicationIdentityDeployment
          })
        })
      } else {
        this.createDeploymentCommand.environmentDeployments.forEach(env => {
          env.applicationIdentityDeployments = [];
        });
      }
      if (x.resourceDeployment) {
        for (const env of this.createDeploymentCommand.environmentDeployments) {
          env.resourceGroup = {
            name: `rg-${this.project.resourceGroup}-${env.environment}`,
            location: this.project.location,
            tags: this.project.mandatoryTags
          } as ResourceGroup;
          env.resourceDeployments = [];
          for (const resource of this.diagramManager.getResourceNodes()) {
            const tempResource = JSON.parse(JSON.stringify(resource)) as ResourceDiagramObject;
            const resourceDeployment = {
              parameters: tempResource.parameters,
              resourceId: tempResource.resourceId,
              secretName: tempResource.secretName,
              name: NamingHelper.encodeResourceName(tempResource.resourceNaming, tempResource.name, env.environment)
            } as ResourceDeployment;
            resourceDeployment.parameters['name'].value = resourceDeployment.name;
            env.resourceDeployments.push(resourceDeployment);
          };
        }
      } else {
        this.createDeploymentCommand.environmentDeployments.forEach(env => {
          env.resourceGroup = {} as ResourceGroup,
            env.resourceDeployments = []
        })
      }
      if (this.editor) {
        this.editor.set(this.createDeploymentCommand);
      }
    });
  }

  ngAfterViewInit(): void {
    const container = document.getElementById("jsoneditor")
    const options = {
      mode: 'tree',
      modes: ['text', 'tree'],
      onChangeText: (text) => {
        try {
          this.createDeploymentCommand = JSON.parse(text);
        } catch (e) {
          console.log(e);
        }
      }
    }
    this.editor = new JSONEditor(container, options);
    this.deploymentForm.patchValue({
      codeDeployment: true,
      applicationIdentityDeployment: true,
      resourceDeployment: true
    });
    this.editor.set(this.createDeploymentCommand);
  }

  get canDeploy(): boolean {
    return this.deploymentObjectsValid && this.projectValid;
  }

  deploy() {
    this.deploymentService.createDeployment(this.createDeploymentCommand).subscribe(x => {
      this.activeIndex = 1;
      this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Deployment created and scheduled' });
      this.eventService.DeploymentQueued.emit(x);
    });
  }
}
