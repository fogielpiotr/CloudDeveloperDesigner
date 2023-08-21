import { DeploymentService } from 'src/app/modules/core/services/deployment-service';
import { takeUntil } from 'rxjs';
import { UnsubscribeOnDestroy } from './../../../../../core/helpers/unsubscribe-on-destroy';
import { Component, Input, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { Deployment, DeploymentStatus, DeploymentLog } from 'src/app/modules/diagram/shared/deployment-info';
import * as signalR from "@microsoft/signalr";
import { environment } from 'src/environments/environment';
import { MsalService } from '@azure/msal-angular';

@Component({
  selector: 'app-deploymentdetails',
  templateUrl: './deploymentdetails.component.html',
  styleUrls: ['./deploymentdetails.component.scss']
})
export class DeploymentdetailsComponent extends UnsubscribeOnDestroy implements AfterViewInit {

  @Input()
  Deployment: Deployment;

  DeploymentStatus = DeploymentStatus;
  connection: signalR.HubConnection;
  StatusMessages: DeploymentLog[] = [];
  Tabs: string[] = [];
  CodeDeploymentTab = "Code Deployment";

  constructor(private authService: MsalService, private cd: ChangeDetectorRef,
    private deploymentService: DeploymentService) { super(); }

  ngAfterViewInit(): void {
    this.deploymentService.getDeploymentsLogs(this.Deployment.id).subscribe(x => {
      console.log(x);
      this.StatusMessages.push(...x);
    });
    const tabs = this.Deployment.environments;
    if (this.Deployment.codeDeployment) {
      tabs.push(this.CodeDeploymentTab);
    }
    this.Tabs = tabs;
    this.cd.detectChanges();

    if (this.Deployment.status === DeploymentStatus.DeploymentQueued) {
      this.authService.acquireTokenSilent(
        { account: this.authService.instance.getAllAccounts()[0], scopes: [environment.apiScope] })
        .pipe(takeUntil(this.destroy$))
        .subscribe(token => {
          this.connection = new signalR.HubConnectionBuilder()
            .withUrl(environment.signalRHub, { accessTokenFactory: () => token.accessToken })
            .build();
  
  
          this.connection.start().then(x => {
            this.connection.on(this.Deployment.id, (message: DeploymentLog) => {
              this.StatusMessages.push(message);
            });
          });
        });
    }
  }

  override ngOnDestroy(): void {
    if (this.Deployment.status === DeploymentStatus.DeploymentQueued) {
      this.connection.off(this.Deployment.id);
      this.connection.stop();
    }

    super.ngOnDestroy();
  }

  getDeploymentNotification(tab: string | null) {
    if (tab === this.CodeDeploymentTab) {
      return this.StatusMessages.filter(x => x.codeDeployment === true);
    }
    return this.StatusMessages.filter(x => x.environment === tab && !x.codeDeployment);
  }

  openLink(url: string) {
    window.open(url, "_blank");
  }
}
