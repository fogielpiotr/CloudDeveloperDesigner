import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Project } from '../../project/project.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  constructor(private http: HttpClient) { }


  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${environment.apiUrl}/Project`);
  }

  delete(projectId: string) {
    return this.http.delete(`${environment.apiUrl}/Project/${projectId}`);
  }

  add(project: string) : Observable<Project> {
    return this.http.post<Project>(`${environment.apiUrl}/Project/`, project);
  }

  edit(project: Project) {
    return this.http.put<Project>(`${environment.apiUrl}/Project/`, project);
  }

  get(projectId: string) {
    return this.http.get<Project>(`${environment.apiUrl}/Project/${projectId}`);
  }
}
