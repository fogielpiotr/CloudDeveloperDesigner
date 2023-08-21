import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Project } from 'src/app/modules/project/project.model';
import { ProjectService } from 'src/app/modules/core/services/project.service';

@Component({
  selector: 'app-project-overview',
  templateUrl: './project-overview.component.html',
  styleUrls: ['./project-overview.component.scss']
})
export class ProjectOverviewComponent implements OnInit {

  constructor(
    private projectService: ProjectService, 
    private messageService: MessageService, 
    private confirmationService: ConfirmationService,
    private router: Router
    ) { }

  projects: Project[] = new Array<Project>();
  projectToEdit: Project = null;
  showAddNewProject = false;
  showEditProject = false;

  ngOnInit(): void {
    this.getProjects();
  }

  deleteProject(projectId: string) {
    this.confirmationService.confirm({
      message: 'Are you sure that you want to delete this project?',
      accept: () => {
        this.projectService.delete(projectId).subscribe(x => {
          this.messageService.add({ severity: 'success', summary: 'Project deleted.' });
          this.getProjects();
        })
      }
    });
  }

  onProjectCreated(project: Project) {
    this.showAddNewProject = false;
    this.getProjects();
  }

  onProjectEdit(project: Project) {
    this.projectToEdit = this.projects.find(x => x.id === project.id);
    this.showEditProject = true;
  }

  onProjectEdited(project: Project) {
    this.showEditProject = false;
    this.getProjects();
  }

  openProject(project: Project) {
    this.router.navigateByUrl(`/diagram/${project.id}`);
  }

  private getProjects(): void {
    this.projectService.getProjects().subscribe(x => {
      this.projects = x;
    })
  }
}
