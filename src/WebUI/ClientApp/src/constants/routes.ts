

export const ROUTES = {
    GARAGE: {
        OVERVIEW: "/garage/:garage_guid/overview",
        AGENDA: "/garage/:garage_guid/agenda",
        SERVICES: "/garage/:garage_guid/services",
        COLLEAGUES: "/garage/:garage_guid/colleagues",
        SETTINGS: "/garage/:garage_guid/settings"
    },
    USER: {
        OVERVIEW: "/user/overview"
    },
    SELECT_VEHICLE: "/select-vehicle"
};

export const GARAGE_GUID_PREFIX = ":garage_guid";

export function RoutesGarageValue(route: string, garage_guid: string) {
    return route.replace(GARAGE_GUID_PREFIX, garage_guid);
}

export function RoutesGarageOverview(garage_guid: string) {
    return ROUTES.GARAGE.OVERVIEW.replace(GARAGE_GUID_PREFIX, garage_guid);
}

export function RoutesGarageAgenda(garage_guid: string) {
    return ROUTES.GARAGE.AGENDA.replace(GARAGE_GUID_PREFIX, garage_guid);
}

export function RoutesGarageServices(garage_guid: string) {
    return ROUTES.GARAGE.SERVICES.replace(GARAGE_GUID_PREFIX, garage_guid);
}

export function RoutesGarageColleagues(garage_guid: string) {
    return ROUTES.GARAGE.COLLEAGUES.replace(GARAGE_GUID_PREFIX, garage_guid);
}

export function RoutesGarageSettings(garage_guid: string) {
    return ROUTES.GARAGE.SETTINGS.replace(GARAGE_GUID_PREFIX, garage_guid);
}

