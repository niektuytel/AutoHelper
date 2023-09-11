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
import { CreateGarageServiceCommand, GarageServiceType } from "../../../../app/web-api-client";
import { COLORS } from "../../../../constants/colors";
import { getTitleForServiceType } from "../defaultGarageService";

// own imports


interface IProps {
    service: CreateGarageServiceCommand;
    selectedItem: CreateGarageServiceCommand;
    setSelectedItem: (service: CreateGarageServiceCommand) => void;
    addCartItem: (service: CreateGarageServiceCommand) => void;
}

export default ({ service, selectedItem, setSelectedItem, addCartItem }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    //const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    //const [timeUnit, setTimeUnit] = useState("minutes");


    //const [otherServiceDescription, setOtherServiceDescription] = useState("");
    //const [otherServiceDuration, setOtherServiceDuration] = useState(0);
    //const [otherServicePrice, setOtherServicePrice] = useState(0);

    //const convertDurationToMinutes = () => {
    //    switch (timeUnit) {
    //        case 'hours': return service.durationInMinutes / 60;
    //        case 'days': return service.durationInMinutes / (24 * 60);
    //        default: return service.durationInMinutes;
    //    }
    //};

    //const handleAddItem = () => {
    //    addCartItem(new CreateGarageServiceCommand({
    //        type: GarageServiceType.Other,
    //        description: otherServiceDescription,
    //        durationInMinutes: convertDurationToMinutes(),
    //        price: otherServicePrice
    //    }));
    //};

    const title = getTitleForServiceType(t, service.type!, service.description);

    return (
        <Card
            key={`service-card-${title}`}
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
                    <Typography variant="caption" color="textSecondary" style={{ marginLeft: "8px" }}>
                        {service.durationInMinutes}
                    </Typography>
                </Box>
                <Box display="flex" alignItems="center" style={{ marginRight: "10px" }} >
                    <EuroIcon color="action" fontSize="small" style={{ marginRight: "5px" }} />
                    <Typography variant="body2" align="right">
                        {service.price}
                    </Typography>
                </Box>
            </CardActions>
        </Card>
    );
}
