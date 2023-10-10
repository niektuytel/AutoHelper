
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
import { GarageLookupDto } from '../../../app/web-api-client';
import { DAYSINWEEKSHORT } from '../../../constants/days';

interface IProps {
    garage: GarageLookupDto;
}

export default ({ garage }: IProps) => {
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
        let identifier = garage.garageId;
        if (!identifier && garage.website) {
            const url = new URL(garage.website);
            identifier = encodeURIComponent(url.hostname);
        }

        navigate(`${window.location.pathname}/${identifier}`);
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
                    alignItems: "center"
                }}
            >
                <Box
                    style={{
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "flex-start"
                    }}
                >
                    <Typography variant="h6">
                        {garage.name}
                    </Typography>
                    <Typography variant="body1" sx={{ color: 'grey.600' }}>
                        <PlaceIcon fontSize='small' sx={{ mr: 1 }} /> {/*, color:"#E34133"*/}
                        {`${garage?.address}, ${garage?.city} (${Math.round(garage.distanceInMeter! * 0.001)} km)`}
                    </Typography>
                    <Typography variant="body1" sx={{ color: 'grey.600' }}>
                        <AccessTimeIcon fontSize='small' sx={{ mr: 1 }} />
                        {`${[...new Set(garage.daysOfWeek! || [])].map(dayIndex => t(DAYSINWEEKSHORT[dayIndex!]))}`}
                    </Typography>
                    <Box>
                        {garage.userRatingsTotal && garage.userRatingsTotal > 5 &&
                            <Chip
                                variant="outlined"
                                color="warning"
                                size="small"
                                label={`${garage.rating}/5.0`}
                                icon={<StarIcon />}
                                sx={{ mr: 1, mt: 1, color:"orange" }}
                            />
                        }
                        {garage.hasPickupService === true &&
                            <Chip
                                variant="outlined"
                                color="primary"
                                size="small"
                                label={t('have the car picked up')}
                                icon={<ModeOfTravelIcon />}
                                sx={{ mr: 1, mt: 1 }}
                            />
                        }
                        {garage.hasReplacementTransportService === true &&
                            <Chip
                                variant="outlined"
                                color="default"
                                size="small"
                                label={t('replacement vehicle')}
                                icon={<PublishedWithChangesIcon />}
                                sx={{ mr: 1, mt: 1 }}
                            />
                        }
                    </Box>
                </Box>
            </Box>
        </Paper>
    </>;

}

