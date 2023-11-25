import { useParams } from "react-router";

export const ROUTES = {
    SELECT_VEHICLE: "/select-vehicle",
    SELECT_GARAGE: "/select-garage",
    GARAGE: "/garage",
    GARAGE_ACCOUNT: {
        OVERVIEW: "/garage/overview",
        PLANNING: "/garage/planning",
        EMPLOYEES: "/garage/employees",
        SERVICES: "/garage/services",
        SERVICELOGS: "/garage/servicelogs",
        SETTINGS: "/garage/settings"
    },
    USER: {
        OVERVIEW: "/user/overview"
    },
};
