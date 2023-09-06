import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "react-query";
import { Dispatch } from "react";
import { FieldValues, UseFormReset, UseFormSetError } from "react-hook-form";
import { TFunction } from "i18next";
import { BriefBankingDetailsDto, BriefLocationDto, CreateGarageServiceCommand, GarageBankingDetailsItem, GarageClient, GarageLocationItem, GarageSettings, UpdateGarageServiceCommand } from "../../../app/web-api-client";
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { RoutesGarageSettings } from "../../../constants/routes";

//own imports

function useGarageServices(garage_guid?: string) {
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const fetchGarageServicesData = async () => {
        try {
            if (!garage_guid) {
                dispatch(showOnError(t("Garage not found!")));
                navigate(`${RoutesGarageSettings(garage_guid!)}?garage_notfound=true`);
                return [];
            }

            const response = await garageClient.getServices(garage_guid!);

            return response;
        } catch (response: any) {
            if (response && response.status === 404) {
                dispatch(showOnError(t("Garage not found!")));
                navigate(`${RoutesGarageSettings(garage_guid!)}?garage_notfound=true`);
                return [];
            } else {
                throw response;
            }
        }
    }

    const { data: garageServices, isLoading, isError } = useQuery(
        ['garageServices', garage_guid],
        fetchGarageServicesData,
        {
            enabled: garage_guid != undefined,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const createMutation = useMutation(garageClient.createService.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service has been created!"));

            // Update the garageSettings in the cache after creating
            queryClient.setQueryData(['garageServices', garage_guid], [...garageServices!, response]);
        },
        onError: (response) => {
            console.error(response)
            //guardHttpResponse(response, setError, t, dispatch);
        }
    });

    const updateMutation = useMutation(garageClient.updateService.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage service has been updated!"));

            // Update the garageSettings in the cache after updating
            const updatedGarageServices = garageServices?.map((service) => {
                if (service.id === response.id) {
                    return response;
                } else {
                    return service;
                }
            });

            queryClient.setQueryData(['garageServices', garage_guid], updatedGarageServices);
        },
        onError: (response) => {
            console.error(response)
            //guardHttpResponse(response, setError, t, dispatch);
        }
    });

    const createService = (data: any) => {
        var command = new CreateGarageServiceCommand();
        command.title = data.title;
        command.description = data.description;
        command.price = data.price;
        command.duration = data.duration;

        console.log(command.toJSON());
        createMutation.mutate(command);
    }

    const updateService = (data: any) => {
        var command = new UpdateGarageServiceCommand();
        command.id = data.id;
        command.garageId = garage_guid;
        command.title = data.title;
        command.description = data.description;
        command.price = data.price;
        command.duration = data.duration;

        console.log(command.toJSON());
        updateMutation.mutate(command);
    }

    // only reset the form when the data is loaded
    const loading = isLoading || createMutation.isLoading || updateMutation.isLoading;
    return {
        loading, isError, garageServices, createService, updateService
    }
}

export default useGarageServices;
