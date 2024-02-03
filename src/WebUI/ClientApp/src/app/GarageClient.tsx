// INFO:
// Used to get user id to pass the api, nswag does not integrate this functionality
// to pass the bearer token(IDToken) to the api with user specific data 

import { useMsal } from "@azure/msal-react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { garageLoginRequest, protectedResources } from "../authConfig";
import { ROUTES } from "../constants/routes";
import useRoleIndex from "../hooks/useRoleIndex";
import useUserRole from "../hooks/useUserRole";
import { showOnError } from "../redux/slices/statusSnackbarSlice";
import { GarageAccountClient } from "./web-api-client";

export function GetGarageAccountClient() {
    const { instance, accounts } = useMsal();
    const account = accounts[0];

    async function fetchWithAuth(url: RequestInfo, init?: RequestInit): Promise<Response> {
        try {
            const response = await instance.acquireTokenSilent({
                scopes: garageLoginRequest.scopes,
                account: account
            });

            const authHeaders = { "Authorization": `Bearer ${response.accessToken}` };
            const headers = init?.headers ? { ...init.headers, ...authHeaders } : authHeaders;

            return window.fetch(url, { ...init, headers });
        } catch (error) {
            console.error('Error fetching with auth', error);
            throw error; // Rethrow or handle as needed
        }
    }

    const client = new GarageAccountClient(process.env.PUBLIC_URL, { fetch: fetchWithAuth });
    return client;
}

export function useHandleApiRequest<T>() {
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const { userRole } = useUserRole();
    const { setConfigurationIndex } = useRoleIndex();

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