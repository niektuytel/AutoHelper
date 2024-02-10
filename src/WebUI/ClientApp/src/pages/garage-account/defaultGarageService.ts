import { TFunction, useTranslation } from "react-i18next";
import { enumToKeyValueArray } from "../../utils/utils";
import { CreateGarageServiceCommand, GarageServiceDtoItem, GarageServiceType, VehicleFuelType, VehicleType } from "../../app/web-api-client";

// Display all enum items except the first one (None)
export function getDefaultGarageServicesInfo(t: any) {
    return enumToKeyValueArray(GarageServiceType).slice(1).map(({ key, value }) => 
        ({
            type: key,
            title: t(`${value}.Title`),
            description: t(`${value}.Description`)
        })
    );
}

export function getAllGarageServiceTypes(t: any) {
    return enumToKeyValueArray(GarageServiceType).map(({ key, value }) =>
    ({
        type: key,
        title: t(`serviceTypes:${value}.Title`),
        description: t(`serviceTypes:${value}.Description`),
        filter: t(`serviceTypes:${value}.Filter`)
    })
    );
}

export function getAllVehicleType(t: any) {
    return enumToKeyValueArray(VehicleType).map(({ key, value }) =>
    ({
        type: key,
        title: t(`serviceTypes:${value}.Title`),
        description: t(`serviceTypes:${value}.Description`),
        filter: t(`serviceTypes:${value}.Filter`)
    })
    );
}

export function getAllVehicleFuelType(t: any) {
    return enumToKeyValueArray(VehicleFuelType).map(({ key, value }) =>
    ({
        type: key,
        title: t(`serviceFuelTypes:${value}.Title`),
        description: t(`serviceFuelTypes:${value}.Description`),
        filter: t(`serviceFuelTypes:${value}.Filter`)
    })
    );
}
