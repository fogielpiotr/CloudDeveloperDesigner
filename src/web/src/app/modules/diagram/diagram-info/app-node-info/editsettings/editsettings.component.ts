import { Component, AfterViewInit, Input } from '@angular/core';
import * as JSONEditor from 'jsoneditor';
import { AppDiagramObject } from '../../../shared/diagram-object.model';

@Component({
  selector: 'app-editsettings',
  templateUrl: './editsettings.component.html',
  styleUrls: ['./editsettings.component.scss']
})
export class EditsettingsComponent implements AfterViewInit {
  @Input()
  nodeData: AppDiagramObject;
  constructor() { }


  ngAfterViewInit(): void {
    const container = document.getElementById("jsoneditor")
    const options = {
      mode: 'text',
      onChangeText: (text) => {
        try {
          this.nodeData.codeDeployment.settingsJson = JSON.stringify(JSON.parse(text));
        } catch (e) {
          //
        }
      }
    }
    const editor = new JSONEditor(container, options);
    const data = JSON.parse(this.nodeData.codeDeployment.settingsJson);
    editor.set(data);
  }
}
