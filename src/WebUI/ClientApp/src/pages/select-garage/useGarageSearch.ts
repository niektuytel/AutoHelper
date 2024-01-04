import { useQuery } from "react-query";
import { useState } from "react";

import {
    GarageClient,
    PaginatedListOfGarageLookupBriefDto,
} from "../../app/web-api-client";

function useGarageSearch(
    licensePlate: string,
    latitude: number,
    longitude: number,
    inMeterRange: number,
    pageNumber: number,
    pageSize: number,
    autoCompleteGarageName: string | null = null,
    filters: string[] | null = null
) {
    const garageSearchClient = new GarageClient(process.env.PUBLIC_URL);

    const [localGarages, setLocalGarages] = useState<PaginatedListOfGarageLookupBriefDto>(
        new PaginatedListOfGarageLookupBriefDto()
    );

    const fetchGaragesData = async (
        license_plate: string,
        latitude: number,
        longitude: number,
        in_meter_range: number,
        page_number: number,
        page_size: number,
        autoCompleteGarageName: string | null = null,
        filters: string[] | null = null
    ) => {
        const response = await garageSearchClient.searchLookups(
            license_plate,
            latitude,
            longitude,
            in_meter_range,
            page_number,
            page_size,
            autoCompleteGarageName,
            filters
        );

        setLocalGarages(response);

        return response;
    };

    const { isLoading, isError } = useQuery(
        ['fetchGaragesData', licensePlate, latitude, longitude, pageNumber, autoCompleteGarageName, filters], // unique query key
        () => fetchGaragesData(licensePlate, latitude, longitude, inMeterRange, pageNumber, pageSize, autoCompleteGarageName, filters),
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
        }
    );

    return {
        loading: isLoading,
        isError,
        garages: localGarages,
        fetchGarages: fetchGaragesData,
        setGaragesData: setLocalGarages,
    };
}

export default useGarageSearch;
