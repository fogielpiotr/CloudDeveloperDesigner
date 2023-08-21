import { DropdownValue } from './../../shared/configuration.model';
import { UnsubscribeOnDestroy } from './../../../core/helpers/unsubscribe-on-destroy';
import { takeUntil } from 'rxjs';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Component, Input, OnInit } from '@angular/core';
import { AppDiagramObject, ResourceDiagramObject } from '../../shared/diagram-object.model';
import { noWhitespaceValidator } from 'src/app/modules/core/helpers/noWhitespaceValidator';
import { ApplicationType } from '../../shared/application.model';
import { DiagramManager } from 'src/app/modules/core/fabric/diagram.manager';
import { NamingHelper } from 'src/app/modules/core/helpers/resource-naming-helper';
import { DiagramCalculation } from 'src/app/modules/core/fabric/diagram.calulation';

@Component({
  selector: 'app-app-node-info',
  templateUrl: './app-node-info.component.html',
  styleUrls: ['./app-node-info.component.scss']
})
export class AppNodeInfoComponent extends UnsubscribeOnDestroy implements OnInit {

  @Input()
  diagramObject: fabric.Object
  objectData: AppDiagramObject;
  appForm: FormGroup = new FormGroup({});
  applicationType: ApplicationType;
  applicationTypes = ApplicationType;
  posibbleApps = Array<DropdownValue>();
  possibleResources = Array<DropdownValue>();
  envs = Array<string>();
  showSettingsWindow = false;
  constructor(private diagramManager: DiagramManager) { super(); }

  ngOnInit(): void {
    this.objectData = this.diagramObject.data as AppDiagramObject;
    this.applicationType = this.objectData.applicationType;
    this.envs = this.objectData.environments;
    this.posibbleApps = this.getAuthorizedAppsDropdown();
    this.possibleResources = this.getConnectedResourcesDropdown();
    this.appForm = new FormGroup({
      name: new FormControl(this.objectData.name,
        [
          Validators.required,
          Validators.maxLength(80),
          Validators.minLength(3),
        ]),
      repositoryName: new FormControl(this.objectData.codeDeployment.repositoryName,
        [
          Validators.required,
          Validators.maxLength(50),
          Validators.minLength(3),
          noWhitespaceValidator
        ],
      ),
      clientIdSecretName: new FormControl(this.objectData.clientIdSecretName, Validators.required)
    });
    if (this.applicationType === ApplicationType.Server) {
      this.appForm.addControl('authorizedApps', new FormControl(this.objectData.authorizedApps));
      this.appForm.addControl('connectedResources', new FormControl(this.objectData.connectedResources));
      this.appForm.addControl('clientSecretName', new FormControl(this.objectData.clientSecretName, Validators.required));
    } else {
      this.objectData.environments.map(x => {
        this.appForm.addControl(`redirectUris${x}`, new FormControl(this.objectData.redirectUris[x]));
      });
    }
    this.prepareSettings();
    this.objectData.valid = this.appForm.valid;

    this.appForm.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(formValue => {
      const oldValue = { ...this.objectData };
      this.objectData.name = formValue.name;
      this.objectData.clientIdSecretName = formValue.clientIdSecretName;
      this.objectData.clientSecretName = formValue.clientSecretName;
      this.objectData.codeDeployment.repositoryName = formValue.repositoryName;
      if (this.applicationType === ApplicationType.Server) {
        this.objectData.connectedResources = formValue.connectedResources;
        this.objectData.authorizedApps = formValue.authorizedApps;
      }
      if (this.applicationType === ApplicationType.Web) {
        this.objectData.environments.forEach(x => {
          this.objectData.redirectUris[x] = formValue[`redirectUris${x}`];
        })
      };
      this.createDiagramConnections(oldValue);
      if (JSON.stringify(oldValue.connectedResources.sort()) !== JSON.stringify(this.objectData.connectedResources.sort()) ||
        oldValue.clientIdSecretName !== this.objectData.clientIdSecretName ||
        oldValue.clientSecretName !== this.objectData.clientSecretName
      ) {
        this.prepareSettings(true);
      }
      this.changeTextNodeText();
      this.objectData.valid = this.appForm.valid;
    });
  }

  private createDiagramConnections(oldData: AppDiagramObject) {
    oldData.authorizedApps?.forEach(x => {
      const fromNode = this.diagramManager.getNodeById(x);
      const toNode = this.diagramManager.getActiveNode();
      this.diagramManager.removeConnection(fromNode, toNode);
    });
    oldData.connectedResources?.forEach(x => {
      const fromNode = this.diagramManager.getActiveNode();
      const toNode = this.diagramManager.getNodeById(x);
      this.diagramManager.removeConnection(fromNode, toNode);
    });
    this.objectData.authorizedApps?.forEach(x => {
      const fromNode = this.diagramManager.getNodeById(x);
      const toNode = this.diagramManager.getActiveNode();
      this.diagramManager.connectObjects(fromNode, toNode);
    });
    this.objectData.connectedResources?.forEach(x => {
      const fromNode = this.diagramManager.getActiveNode();
      const toNode = this.diagramManager.getNodeById(x);
      this.diagramManager.connectObjects(fromNode, toNode);
    });
  }

  private prepareSettings(createAgain: boolean = false) {
    if (
      createAgain ||
      JSON.parse(this.objectData.codeDeployment.settingsJson) === null ||
      Object.keys(JSON.parse(this.objectData.codeDeployment.settingsJson)).length === 0) {
      const json = {};
      this.objectData?.connectedResources?.forEach(x => {
        const node = this.diagramManager.getNodeById(x);
        const nodeData: ResourceDiagramObject = node.data;
        if (!nodeData.isSecretProvider) {
          const secret = nodeData.secretName;
          json[secret] = `_(${secret})_`;
        }
      });
      json['ClientSecret']= `_(${this.objectData.clientSecretName})_`;
      json['ClientId']= `_(${this.objectData.clientIdSecretName})_`;
      this.objectData.codeDeployment.settingsJson = JSON.stringify(json);
    }
  }

  private getAuthorizedAppsDropdown(): DropdownValue[] {
    return this.diagramManager.getAppNodes().filter(x => x.id !== this.objectData.id).map(x => {
      return {
        id: x.id,
        name: NamingHelper.encodeAppName(x.name, '')
      } as DropdownValue
    });
  }

  private getConnectedResourcesDropdown(): DropdownValue[] {
    return this.diagramManager.getResourceNodes().filter(x => x.id !== this.objectData.id && x.valid).map(x => {
      return {
        id: x.id,
        name: NamingHelper.encodeResourceName(x.resourceNaming, x.name, '')
      } as DropdownValue
    });
  }

  private changeTextNodeText() {
    const textId = this.diagramManager.getTextNodes().find(x => x.nodeId == this.objectData.id).id;
    const textNode = this.diagramManager.getNodeById(textId) as fabric.Text;
    const appNode = this.diagramManager.getNodeById(this.objectData.id) as fabric.Image;
    textNode.text = NamingHelper.encodeAppName(this.objectData.name, '');
    DiagramCalculation.calculateTextNodePosition(textNode, appNode);
    this.diagramManager.renderDiagram();
  }
}
