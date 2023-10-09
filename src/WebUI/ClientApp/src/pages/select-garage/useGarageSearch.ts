import { useQuery } from "react-query";
import { useState } from "react";

import {
    GarageSearchClient,
    PaginatedListOfGarageLookupDto,
} from "../../app/web-api-client";

function useGarageSearch(
    licensePlate: string,
    latitude: number,
    longitude: number,
    inMeterRange: number,
    pageNumber: number,
    pageSize: number
) {
    const garageSearchClient = new GarageSearchClient(process.env.PUBLIC_URL);

    const [localGarages, setLocalGarages] = useState<PaginatedListOfGarageLookupDto>(
        new PaginatedListOfGarageLookupDto()
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
        const response = await garageSearchClient.searchGarages(
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
        ['fetchGaragesData', licensePlate, latitude, longitude, pageNumber], // unique query key
        () => fetchGaragesData(licensePlate, latitude, longitude, inMeterRange, pageNumber, pageSize),
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
