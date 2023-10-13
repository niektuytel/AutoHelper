// INFO:
// Used to get user id to pass the api, nswag does not integrate this functionality
// to pass the bearer token(IDToken) to the api with user specific data 

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

export function GetGarageAccountClient(accessToken: Promise<string>): GarageAccountClient {
    const client = new GarageAccountClient(process.env.PUBLIC_URL, { fetch: (url: RequestInfo, init?: RequestInit) => fetchWithToken(accessToken, url, init) });
    return client;
}

