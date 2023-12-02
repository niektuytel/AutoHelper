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
    return <>
    </>;
                    //<Box key={`serviceLog-${index}`}>
                    //    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', m: 1 }}>
                    //        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    //            <Typography variant="subtitle1" color="text.secondary">
                    //                {logItem.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                    //            </Typography>
                    //            <Typography sx={{ mx: 1 }}> - </Typography>
                    //            <Typography variant="h6" sx={textStyles.root}>
                    //                <b>{getServiceTypeLabel(logItem.type!)}</b>
                    //            </Typography>
                    //        </Box>
                    //        <Chip
                    //            label="Unverified"
                    //            color="warning"
                    //            variant="outlined"
                    //            sx={{ ml: 'auto' }}
                    //        />
                    //    </Box>
                    //    {logItem.description && (
                    //        <Typography variant="body2" sx={{ mx: 1 }}>
                    //            {logItem.description}
                    //        </Typography>
                    //    )}
                    //    <Box sx={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center', m: 1 }}>
                    //        <Chip
                    //            icon={<SpeedIcon />}
                    //            label={`${logItem.odometerReading!.toLocaleString()} km`}
                    //            variant="outlined"
                    //            sx={{ mr: 1, my: 0.5 }}
                    //        />
                    //        <Chip
                    //            icon={<GarageIcon />}
                    //            label={logItem.garageLookupName}
                    //            variant="outlined"
                    //            onClick={() => navigate(`${ROUTES.GARAGE}/${logItem.garageLookupIdentifier}?licensePlate=${license_plate}`)}
                    //            sx={{ mr: 1, my: 0.5 }}
                    //        />
                    //        {logItem.attachedFile && (
                    //            <Chip
                    //                icon={<AttachFileIcon />}
                    //                label={"Bijlage"}
                    //                variant="outlined"
                    //                onClick={() => window.open(logItem.attachedFile, '_blank')}
                    //                sx={{ mr: 1, my: 0.5 }}
                    //            />
                    //        )}
                    //    </Box>
                    //    <Divider sx={{ mt: 1 }} />
                    //</Box>
        //<Card
        //    style={{
        //        marginBottom: "10px",
        //        padding: "8px",
        //        cursor: "pointer",
        //        border: selectedItem === service ? `1px solid black` : `1px solid ${COLORS.BORDER_GRAY}`
        //    }}
        //    title={service.description}
        //    onClick={() => setSelectedItem(service)}
        //>
        //    <CardHeader
        //        title={title}
        //        titleTypographyProps={{ variant: "body1" }}
        //        style={{ paddingBottom: "4px", paddingTop: "4px", paddingLeft: "4px" }}
        //    />
        //</Card>
}
