import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { Project } from 'src/app/modules/project/project.model';
import { ProjectService } from 'src/app/modules/core/services/project.service';
import { lowerCaseValidator } from '../../core/helpers/lowerCaseValidator';

@Component({
  selector: 'app-project-edit',
  templateUrl: './project-edit.component.html',
  styleUrls: ['./project-edit.component.scss']
})
export class ProjectEditComponent implements OnInit {

  projectForm: FormGroup;
  
  @Input() project: Project;
  @Output() onProjectUpdated: EventEmitter<Project> = new EventEmitter();

  constructor(private projectService: ProjectService, private messageService: MessageService) { 
    this.projectForm = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.minLength(3), Validators.maxLength(25), lowerCaseValidator]),
      description: new FormControl('', Validators.required)
    })
  }

  ngOnInit(): void {
    this.projectForm.controls['name'].setValue(this.project.name);
    this.projectForm.controls['description'].setValue(this.project.description);
  }

  
  edit() {
    if (this.projectForm.valid) {
      this.project.name = this.projectForm.controls['name'].value;
      this.project.description = this.projectForm.controls['description'].value;
      
      this.projectService.edit(this.project).subscribe(x => {
         this.onProjectUpdated.emit(x);
         this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Project updated' });
         this.projectForm.reset();
      })
    };
  }
}
