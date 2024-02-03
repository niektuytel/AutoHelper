import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "react-query";
import { Dispatch } from "react";
import { FieldValues, UseFormReset, UseFormSetError } from "react-hook-form";
import { TFunction } from "i18next";
import { GarageLocationDtoItem, CreateGarageCommand, GarageAccountClient, GarageSettingsDtoItem, UpdateGarageSettingsCommand, BadRequestResponse } from "../../../app/web-api-client";
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";
import { ROUTES } from "../../../constants/routes";
import { GetGarageAccountClient, useHandleApiRequest } from "../../../app/GarageClient";
import useUserRole from "../../../hooks/useUserRole";
import useRoleIndex from "../../../hooks/useRoleIndex";

//own imports

function useGarageSettings(reset: UseFormReset<FieldValues>, setError: UseFormSetError<FieldValues>, notFound: boolean) {
    const { userRole } = useUserRole()
    const { setConfigurationIndex } = useRoleIndex();
    const queryClient = useQueryClient();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();
    const garageClient = GetGarageAccountClient();
    const handleApiRequest = useHandleApiRequest<GarageSettingsDtoItem>();

    const fetchGarageData = async () => {
        const response = await handleApiRequest(
            async () => {
                const result = await garageClient.getSettings();

                setConfigurationIndex(2, userRole);
                return result;
            },
            t("GarageClient.404.Message")
        );

        return response;
    }

    const { data: garageSettings, isLoading, isError } = useQuery(
        ['garageSettings'],
        fetchGarageData,
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour

            onSuccess: (data: any) => {
                if(data == null) return;

                reset({
                    name: data.name,
                    address: data.address ? `${data.address}, ${data.city}` : '',
                    city: data.city,
                    image: data.image,
                    imageThumbnail: data.imageThumbnail,
                    phoneNumber: data.phoneNumber,
                    whatsappNumber: data.whatsappNumber,
                    emailAddress: data.emailAddress,
                    conversationContactEmail: data.conversationContactEmail,
                    conversationContactWhatsappNumber: data.conversationContactWhatsappNumber,
                    website: data.website
                });

                // Enable garage overview + services pages
                setConfigurationIndex(2, userRole);
            }
        }
    );

    const createMutation = useMutation(garageClient.createGarage.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage has been created!"));

            // Update the garageSettings in the cache after creating
            queryClient.setQueryData(['garageSettings'], response);

            // Enable garage overview + services pages
            setConfigurationIndex(2, userRole);
            navigate(ROUTES.GARAGE_ACCOUNT.SERVICES);
        },
        onError: (response) => {
            console.error('Error:', response);

            // Display specific error message from server response
            if (response instanceof BadRequestResponse && response.errors) {
                dispatch(showOnError(Object.entries(response.errors)[0][1]));
            }
        }
    });

    const updateMutation = useMutation(garageClient.updateSettings.bind(garageClient), {
        onSuccess: (response) => {
            dispatch(showOnSuccess("Garage has been updated!"));

            // Update the garageSettings in the cache after updating
            queryClient.setQueryData(['garageSettings'], response);

            // Enable garage overview + services pages
            setConfigurationIndex(2, userRole);
        },
        onError: (response) => {
            console.error('Error:', response);
            // Display specific error message from server response
            if (response instanceof BadRequestResponse && response.errors) {
                const errors = Object.entries(response.errors);
                errors.forEach((error) => {
                    let key = error[0];
                    key = key.charAt(0).toLowerCase() + key.slice(1);

                    const value = error[1];
                    setError(key, {
                        type: "manual",
                        message: t(value as string)
                    });
                });
            }
        }
    });

    const createGarage = (data: any) => {
        var command = new CreateGarageCommand();
        command.garageLookupIdentifier = data.garageLookup.identifier;
        command.website = data.website;
        command.phoneNumber = data.phoneNumber;
        command.whatsappNumber = data.whatsappNumber;
        command.emailAddress = data.emailAddress;
        command.conversationEmail = data.conversationEmail;
        command.conversationWhatsappNumber = data.conversationWhatsappNumber;

        const cleanAddress = data.address.replace(`, ${data.city}`, '');
        command.location = new GarageLocationDtoItem({
            address: cleanAddress,
            city: data.city,
            longitude: data.longitude,
            latitude: data.latitude
        });

        console.log(command.toJSON());
        createMutation.mutate(command);
    }

    const updateGarageSettings = (data: any) => {

        var command = new UpdateGarageSettingsCommand();
        command.name = data.name;
        command.website = data.website;
        command.phoneNumber = data.phoneNumber;
        command.whatsappNumber = data.whatsappNumber;
        command.emailAddress = data.emailAddress;
        command.conversationEmail = data.conversationEmail;
        command.conversationWhatsappNumber = data.conversationWhatsappNumber;

        if (data?.longitude) {
            const cleanAddress = data.address.replace(`, ${data.city}`, '');
            command.location = new GarageLocationDtoItem({
                address: cleanAddress,
                city: data.city,
                longitude: data.longitude,
                latitude: data.latitude
            });
        }

        console.log(command.toJSON());
        updateMutation.mutate(command);
    }

    // only reset the form when the data is loaded
    const loading = isLoading || createMutation.isLoading || updateMutation.isLoading;
    useEffect(() => {
        if (garageSettings && !loading && !updateMutation.isError && !createMutation.isError) {
            console.log("reset form with data garageSettings: ", garageSettings);
            reset({
                name: garageSettings.name,
                image: garageSettings.image,
                imageThumbnail: garageSettings.imageThumbnail,
                emailAddress: garageSettings.emailAddress,
                phoneNumber: garageSettings.phoneNumber,
                whatsappNumber: garageSettings.whatsappNumber,
                address: garageSettings.address ? `${garageSettings.address}, ${garageSettings.city}` : '',
                city: garageSettings.city,
                website: garageSettings.website,
                conversationEmail: garageSettings.conversationContactEmail,
                conversationWhatsappNumber: garageSettings.conversationContactWhatsappNumber,
            });
        }
    }, [garageSettings, loading, reset]);

    return {
        loading, isError, garageSettings, createGarage, updateGarageSettings
    }
}

export default useGarageSettings;
