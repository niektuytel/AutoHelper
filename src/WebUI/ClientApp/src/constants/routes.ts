import { useParams } from "react-router";

export const ROUTES = {
    SELECT_VEHICLE: "/select-vehicle",
    SELECT_GARAGE: "/select-garage",
    GARAGE: "/garage",
    GARAGE_ACCOUNT: {
        OVERVIEW: "/garage/overview",
        PLANNING: "/garage/planning",
        SERVICES: "/garage/services",
        EMPLOYEES: "/garage/employees",
        SETTINGS: "/garage/settings"
    },
    USER: {
        OVERVIEW: "/user/overview"
    },
};
