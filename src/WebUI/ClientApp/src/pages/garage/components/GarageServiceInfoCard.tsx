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
import HelpCenterOutlinedIcon from '@mui/icons-material/HelpCenterOutlined';
import { GarageServiceDtoItem, GarageServiceType, SelectedService, VehicleType } from "../../../app/web-api-client";
import { COLORS } from "../../../constants/colors";

// own imports

interface IProps {
    service: GarageServiceDtoItem;
    showConversationActions: boolean;
    selectedItem: any | null;
    setSelectedItem: (serviceType: any) => void;
    addCartItem: (selectedService: SelectedService) => void;
    hasQuestionItem: (serviceType: GarageServiceDtoItem) => void;
}

export default ({ service, showConversationActions, selectedItem, setSelectedItem, addCartItem, hasQuestionItem }: IProps) => {
    const { t } = useTranslation('serviceTypes');
    const theme = useTheme();

    const typeTitle = t(`serviceTypes:${GarageServiceType[service.type!]}.Title`);
    const vehicleTypeTitle = t(`serviceTypes:${VehicleType[service.vehicleType!]}.Title`);
    const title = service.title ? service.title : typeTitle;
    const description = service.description ? service.description : t(`serviceTypes:${GarageServiceType[service.type!]}.Description`);


    return <>
        <Card
            style={{
                marginBottom: "10px",
                padding: "8px",
                cursor: "pointer",
                border: selectedItem === service ? `1px solid black` : `1px solid ${COLORS.BORDER_GRAY}`
            }}
            onClick={() => setSelectedItem(service)}
        >
            <CardHeader
                action={ showConversationActions &&
                    <>
                        <IconButton
                            onClick={(e) => {
                                e.stopPropagation();
                                hasQuestionItem(service);
                            }}
                        >
                            <HelpCenterOutlinedIcon />
                        </IconButton>
                        <IconButton
                            onClick={(e) => {
                                e.stopPropagation();

                                const selectedService = new SelectedService({
                                    garageServiceId: service.id,
                                });

                                addCartItem(selectedService);
                            }}
                        >
                            <AddIcon sx={{ color: "black"}} />
                        </IconButton>
                    </>
                }
                title={`${title} (${vehicleTypeTitle})`}
                titleTypographyProps={{ variant: "body1", fontWeight: 'bold' }} 
                style={{ paddingBottom: "4px", paddingTop: "4px", paddingLeft: "4px" }}
            />
            <CardContent style={{ paddingTop: "4px", paddingBottom: "4px", paddingLeft: "4px" }}>
                <Box display="flex" alignItems="center">
                    {description}
                </Box>
            </CardContent>

        </Card>

    </>;
}