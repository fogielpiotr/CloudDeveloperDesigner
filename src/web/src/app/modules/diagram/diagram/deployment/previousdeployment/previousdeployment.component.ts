import { takeUntil } from 'rxjs';
import { EventService } from 'src/app/modules/core/services/event-service';
import { UnsubscribeOnDestroy } from 'src/app/modules/core/helpers/unsubscribe-on-destroy';
import { DeploymentService } from 'src/app/modules/core/services/deployment-service';
import { Component, Input, OnInit } from '@angular/core';
import { Deployment, DeploymentStatus } from 'src/app/modules/diagram/shared/deployment-info';

@Component({
  selector: 'app-previousdeployment',
  templateUrl: './previousdeployment.component.html',
  styleUrls: ['./previousdeployment.component.scss']
})
export class PreviousdeploymentComponent extends UnsubscribeOnDestroy implements OnInit {

  @Input()
  ProjectId: string;

  Deployments: Deployment[] = [];
  DeploymentStatus = DeploymentStatus;
  SelectedDeployment: Deployment;
  ShowInfo = false;

  constructor(private deploymentService: DeploymentService, private eventService: EventService) { super(); }

  ngOnInit(): void {
    this.getDeployments();
    this.eventService.DeploymentQueued.pipe(takeUntil(this.destroy$)).subscribe(x => {
      this.getDeployments();
    })
  }

  private getDeployments() {
    this.deploymentService.getDeploymentsForProject(this.ProjectId).subscribe(x => {
      this.Deployments = x;
    });
  }

  openDetails(deployment: Deployment) {
    this.SelectedDeployment = deployment;
    this.ShowInfo = true;
  }

  onVisibleChanged(visible) {
    if(visible === false) {
      this.SelectedDeployment = null;
      this.getDeployments();
    }
  }
}
