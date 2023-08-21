export interface Template {
    parameters: Record<string, ParameterDefinition> 
}

export interface ParameterDefinition {
    type: string,
    defaultValue: string,
    minValue: number,
    maxValue: number,
    minLenght: number,
    maxLenght: number
    allowedValues: string[];
}