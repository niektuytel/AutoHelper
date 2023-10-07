
import { Box, Chip, Paper, Typography } from '@mui/material';
import React from 'react';
import { GarageLookupDto } from '../../../app/web-api-client';
import EuroSymbolIcon from '@mui/icons-material/EuroSymbol';
import PlaceIcon from '@mui/icons-material/Place';
import PublishedWithChangesIcon from '@mui/icons-material/PublishedWithChanges';
import ModeOfTravelIcon from '@mui/icons-material/ModeOfTravel';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import { DAYSINWEEKSHORT } from '../../../constants/days';
import { useTranslation } from 'react-i18next';
import ImageLogo from '../../../components/logo/ImageLogo';
import { useNavigate } from 'react-router';

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
        var identifier = garage.garageId;
        if (!identifier && garage.website) {
            identifier = encodeURIComponent(garage.website);
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
                {/*<ImageLogo*/}
                {/*    style={{*/}
                {/*        width: "50px",*/}
                {/*        height: "50px",*/}
                {/*        marginRight: "10px"*/}
                {/*    }}*/}
                {/*/>*/}
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
                        <PlaceIcon fontSize='small' sx={{ mr: 1 }} />
                        {`${garage?.address}, ${garage?.city} (${garage.distanceInMeter! * 0.001} km)`}
                    </Typography>
                    <Typography variant="body1" sx={{ color: 'grey.600' }}>
                        <AccessTimeIcon fontSize='small' sx={{ mr: 1 }} />
                        {`${[...new Set(garage.daysOfWeek! || [])].map(dayIndex => t(DAYSINWEEKSHORT[dayIndex!]))}`}
                    </Typography>
                    { garage.hasBestPrice || garage.hasPickupService || garage.hasReplacementTransportService  &&
                        <Box>
                            { garage.hasPickupService &&
                                <Chip
                                    variant="outlined"
                                    color="primary"
                                    size="small"
                                    label="auto op laten halen"// TODO: translate
                                    icon={<ModeOfTravelIcon />}
                                    sx={{ mr: 1 }}
                                />
                            }
                            {garage.hasReplacementTransportService &&
                                <Chip
                                    variant="outlined"
                                    color="default"
                                    size="small"
                                    label="vervangend vervoer"// TODO: translate
                                    icon={<PublishedWithChangesIcon />}
                                    sx={{ mr: 1 }}
                                />
                            }
                            {garage.hasBestPrice &&
                                <Chip
                                    variant="outlined"
                                    color="success"
                                    size="small"
                                    label="beste prijs"// TODO: translate
                                    icon={<EuroSymbolIcon />}
                                    sx={{ mr: 1 }}
                                />
                            }
                        </Box>
                    }
                    <Box>
                        {garage.knownServices!.map(knownservice => 
                            <Chip
                                variant="outlined"
                                color="default"
                                size="small"
                                label={knownservice}
                                sx={{ mr: 1, mt: 1 }}
                            />
                        )}
                    </Box>
                </Box>
            </Box>
            <Box
                style={{
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "flex-end"
                }}
            >
                {/*<Typography*/}
                {/*    variant="body1"*/}
                {/*    style={{*/}
                {/*        color: colorOnIndex(index)*/}
                {/*    }}*/}
                {/*>*/}
                {/*    {garage.distance} km*/}
                {/*</Typography>*/}
                {/*<Typography*/}
                {/*    variant="body1"*/}
                {/*    style={{*/}
                {/*        color: colorOnIndex(index)*/}
                {/*    }}*/}
                {/*>*/}
                {/*    {garage.price} €*/}
                {/*</Typography>*/}
            </Box>
        </Paper>
    </>;

}

