import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { Resource } from "../../diagram/shared/resource.model";

@Injectable()
export class ResourceService {
    constructor(private http: HttpClient) { }
    
    
    getResources(): Observable<Resource[]> {
        return this.http.get<Resource[]>(`${environment.apiUrl}/Resource`);
    }
}

