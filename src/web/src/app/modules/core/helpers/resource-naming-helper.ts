import { ResourceNaming } from "../../diagram/shared/resource.model";

export class NamingHelper {
    static encodeResourceName(naming: ResourceNaming, name: string, env: string) {
        let envName = env;
        name = name.split(' ').join('')
        if (!envName) {
            envName = '{env}'
        }
        return `${naming.abbreviation}${naming.divider}${name}${naming.divider}${envName}`
    }

    static encodeAppName(appName: string, env: string) {
        let envName = env;
        if (!envName) {
            envName = '{env}'
        }
        return `${appName}${envName}`
    }
}

