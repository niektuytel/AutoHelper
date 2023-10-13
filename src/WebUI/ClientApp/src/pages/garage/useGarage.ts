import { useQuery, useQueryClient } from "react-query";
import { useState } from "react";

//own imports
import { GarageClient } from "../../app/web-api-client";

function useGarage(
    identifier: string,
    licensePlate: string | null
) {
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();

    const fetchGarageLookup = async (licensePlate: string | null) => {
        try {
            const response = await garageClient.getLookup(identifier, licensePlate);
            return response;
        } catch (response: any) {
            throw response;
        }
    }

    const { data: garageLookup, isLoading, isError } = useQuery(
        [`garageLookup-${identifier}-${licensePlate}`],
        () => fetchGarageLookup(licensePlate),
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const fetchGarageLookupByPlate = async (licensePlate: string) => {
        // Check if data exists in cache first
        const cachedData = queryClient.getQueryData(
            [`garageLookup-${identifier}-${licensePlate}`]
        );

        if (cachedData) {
            return cachedData;
        }

        return fetchGarageLookup(licensePlate);
    }


    // only reset the form when the data is loaded
    const loading = isLoading;
    return {
        loading, isError, garageLookup, fetchGarageLookupByPlate
    }
}

export default useGarage;
