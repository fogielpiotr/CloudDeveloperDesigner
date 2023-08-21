export interface Application {
    id: string;
    name: string;
    displayName: string;
    iconFile: string;
    type: ApplicationType;
}

export enum ApplicationType {
    Web = 1,
    Server
}
