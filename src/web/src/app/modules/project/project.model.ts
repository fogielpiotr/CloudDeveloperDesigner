export interface Project {
    id: string;
    name: string;
    created: Date;
    updated: Date;
    createdBy: string;
    updatedBy: string;
    description: string;
    diagram: string;
    environments: string[];
    resourceGroup: string;
    mandatoryTags: Record<string,string>; 
    location: string;
}