import { useQuery, useQueryClient } from "react-query";
import { useState, useEffect } from "react";
import { GarageClient } from "../../../app/web-api-client";
import { useAuth0 } from "@auth0/auth0-react";
import { GetGarageAccountClient } from "../../../app/GarageClient";

export default () => {
    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageAccountClient(accessToken);

    const [licensePlate, setLicensePlate] = useState("");

    const fetchGarageServiceTypes = async () => {
        try {
            const response = await garageClient.getServices(licensePlate);
            return response;
        } catch (response: any) {
            throw response;
        }
    }

    const { data: garageServiceTypes, isLoading, isError, refetch } = useQuery(
        ['garageServices-ForLogs', licensePlate],
        fetchGarageServiceTypes,
        {
            enabled: licensePlate?.length > 0,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000, // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const triggerFetch = (newLicensePlate: string) => {
        setLicensePlate(newLicensePlate);
    }

    useEffect(() => {
        if (licensePlate.length > 0) {
            refetch();
        }
    }, [licensePlate, refetch]);

    return {
        loading: isLoading,
        isError,
        garageServiceTypes: garageServiceTypes,
        triggerFetch
    };
}
