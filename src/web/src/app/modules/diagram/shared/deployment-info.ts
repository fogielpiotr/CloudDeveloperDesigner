
export interface Deployment {
    creator: string;
    id: string;
    projectId: string;
    status: DeploymentStatus;
    createdAt: Date;
    environments: string[];
    codeDeployment: boolean;
}

export enum DeploymentStatus {
    OperationStarted,
    OperationSucceeded,
    OperationFailed,
    DeploymentFinished,
    DeploymentQueued,
    DeploymentFailed,
    DeploymentStarted
}

export interface DeploymentLog {
    status: DeploymentStatus;
    date: Date;
    message: string;
    environment: string;
    error: string;
    codeDeployment: boolean;
    url: string;
}
