
import { Box, Chip, Paper, Typography } from '@mui/material';
import React from 'react';
import PlaceIcon from '@mui/icons-material/Place';
import PublishedWithChangesIcon from '@mui/icons-material/PublishedWithChanges';
import ModeOfTravelIcon from '@mui/icons-material/ModeOfTravel';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import { useTranslation } from 'react-i18next';
import StarIcon from '@mui/icons-material/Star';
import { useNavigate } from 'react-router';

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
    const { t } = useTranslation();
    const navigate = useNavigate();

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
        navigate(`${ROUTES.GARAGE}/${garage.identifier}?licensePlate=${licensePlate}&lat=${lat}&lng=${lng}`);
    };


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
                        flexDirection: "column", // keep this as 'column'
                        alignItems: "flex-start",
                        width: "100%"
                    }}
                >
                    <Box
                        style={{
                            display: "flex",
                            flexDirection: "row", // Change this to 'row'
                            alignItems: "center",
                            justifyContent: "space-between", // Spread items apart
                            width: "100%" // Ensure this inner container also takes up the full width
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
                        <PlaceIcon fontSize='small'  /> {/*, color:"#E34133"*/}
                        {`${garage?.address}, ${garage?.city} (${Math.round(garage.distanceInMeter! * 0.001)} km)`}
                    </Typography>
                    <Typography variant="body1" sx={{ color: 'grey.600' }}>
                        <AccessTimeIcon fontSize='small' sx={{ mr: "4px" }} />
                        {`${[...new Set(garage.daysOfWeek! || [])].map(dayIndex => t(DAYSINWEEKSHORT[dayIndex!]))}`}
                    </Typography>
                    <Box>
                        {garage.knownServices && garage.knownServices.map(service => service !== GarageServiceType.Other &&
                            <Chip
                                key={service}
                                label={t(`${GarageServiceType[service]}.Filter`)}
                                sx={{ mr: 1, mt: 1 }}
                                variant="outlined"
                                size="small"
                            />
                        )}
                        {garage.hasPickupService === true &&
                            <Chip
                                variant="outlined"
                                color="primary"
                                size="small"
                                label={t('have the car picked up')}
                                icon={<ModeOfTravelIcon />}
                                sx={{ mr: "3px", mt: "3px" }}
                            />
                        }
                        {garage.hasReplacementTransportService === true &&
                            <Chip
                                variant="outlined"
                                color="default"
                                size="small"
                                label={t('replacement vehicle')}
                                icon={<PublishedWithChangesIcon />}
                                sx={{ mr: "3px", mt: "3px" }}
                            />
                        }
                    </Box>
                </Box>
            </Box>
        </Paper>
    </>;

}

