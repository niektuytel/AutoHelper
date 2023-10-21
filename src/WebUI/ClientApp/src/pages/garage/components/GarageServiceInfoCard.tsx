﻿import React, { useState } from "react";
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
import AddBoxOutlinedIcon from '@mui/icons-material/AddBoxOutlined';
import { useTranslation } from "react-i18next";
import AddIcon from '@mui/icons-material/Add';
import LiveHelpIcon from '@mui/icons-material/LiveHelp';
import HelpCenterOutlinedIcon from '@mui/icons-material/HelpCenterOutlined';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import EuroIcon from '@mui/icons-material/Euro';
import { GarageServiceItemDto, GarageServiceType } from "../../../app/web-api-client";
import { COLORS } from "../../../constants/colors";
import { getDefaultCreateGarageServices, getDefaultGarageServicesInfo, getTitleForServiceType } from "../../garage-account/defaultGarageService";

// own imports

interface IProps {
    serviceType: GarageServiceType;
    selectedItem: any | null;
    setSelectedItem: (serviceType: any) => void;
    addCartItem: (serviceType: any) => void;
    hasQuestionItem: (serviceType: GarageServiceType) => void;
}

export default ({ serviceType, selectedItem, setSelectedItem, addCartItem, hasQuestionItem }: IProps) => {
    const { t } = useTranslation('serviceTypes');

    const defaultAvailableServices = getDefaultGarageServicesInfo(t);
    const service = defaultAvailableServices.find(item => item.type === serviceType) as any;

    if (!service) {
        return <></>;
    }

    return <>
        <Card
            style={{
                marginBottom: "10px",
                padding: "8px",
                border: selectedItem === service ? `1px solid black` : `1px solid ${COLORS.BORDER_GRAY}`
            }}
            onClick={() => setSelectedItem(service)}
        >
            <CardHeader
                action={
                    <>
                        <IconButton
                            onClick={(e) => {
                                e.stopPropagation();
                                hasQuestionItem(serviceType);
                            }}
                        >
                            <HelpCenterOutlinedIcon sx={{ color: "black" }} />
                        </IconButton>
                        <IconButton
                            onClick={(e) => {
                                e.stopPropagation();
                                addCartItem(service);
                            }}
                        >
                            <AddBoxOutlinedIcon sx={{ color: "black"}} />
                        </IconButton>
                    </>
                }
                title={service.title}
                titleTypographyProps={{ variant: "body1" }}
                style={{ paddingBottom: "4px", paddingTop: "4px", paddingLeft: "4px" }}
            />
            <CardContent style={{ paddingTop: "4px", paddingBottom: "4px", paddingLeft: "4px" }}>
                <Typography variant="caption" color="textSecondary">
                    {service.description}
                </Typography>
            </CardContent>
        </Card>
    </>;
}
