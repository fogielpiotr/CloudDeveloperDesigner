import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Configuration } from '../../diagram/shared/configuration.model';

@Injectable()
export class ConfigurationService {
    constructor(private http: HttpClient) { }


    getConfiguration(): Observable<Configuration> {
        return this.http.get<Configuration>(`${environment.apiUrl}/Configuration`);
    }
}