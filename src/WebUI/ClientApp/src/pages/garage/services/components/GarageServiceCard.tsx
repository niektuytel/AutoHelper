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

// own imports


interface IProps {
    addCartItem: (service: CreateGarageServiceCommand) => void;
}

export default ({ addCartItem }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [timeUnit, setTimeUnit] = useState("minutes");


    const [otherServiceDescription, setOtherServiceDescription] = useState("");
    const [otherServiceDuration, setOtherServiceDuration] = useState(0);
    const [otherServicePrice, setOtherServicePrice] = useState(0);

    const convertDurationToMinutes = () => {
        switch (timeUnit) {
            case 'hours': return otherServiceDuration * 60;
            case 'days': return otherServiceDuration * 24 * 60;
            default: return otherServiceDuration;
        }
    };

    const handleAddItem = () => {
        addCartItem(new CreateGarageServiceCommand({
            type: GarageServiceType.Other,
            description: otherServiceDescription,
            durationInMinutes: convertDurationToMinutes(),
            price: otherServicePrice
        }));
    };

    return (
        <>
            <Card style={{
                marginBottom: "10px",
                border: `1px solid ${COLORS.BORDER_GRAY}`
            }}>
                <CardHeader
                    action={
                        <IconButton
                            onClick={handleAddItem}
                            style={{ marginTop: "5px", marginLeft: "5px", marginRight: "5px" }}
                        >
                            <AddIcon />
                        </IconButton>
                    }
                    title={
                        <TextField
                            fullWidth
                            size="small"
                            value={otherServiceDescription}
                            onChange={(e) => setOtherServiceDescription(e.target.value)}
                        />
                    }
                />
                <CardContent>
                    {isMobile ?
                        <Box width="100%">
                            <TextField
                                label={t("Duration")}
                                fullWidth
                                size="small"
                                type="number"
                                inputProps={{ min: 0 }}
                                onChange={(e) => setOtherServiceDuration(Number(e.target.value))}
                                variant="outlined"
                                margin="normal"
                                InputProps={{
                                    startAdornment: (
                                        <InputAdornment position="start">
                                            <AccessTimeIcon />
                                        </InputAdornment>
                                    ),
                                    endAdornment: (
                                        <InputAdornment position="end">
                                            <Select
                                                value={timeUnit}
                                                onChange={(e) => setTimeUnit(e.target.value)}
                                                sx={{
                                                    minWidth: "100%",
                                                    fontSize: '0.8rem',
                                                    border: 'none',
                                                    boxShadow: 'none',
                                                    '&:focus': {
                                                        border: 'none',
                                                        boxShadow: 'none',
                                                        outline: 'none',   // Remove the outline when focused
                                                    },
                                                    '& .MuiOutlinedInput-notchedOutline': {   // Remove the outline for the outlined variant
                                                        border: 'none',
                                                    },
                                                    '&:hover .MuiOutlinedInput-notchedOutline': {   // Remove the outline when hovered
                                                        border: 'none',
                                                    },
                                                }}
                                                size="small"
                                            >
                                                <MenuItem value="minutes">{t("minutes")}</MenuItem>
                                                <MenuItem value="hours">{t("hours")}</MenuItem>
                                                <MenuItem value="days">{t("days")}</MenuItem>
                                            </Select>
                                        </InputAdornment>
                                    ),
                                    style: { paddingRight: '0' } // Reducing the padding to give more space
                                }}
                            />
                            <TextField
                                label={t("Price")}
                                onChange={(e) => setOtherServicePrice(Number(e.target.value))}
                                fullWidth
                                size="small"
                                type="number"
                                inputProps={{ step: '0.01' }}
                                variant="outlined"
                                margin="normal"
                                InputProps={{
                                    startAdornment: (
                                        <InputAdornment position="start">
                                            €
                                        </InputAdornment>
                                    ),
                                }}
                            />
                        </Box>
                        :
                        <Box display="flex" justifyContent="space-between" alignItems="center" width="100%">
                            {/* Duration Input */}
                            <Box display="flex" alignItems="center" flexGrow={1} marginRight={2}>
                                <TextField
                                    label={t("Duration")}
                                    fullWidth
                                    size="small"
                                    type="number"
                                    inputProps={{ min: 0 }}
                                    onChange={(e) => setOtherServiceDuration(Number(e.target.value))}
                                    variant="outlined"
                                    margin="none"
                                    InputProps={{
                                        startAdornment: (
                                            <InputAdornment position="start">
                                                <AccessTimeIcon />
                                            </InputAdornment>
                                        ),
                                        endAdornment: (
                                            <InputAdornment position="end">
                                                <Select
                                                    value={timeUnit}
                                                    onChange={(e) => setTimeUnit(e.target.value)}
                                                    sx={{
                                                        minWidth: "100%",
                                                        fontSize: '0.8rem',
                                                        border: 'none',
                                                        boxShadow: 'none',
                                                        '&:focus': {
                                                            border: 'none',
                                                            boxShadow: 'none',
                                                            outline: 'none',   // Remove the outline when focused
                                                        },
                                                        '& .MuiOutlinedInput-notchedOutline': {   // Remove the outline for the outlined variant
                                                            border: 'none',
                                                        },
                                                        '&:hover .MuiOutlinedInput-notchedOutline': {   // Remove the outline when hovered
                                                            border: 'none',
                                                        },
                                                    }}
                                                    size="small"
                                                >
                                                    <MenuItem value="minutes">{t("minutes")}</MenuItem>
                                                    <MenuItem value="hours">{t("hours")}</MenuItem>
                                                    <MenuItem value="days">{t("days")}</MenuItem>
                                                </Select>
                                            </InputAdornment>
                                        ),
                                        style: { paddingRight: '0' } // Reducing the padding to give more space
                                    }}
                                />
                            </Box>

                            {/* Price Input */}
                            <Box display="flex" alignItems="center" flexGrow={1}>
                                <TextField
                                    label={t("Price")}
                                    onChange={(e) => setOtherServicePrice(Number(e.target.value))}
                                    fullWidth
                                    size="small"
                                    type="number"
                                    inputProps={{ step: '0.01' }}
                                    variant="outlined"
                                    margin="none"
                                    InputProps={{
                                        startAdornment: (
                                            <InputAdornment position="start">
                                                €
                                            </InputAdornment>
                                        ),
                                    }}
                                />
                            </Box>
                        </Box>
                    }
                </CardContent>
            </Card>
        </>
    );
}
