import React, { useState } from "react";
import {
    Box,
    Button,
    Divider,
    IconButton,
    Tooltip,
    Typography,
    TextField,
    Card,
    CardHeader,
    CardContent,
    CardActions,
    CircularProgress,
    useTheme,
    useMediaQuery,
    Drawer,
    ButtonGroup,
    InputAdornment,
    Select,
    MenuItem,
} from "@mui/material";
import { useTranslation } from "react-i18next";
import AddIcon from '@mui/icons-material/Add';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import EuroIcon from '@mui/icons-material/Euro';
import { GarageServiceType, VehicleServiceLogAsGarageDtoItem } from "../../../../app/web-api-client";
import { COLORS } from "../../../../constants/colors";
import { getTitleForServiceType } from "../../defaultGarageService";

// own imports

type ServiceProps = {
    durationInMinutes?: number;
};

const DurationDisplay: React.FC<ServiceProps> = ({ durationInMinutes = 0 }) => {
    const { t } = useTranslation();
    const hours = Math.floor(durationInMinutes / 60);
    const minutes = durationInMinutes % 60;

    return (
        <Typography variant="caption" color="textSecondary" style={{ marginLeft: "8px" }}>
            {hours > 0 && `${hours} ${t('hours')}${minutes > 0 ? ',' : ''} `}
            {minutes > 0 && `${minutes} ${t('minutes')}`}
        </Typography>
    );
};

interface IProps {
    service: VehicleServiceLogAsGarageDtoItem;
    selectedItem: VehicleServiceLogAsGarageDtoItem;
    setSelectedItem: (service: VehicleServiceLogAsGarageDtoItem) => void;
    addCartItem: (service: VehicleServiceLogAsGarageDtoItem) => void;
}

export default ({ service, selectedItem, setSelectedItem, addCartItem }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();

    const title = getTitleForServiceType(
        t,
        service.type ? service.type : GarageServiceType.Other,
        service.description
    );

    // INFO: This is an feature that is not yet implemented
    // To make it possible to use service to create an order.
    // Then on the confirm button they can send an tikkie
    // action={
    //
    //    <IconButton
    //        onClick={(e) => {
    //            e.stopPropagation();
    //            addCartItem(service);
    //        }}
    //    >
    //        <AddIcon />
    //    </IconButton>
    // }
    return (
        <Card
            style={{
                marginBottom: "10px",
                padding: "8px",
                cursor: "pointer",
                border: selectedItem === service ? `1px solid black` : `1px solid ${COLORS.BORDER_GRAY}`
            }}
            title={service.description}
            onClick={() => setSelectedItem(service)}
        >
            <CardHeader
                title={title}
                titleTypographyProps={{ variant: "body1" }}
                style={{ paddingBottom: "4px", paddingTop: "4px", paddingLeft: "4px" }}
            />
        </Card>
    );
}
