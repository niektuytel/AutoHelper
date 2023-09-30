import { useQuery, useMutation, useQueryClient } from "react-query";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

//own imports
import { GarageSearchClient, VehicleClient } from "../../app/web-api-client";

function useGarageSearch(
    licensePlate: string,
    latitude: number,
    longitude: number,
    inKmRange: number,
    pageNumber: number,
    pageSize: number
) {
    const useGarageSearchClient = new GarageSearchClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchGaragesData = async (
        license_plate: string,
        latitude: number,
        longitude: number,
        in_km_range: number,
        page_number: number,
        page_size: number
    ) => {
        try {
            const response = await useGarageSearchClient.searchGarages(
                license_plate,
                latitude,
                longitude,
                in_km_range,
                page_number,
                page_size
            );
            return response;
        } catch (response: any) {
            throw response;
        }
    }

    const { data: garages, isLoading, isError } = useQuery(
        [`garagesSearch-${licensePlate}-${latitude}-${longitude}`],
        () => fetchGaragesData(
            licensePlate,
            latitude,
            longitude,
            inKmRange,
            pageNumber,
            pageSize
        ),
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const fetchGarages = async (
        licensePlate: string,
        latitude: number,
        longitude: number,
        inKmRange: number,
        pageNumber: number,
        pageSize: number
    ) => {
        // Check if data exists in cache first
        const cachedData = queryClient.getQueryData(
            [`garagesSearch-${licensePlate}-${latitude}-${longitude}`],
        );

        if (cachedData) {
            return cachedData;
        }

        return fetchGaragesData(
            licensePlate,
            latitude,
            longitude,
            inKmRange,
            pageNumber,
            pageSize
        );
    }


    // only reset the form when the data is loaded
    const loading = isLoading;
    return {
        loading, isError, garages, fetchGarages
    }
}

export default useGarageSearch;
