import { Template } from './template.model';

export interface Resource {
    id: string,
    name: string;
    displayName: string;
    iconFile: string;
    template: Template;
    isSecretProvider: boolean;
    resourceNaming: ResourceNaming
}

export interface ResourceNaming {
    abbreviation: string;
    divider: string;
    maxLenght: number;
    minLenght: number;
}

