﻿import { useQuery } from "react-query";
import { useState } from "react";

import {
    GarageClient,
    PaginatedListOfGarageLookupBriefDto,
    VehicleClient,
} from "../../app/web-api-client";

function useGarageServiceTypes(licensePlate: string) {
    const garageSearchClient = new VehicleClient(process.env.PUBLIC_URL);

    const fetchGarageServiceTypes = async (licensePlate: string) => {
        try {
            const response = await garageSearchClient.getRelatedServices(licensePlate);
            return response;
        } catch (response: any) {
            throw response;
        }
    }

    const { data: garageServiceTypes, isLoading, isError } = useQuery(
        [`garageServiceTypes-${licensePlate}`],
        () => fetchGarageServiceTypes(licensePlate),
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    return {
        loading: isLoading,
        isError,
        garageServiceTypes: garageServiceTypes,
    };
}

export default useGarageServiceTypes;
