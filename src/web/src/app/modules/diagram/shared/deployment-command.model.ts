import { ApplicationType } from "./application.model";

export interface CreateDeploymentCommand {
    projectId: string;
    environmentDeployments: EnvironmentDeploymentCommand[];
    codeRepositoryDeployments: CodeDeployment[];
}

export interface EnvironmentDeploymentCommand {
    environment: string;
    resourceGroup: ResourceGroup;
    resourceDeployments: ResourceDeployment[];
    applicationIdentityDeployments: ApplicationIdentityDeployment[];
}

export interface ResourceGroup {
    name: string;
    location: string;
    tags: Record<string, string>;
}

export interface ResourceDeployment {
    name: string,
    resourceId: string,
    secretName: string,
    parameters: Record<string, ParamValue>;
}

export interface ParamValue {
    value: string;
}

export interface ApplicationIdentityDeployment {
    name: string;
    applicationType: ApplicationType;
    authorizedApps: string[];
    redirectUris: string[];
    clientSecretName: string;
    clientIdSecretName: string;
}

export interface CodeDeployment {
    appId: string;
    repositoryName: string;
    settingsJson: string;
}

