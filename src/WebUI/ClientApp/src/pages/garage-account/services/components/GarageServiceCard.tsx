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
import { GarageServiceType, GarageServiceDtoItem } from "../../../../app/web-api-client";
import { COLORS } from "../../../../constants/colors";
import { getTitleForServiceType } from "../../defaultGarageService";

// own imports

type ServiceProps = {
    durationInMinutes?: number;
};

interface IProps {
    service: GarageServiceDtoItem;
    selectedItem: GarageServiceDtoItem;
    setSelectedItem: (service: GarageServiceDtoItem) => void;
    addCartItem: (service: GarageServiceDtoItem) => void;
}

export default ({ service, selectedItem, setSelectedItem, addCartItem }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();

    const title = getTitleForServiceType(
        t,
        service.type ? service.type : GarageServiceType.Other,
        service.description
    );

    // TODO: This is an feature that is not yet implemented
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
            <CardContent style={{ paddingTop: "4px", paddingBottom: "4px", paddingLeft: "4px" }}>
                <Box display="flex" alignItems="center">
                    {service.description}
                </Box>
            </CardContent>

        </Card>
    );
}
