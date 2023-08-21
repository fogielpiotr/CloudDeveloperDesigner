import { DiagramCalculation } from './../../../core/fabric/diagram.calulation';
import { NamingHelper } from 'src/app/modules/core/helpers/resource-naming-helper';
import { DiagramManager } from 'src/app/modules/core/fabric/diagram.manager';
import { takeUntil } from 'rxjs';
import { UnsubscribeOnDestroy } from './../../../core/helpers/unsubscribe-on-destroy';
import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ResourceDiagramObject } from '../../shared/diagram-object.model';
import { noWhitespaceValidator } from 'src/app/modules/core/helpers/noWhitespaceValidator';
import { lowerCaseValidator } from 'src/app/modules/core/helpers/lowerCaseValidator';

@Component({
  selector: 'app-resource-node-info',
  templateUrl: './resource-node-info.component.html',
  styleUrls: ['./resource-node-info.component.scss']
})
export class ResourceNodeInfoComponent extends UnsubscribeOnDestroy implements OnInit {
  @Input()
  diagramObject: fabric.Object
  objectData: ResourceDiagramObject;
  params: Array<string> = [];
  resourceForm: FormGroup = new FormGroup({})

  constructor(private diagramManager: DiagramManager) { super() }

  ngOnInit(): void {
    this.objectData = this.diagramObject.data as ResourceDiagramObject;
    this.resourceForm = new FormGroup({
      name: new FormControl(this.objectData.name,
        [
          Validators.required,
          Validators.maxLength(this.objectData.resourceNaming.maxLenght -
            this.objectData.resourceNaming.abbreviation.length -
            (this.objectData.resourceNaming.divider.length * 2) -
            3), //for envirionment), ),
          Validators.minLength(this.objectData.resourceNaming.minLenght -
            this.objectData.resourceNaming.abbreviation.length -
            (this.objectData.resourceNaming.divider.length * 2) -
            3), //for envirioment),
          noWhitespaceValidator,
          lowerCaseValidator
        ])
    });

    if (!this.objectData.isSecretProvider) {
      this.resourceForm.addControl('secretName', new FormControl(this.objectData.secretName,
        [Validators.required, noWhitespaceValidator]));
    }
    this.params = Object.keys(this.objectData.parameters).filter(x => x.toLocaleLowerCase() !== 'name');
    this.params.map(x => {
      this.resourceForm.addControl(x, new FormControl('', Validators.required));
      this.resourceForm.controls[x].patchValue(this.objectData.parameters[x].value, { emitEvent: false })
    });
    this.objectData.valid = this.resourceForm.valid;

    this.resourceForm.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(formValue => {
      this.objectData.name = formValue.name;
      this.objectData.secretName = formValue.secretName;
      this.objectData.parameters['name'].value = formValue.name;
      this.params.filter(x=>x).map(paramKey => {
        if(paramKey !== 'name') {
          this.objectData.parameters[paramKey].value = formValue[paramKey];
        }
      });
      this.objectData.valid = this.resourceForm.valid;
      this.changeTextNodeText();
    });
  }

  private changeTextNodeText() {
    const textId = this.diagramManager.getTextNodes().find(x => x.nodeId == this.objectData.id).id;
    const textNode = this.diagramManager.getNodeById(textId) as fabric.Text;
    const resourceNode = this.diagramManager.getNodeById(this.objectData.id) as fabric.Image;
    textNode.text = NamingHelper.encodeResourceName(this.objectData.resourceNaming, this.objectData.name, '');
    DiagramCalculation.calculateTextNodePosition(textNode, resourceNode);
    this.diagramManager.renderDiagram();
  }
}
