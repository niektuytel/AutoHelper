import { TFunction, useTranslation } from "react-i18next";
import { CreateGarageServiceCommand, GarageServiceType } from "../../app/web-api-client";


export function getTitleForServiceType(t: TFunction, serviceType: GarageServiceType, altValue: string = "")
{
    switch (serviceType)
    {
        case GarageServiceType.Inspection:
            return t("Inspection");
        case GarageServiceType.SmallMaintenance:
            return t("SmallMaintenance");
        case GarageServiceType.GreatMaintenance:
            return t("GreatMaintenance");
        case GarageServiceType.AirConditioningMaintenance:
            return t("AirConditioningMaintenance");
        case GarageServiceType.SeasonalTireChange:
            return t("SeasonalTireChange");
        case GarageServiceType.MOTService:
            return t("MOTService");
    }

    if (altValue == "" && serviceType == GarageServiceType.Other) {
        return t("Other");
    }

    return altValue;
}

export function getDefaultCreateGarageServices(t: TFunction) {
    const defaultServices: CreateGarageServiceCommand[] = [
        new CreateGarageServiceCommand({
            type: GarageServiceType.MOTService,
            description: t("MOTService.Description"),
            durationInMinutes: 180,
            price: 30.00
        }),
        new CreateGarageServiceCommand({
            type: GarageServiceType.Inspection,
            description: t("Inspection.Description"),
            durationInMinutes: 25,
            price: 35.00
        }),
        new CreateGarageServiceCommand({
            type: GarageServiceType.SmallMaintenance,
            description: t("SmallMaintenance.Description"),
            durationInMinutes: 45,
            price: 125.00
        }),
        new CreateGarageServiceCommand({
            type: GarageServiceType.GreatMaintenance,
            description: t("GreatMaintenance.Description"),
            durationInMinutes: 360,
            price: 300.00
        }),
        new CreateGarageServiceCommand({
            type: GarageServiceType.AirConditioningMaintenance,
            description: t("AirConditioningMaintenance.Description"),
            durationInMinutes: 90,
            price: 200.00
        }),
        new CreateGarageServiceCommand({ 
            type: GarageServiceType.SeasonalTireChange, 
            description: t("SeasonalTireChange.Description"), 
            durationInMinutes: 60, 
            price: 115.00
        })
    ];

    return defaultServices;
}