
import { Box, Chip, Paper, Typography } from '@mui/material';
import React, { useEffect, useState } from 'react';
import PlaceIcon from '@mui/icons-material/Place';
import PublishedWithChangesIcon from '@mui/icons-material/PublishedWithChanges';
import ModeOfTravelIcon from '@mui/icons-material/ModeOfTravel';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import { useTranslation } from 'react-i18next';
import StarIcon from '@mui/icons-material/Star';
import { useLocation, useNavigate } from 'react-router';

// own imports
import { GarageLookupBriefDto, GarageServiceType } from '../../../app/web-api-client';
import { DAYSINWEEKSHORT } from '../../../constants/days';
import { ROUTES } from '../../../constants/routes';

interface IProps {
    garage: GarageLookupBriefDto;
    licensePlate: string;
    lat: string;
    lng: string;
}

export default ({ garage, licensePlate, lat, lng }: IProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const navigate = useNavigate();
    const location = useLocation();

    const queryParams = new URLSearchParams(window.location.search);
    const [filters, setFilters] = useState<string[]>([]);
    useEffect(() => {
        if (queryParams.has("filters")) {
            const filters = queryParams.get("filters")?.split(",");
            setFilters(filters || []);
        }

    }, [window.location.search]);

    // Local state for hover effect
    const [isHovered, setIsHovered] = React.useState(false);

    // Handler for mouse enter
    const handleMouseEnter = () => {
        setIsHovered(true);
    };

    // Handler for mouse leave
    const handleMouseLeave = () => {
        setIsHovered(false);
    };

    // Handler for click
    const handleClick = () => {
        navigate(`${ROUTES.GARAGE}/${garage.identifier}?licensePlate=${licensePlate}&lat=${lat}&lng=${lng}`, { state: { from: location } });
    };

    const uniqueTypes = new Set();
    const uniqueServices = garage.services?.filter(service => {
        const isUnique = !uniqueTypes.has(service.type);
        uniqueTypes.add(service.type);
        return isUnique && service.type !== GarageServiceType.Other;
    });

    return <>
        <Paper
            variant="outlined"
            sx={{
                mb: 1,
                cursor: 'pointer',
                backgroundColor: isHovered ? 'grey.100' : 'white'
            }}
            style={{
                display: "flex",
                flexDirection: "row",
                justifyContent: "space-between",
                alignItems: "center",
                padding: "10px",
                width: "100%",
                borderBottom: "1px solid #ccc"
            }}
            onMouseEnter={handleMouseEnter}
            onMouseLeave={handleMouseLeave}
            onClick={handleClick}
        >
            <Box
                style={{
                    display: "flex",
                    flexDirection: "row",
                    width: "100%",
                    alignItems: "center"
                }}
            >
                <Box
                    style={{
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "flex-start",
                        width: "100%"
                    }}
                >
                    <Box
                        style={{
                            display: "flex",
                            flexDirection: "row",
                            alignItems: "center",
                            justifyContent: "space-between",
                            width: "100%"
                        }}
                    >
                        <Typography variant="h6">{garage.name}</Typography>

                        {garage.userRatingsTotal && garage.userRatingsTotal > 5 &&
                            <Chip
                                variant="outlined"
                                color="warning"
                                size="small"
                                label={`${garage.rating!.toFixed(1)}`}
                                icon={<StarIcon />}
                                sx={{ ml: 1, color: "orange" }}
                            />
                        }
                    </Box>
                    <Typography variant="body1" sx={{ color: 'grey.600' }}>
                        <PlaceIcon fontSize='small'  />
                        {`${garage?.address}, ${garage?.city} (${Math.round(garage.distanceInMeter! * 0.001)} km)`}
                    </Typography>
                    <Typography variant="body1" sx={{ color: 'grey.600' }}>
                        <AccessTimeIcon fontSize='small' sx={{ mr: "4px" }} />
                        {`${[...new Set(garage.daysOfWeek?.filter(day => day.includes(': ')) || [])].map((day, dayIndex) => t(DAYSINWEEKSHORT[dayIndex!]))}`}
                    </Typography>
                    <Box>
                        {uniqueServices?.map(service => (
                            <Chip
                                key={`service.id:${service.id}`}
                                label={t(`serviceTypes:${GarageServiceType[service.type!]}.Filter`)}
                                variant={filters.includes(String(service.type)) ? "filled" : "outlined"}
                                sx={{ mr: 1, mt: 1 }}
                                size="small"
                            />
                        ))}
                    </Box>
                </Box>
            </Box>
        </Paper>
    </>;

}

