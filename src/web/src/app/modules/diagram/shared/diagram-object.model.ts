import { DiagramObjectType } from "../../core/fabric/diagram-object-type-enum";
import { ApplicationType } from "./application.model";
import {  CodeDeployment, ParamValue } from "./deployment-command.model";
import { ResourceNaming } from "./resource.model";
import { Template } from "./template.model";

export interface DiagramObject {
    id: string;
    name: string;
    displayName: string;
    backendName: string;
    type: DiagramObjectType;
    iconFile: string;
    valid: boolean;
}

export interface ResourceDiagramObject extends DiagramObject {
    resourceNaming: ResourceNaming;
    template: Template;
    parameters: Record<string, ParamValue>;
    secretName: string;
    isSecretProvider: boolean;
    resourceId: string;
}


export interface AppDiagramObject extends DiagramObject {
    environments: string[],
    connectedResources: string[];
    authorizedApps: string[];
    codeDeployment: CodeDeployment;
    applicationType: ApplicationType;
    redirectUris: Record<string, string[]>;
    clientSecretName: string;
    clientIdSecretName: string;
}

export interface LineDiagramObject extends DiagramObject {
    from: string;
    to: string;
}

export interface ArrowDiagramObject extends DiagramObject {
    lineId: string;
}

export interface TextDiagramObject extends DiagramObject {
    nodeId: string;
}

