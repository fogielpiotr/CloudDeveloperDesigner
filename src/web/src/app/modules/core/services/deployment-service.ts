import { CreateDeploymentCommand } from 'src/app/modules/diagram/shared/deployment-command.model';
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { Deployment, DeploymentLog } from 'src/app/modules/diagram/shared/deployment-info';


@Injectable()
export class DeploymentService {
    constructor(private http: HttpClient) { }


    createDeployment(command: CreateDeploymentCommand): Observable<string> {
        return this.http.post<string>(`${environment.apiUrl}/Deployment`, command);
    }

    getDeploymentsForProject(projectId: string): Observable<Deployment[]> {
        return this.http.get<Deployment[]>(`${environment.apiUrl}/Deployment/${projectId}`);
    }

    getDeploymentsLogs(deploymentId: string): Observable<DeploymentLog[]> {
        return this.http.get<DeploymentLog[]>(`${environment.apiUrl}/Deployment/logs/${deploymentId}`)
    }
}
