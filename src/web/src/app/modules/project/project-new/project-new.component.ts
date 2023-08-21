import { MessageService } from 'primeng/api';
import { ProjectService } from '../../core/services/project.service';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Project } from 'src/app/modules/project/project.model';
import { lowerCaseValidator } from '../../core/helpers/lowerCaseValidator';

@Component({
  selector: 'app-project-new',
  templateUrl: './project-new.component.html',
  styleUrls: ['./project-new.component.scss']
})
export class ProjectNewComponent implements OnInit {
  @Output() onProjectCreated: EventEmitter<Project> = new EventEmitter();
  
  public projectForm: FormGroup;

  constructor(private projectService: ProjectService, private messageService: MessageService) {
    this.projectForm = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.minLength(3), Validators.maxLength(25), lowerCaseValidator]),
      description: new FormControl('', Validators.required)
    })
  }

  ngOnInit(): void {
  }


  addProject() {
    if (this.projectForm.valid) {
      this.projectService.add(this.projectForm.value).subscribe(x => {
        this.onProjectCreated.emit(x);
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Project added' });
        this.projectForm.reset();
      })
    };
  }
}
