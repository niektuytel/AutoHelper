// INFO:
// Used to get user id to pass the api, nswag does not integrate this functionality
// to pass the bearer token(IDToken) to the api with user specific data 

import { useAuth0 } from "@auth0/auth0-react";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { ROUTES } from "../constants/routes";
import useConfirmationStep from "../hooks/useConfirmationStep";
import useUserRole from "../hooks/useUserRole";
import { showOnError } from "../redux/slices/statusSnackbarSlice";
import { GarageAccountClient } from "./web-api-client";

async function fetchWithToken(accessToken: Promise<string>, url: RequestInfo, init?: RequestInit): Promise<Response> {
    const resolvedToken = await accessToken;
    const request: RequestInit = {
        ...init,
        headers: {
            ...init?.headers,
            "Authorization": `Bearer ${resolvedToken}`
        }
    };
    return window.fetch(url, request);
}

export function GetGarageAccountClient(): GarageAccountClient {
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();

    const client = new GarageAccountClient(process.env.PUBLIC_URL, { fetch: (url: RequestInfo, init?: RequestInit) => fetchWithToken(accessToken, url, init) });
    return client;
}

export function useHandleApiRequest<T>() {
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const { userRole } = useUserRole();
    const { setConfigurationIndex } = useConfirmationStep();

    const handleApiRequest = async (
        apiCall: () => Promise<T>,
        messageFor404Error?: string
    ): Promise<T | null> => {
        try {
            const response: T = await apiCall();
            return response;
        } catch (error: any) {
            if (error.status === 404) {
                setConfigurationIndex(0, userRole);
                dispatch(showOnError(messageFor404Error));

                if (window.location.pathname !== ROUTES.GARAGE_ACCOUNT.SETTINGS) {
                    navigate(ROUTES.GARAGE_ACCOUNT.SETTINGS, {
                        state: { from: window.location },
                    });
                }

                return null;
            } else {
                throw error;
            }
        }
    };

    return handleApiRequest;
}