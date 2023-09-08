import { VehicleClient } from "./web-api-client";


function GetGarageRegisterClient(accessToken: string): VehicleClient {

    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL, { fetch: fetchWithToken });
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

    return vehicleClient;
}

export default GetGarageRegisterClient;
