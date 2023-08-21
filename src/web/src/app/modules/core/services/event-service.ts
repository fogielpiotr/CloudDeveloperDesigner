import { EventEmitter, Injectable } from "@angular/core";
import { Configuration } from "../../diagram/shared/configuration.model";
import { DiagramSize } from "../../diagram/shared/diagram-size";
import { Project } from "../../project/project.model";

@Injectable({
    providedIn: 'root'
})
export class EventService {
    ProjectSelectedEvent = new EventEmitter<Project>();
    ProjectUpdatedEvent = new EventEmitter<Project>();
    ConfigurationLoaded = new EventEmitter<Configuration>();
    DiagramSizeCalculated = new EventEmitter<DiagramSize>();
    DiagramSelectionChanged = new EventEmitter<string | null>();
    ProjectFormValidationChanged = new EventEmitter<boolean>();
    DeploymentQueued = new EventEmitter<string>();
}