import { DiagramObjectType } from "../../core/fabric/diagram-object-type-enum";
import { Application } from "./application.model";
import { Resource } from "./resource.model";


export interface DiagramDragObject {
    type: DiagramObjectType;
}

export interface ApplicationDragObject extends DiagramDragObject {
    data: Application;
}

export interface ResourceDragObject extends DiagramDragObject {
    data: Resource;
}

export const DATA_TRANSFER_INFO = 'dataTransferInfo'