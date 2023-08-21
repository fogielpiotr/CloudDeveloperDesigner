export interface Configuration {
    availableEnvironments: string[];
    availableLocations: ConfigurationLocation[];
    mandatoryTags: string[]
}

export interface ConfigurationLocation {
    name: string;
    displayName: string;
}

export interface DropdownValue {
    name: string;
    id: string;
}