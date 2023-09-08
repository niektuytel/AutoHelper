// INFO:
// Used to get user id to pass the api, nswag does not integrate this functionality
// to pass the bearer token to the api

import { GarageClient, GarageRegisterClient } from "./web-api-client";

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

export function GetGarageClient(accessToken: Promise<string>): GarageClient {
    const client = new GarageClient(process.env.PUBLIC_URL, { fetch: (url: RequestInfo, init?: RequestInit) => fetchWithToken(accessToken, url, init) });
    return client;
}

export function GetGarageRegisterClient(accessToken: Promise<string>): GarageRegisterClient {
    const client = new GarageRegisterClient(process.env.PUBLIC_URL, { fetch: (url: RequestInfo, init?: RequestInit) => fetchWithToken(accessToken, url, init) });
    return client;
}


