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
import { GarageServiceType, GarageServiceItemDto } from "../../../../app/web-api-client";
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
    service: GarageServiceItemDto;
    selectedItem: GarageServiceItemDto;
    setSelectedItem: (service: GarageServiceItemDto) => void;
    addCartItem: (service: GarageServiceItemDto) => void;
}

export default ({ service, selectedItem, setSelectedItem, addCartItem }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();

    const title = getTitleForServiceType(
        t,
        service.type ? service.type : GarageServiceType.Other,
        service.description
    );

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
                action={
                    <IconButton
                        onClick={(e) => {
                            e.stopPropagation();
                            addCartItem(service);
                        }}
                    >
                        <AddIcon />
                    </IconButton>
                }
                title={title}
                titleTypographyProps={{ variant: "body1" }}
                style={{ paddingBottom: "4px", paddingTop: "4px", paddingLeft: "4px" }}
            />
            <CardActions style={{ padding: "0", justifyContent: "space-between" }}>
                <Box display="flex" alignItems="center">
                    <AccessTimeIcon color="action" fontSize="small" />
                    <DurationDisplay durationInMinutes={service.durationInMinutes} />
                </Box>
                <Box display="flex" alignItems="center" style={{ marginRight: "10px" }} >
                    <Typography variant="body2" align="right">
                        €{Number(service.price).toFixed(2)}
                    </Typography>
                </Box>
            </CardActions>
        </Card>
    );
}
