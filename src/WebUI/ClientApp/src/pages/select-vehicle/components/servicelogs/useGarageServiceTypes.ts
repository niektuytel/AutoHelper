import { useQuery, useQueryClient } from "react-query";
import { useState, useEffect } from "react";
import { GarageClient } from "../../../../app/web-api-client";

export default ( licensePlate: string) => {
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const [garageIdentifier, setGarageIdentifier] = useState("");

    const fetchGarageServiceTypes = async () => {
        try {
            const response = await garageClient.getServices(garageIdentifier, licensePlate);
            return response;
        } catch (response: any) {
            throw response;
        }
    }

    const { data: garageServiceTypes, isLoading, isError, refetch } = useQuery(
        [`garageServiceTypes-${licensePlate}`, garageIdentifier],
        fetchGarageServiceTypes,
        {
            enabled: garageIdentifier?.length > 0,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000, // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const triggerFetch = (identifier: string) => {
        setGarageIdentifier(identifier);
    }

    useEffect(() => {
        if (garageIdentifier.length > 0) {
            refetch();
        }
    }, [garageIdentifier, refetch]);

    return {
        loading: isLoading,
        isError,
        garageServiceTypes: garageServiceTypes,
        triggerFetch
    };
}
