import React, { useState, MouseEvent, useEffect, useRef, RefObject } from "react";
import CloseIcon from '@mui/icons-material/Close';
import { useDispatch, useSelector } from "react-redux";
import { Avatar, Button, Card, CardContent, CardHeader, Container, Divider, Grid, Hidden, IconButton, List, ListItem, ListItemAvatar, ListItemSecondaryAction, ListItemText, Skeleton, Theme, Typography, useMediaQuery, useTheme } from "@mui/material";
import DeleteIcon from '@mui/icons-material/Delete';
import { useLocation, useNavigate } from "react-router";
import { useTranslation } from "react-i18next";

// own imports
import { ROUTES } from "../../../constants/routes";
import { VehicleService } from "../../../app/web-api-client";
import { removeService } from "../../../redux/slices/storedServicesSlice";
import GarageContactDialog from "../../GarageContactDialog";

export function useOutsideClick<T extends HTMLElement = HTMLElement>(
    ref: RefObject<T>,
    callback: () => void
): void {
    const handleClick = (e: any) => {
        if (ref.current && !ref.current.contains(e.target as Node)) {
            callback();
        }
    };

    useEffect(() => {
        document.addEventListener('mousedown', handleClick);
        return () => {
            document.removeEventListener('mousedown', handleClick);
        };
    }, [ref, callback]);
}

interface IProps {
    isCardVisible: boolean;
    services: VehicleService[];
    onClose: () => void;
}

export default ({ isCardVisible, services, onClose }: IProps) => {
    const cardRef = useRef<HTMLDivElement>(null);
    useOutsideClick(cardRef, onClose);
    const navigate = useNavigate();
    const location = useLocation();
    const dispatch = useDispatch();
    const { t } = useTranslation();
    const [dialogOpen, setDialogOpen] = useState(false);
    const [requestQuote, setRequestQuote] = useState(false);

    const handleServiceClick = (service: VehicleService) => {
        navigate(`${ROUTES.GARAGE}/${service.relatedGarageLookupIdentifier}?licensePlate=${service.vehicleLicensePlate}&lat=${service.vehicleLatitude}&lng=${service.vehicleLongitude}`, { state: { from: location } });
    };

    const handleServiceRemove = (event: React.MouseEvent<HTMLButtonElement>, service: VehicleService) => {
        event.stopPropagation();

        if (services.length == 1) {
            onClose();
        }

        dispatch(removeService(service));
    };


    const handleAskQuestionClick = () => {
        setRequestQuote(false);
        setDialogOpen(true);
    };

    const handleRequestQuoteClick = () => {
        setRequestQuote(true);
        setDialogOpen(true);
    };


    return (
        <>
            {isCardVisible && 
                <Card ref={cardRef} sx={{ maxWidth: 345, position: 'absolute', top: '100%', right: 0, zIndex: 2, m: 1 }}>
                    <CardHeader
                        title={t("Selected Services")}
                        action={
                            <IconButton onClick={onClose}>
                                <CloseIcon />
                            </IconButton>
                        }
                        titleTypographyProps={{ align: 'left' }}
                        sx={{ paddingBottom: 0 }}
                    />
                    <CardContent>
                        <List dense>
                            {services.map((service, index) => (
                                <React.Fragment key={`${service.garageServiceId}`}>
                                    {index > 0 && <Divider component="li" />}
                                    <ListItem
                                        onClick={() => handleServiceClick(service)}
                                        secondaryAction={
                                            <IconButton onClick={(e: any) => handleServiceRemove(e, service)} edge="end" aria-label="delete">
                                                <DeleteIcon />
                                            </IconButton>
                                        }
                                        sx={{
                                            '&:hover': {
                                                backgroundColor: 'rgba(0, 0, 0, 0.04)', // Or any other color
                                                cursor: 'pointer',
                                            },
                                        }}
                                    >
                                        <ListItemText
                                            primary={
                                                <Typography variant="subtitle1" noWrap>
                                                    {service.garageServiceTitle}
                                                </Typography>
                                            }
                                            secondary={
                                                <Typography variant="body2" noWrap sx={{ overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                                                    {service.relatedGarageLookupName} {t("for")} {service.vehicleLicensePlate}
                                                </Typography>
                                            }
                                        />
                                    </ListItem>
                                </React.Fragment>
                            ))}
                        </List>
                        <Button variant="outlined" fullWidth sx={{ marginTop: 1 }} onClick={handleAskQuestionClick}>
                            {t("Ask question")}
                        </Button>
                        <Button variant="contained" fullWidth sx={{ marginTop: 1 }} onClick={handleRequestQuoteClick}>
                            {t("Request quote")}
                        </Button>
                    </CardContent>
                </Card>
            }
            <GarageContactDialog
                requestQuote={requestQuote}
                services={services}
                open={dialogOpen}
                onClose={(removeAllService) => {
                    if (removeAllService) {
                        for (const service of services) {
                            dispatch(removeService(service));
                        }
                    }

                    setDialogOpen(false);
                }}
            />
        </>
    );
};