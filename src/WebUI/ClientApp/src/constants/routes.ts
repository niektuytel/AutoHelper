import { useParams } from "react-router";

export const ROUTES = {
    VEHICLE: "/vehicle",
    GARAGES: "/garages",
    GARAGE: "/garage",
    POLICY: "/policy",
    THANKYOU: "/thankyou",
    GARAGE_ACCOUNT: {
        DEFAULT: "/garage-account",
        OVERVIEW: "/garage-account/overview",
        PLANNING: "/garage-account/planning",
        EMPLOYEES: "/garage-account/employees",
        SERVICES: "/garage-account/services",
        SERVICELOGS: "/garage-account/servicelogs",
        SETTINGS: "/garage-account/settings"
    },
    USER: {
        OVERVIEW: "/user/overview"
    },
};
