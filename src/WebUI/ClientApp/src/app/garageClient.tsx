import { GarageClient } from "./web-api-client";


function GetGarageClient(accessToken: string): GarageClient {

    const garageClient = new GarageClient(process.env.PUBLIC_URL, { fetch: fetchWithToken });
    async function fetchWithToken(url: RequestInfo, init?: RequestInit): Promise<Response> {
        const newInit: RequestInit = {
            ...init,
            headers: {
                ...init?.headers,
                "Authorization": `Bearer ${accessToken}`
            }
        };
        return window.fetch(url, newInit);
    }

    return garageClient;
}

export default GetGarageClient;
