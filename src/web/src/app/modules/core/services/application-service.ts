import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { Application } from "../../diagram/shared/application.model";


@Injectable()
export class ApplicationService {
    constructor(private http: HttpClient) { }


    getApplication(): Observable<Application[]> {
        return this.http.get<Application[]>(`${environment.apiUrl}/Application`);
    }
}
