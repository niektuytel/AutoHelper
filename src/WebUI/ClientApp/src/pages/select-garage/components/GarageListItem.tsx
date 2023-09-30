
import { Box, Chip, Paper, Typography } from '@mui/material';
import React from 'react';
import { GarageItemSearchDto } from '../../../app/web-api-client';
import EuroSymbolIcon from '@mui/icons-material/EuroSymbol';
import PlaceIcon from '@mui/icons-material/Place';
import PublishedWithChangesIcon from '@mui/icons-material/PublishedWithChanges';
import ModeOfTravelIcon from '@mui/icons-material/ModeOfTravel';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import { DAYSINWEEKSHORT } from '../../../constants/days';
import { useTranslation } from 'react-i18next';
import ImageLogo from '../../../components/logo/ImageLogo';

interface IProps {
    garage: GarageItemSearchDto;
}

export default ({ garage }: IProps) => {
    const { t } = useTranslation();


    return <>
        <Paper
            elevation={5}  
            style={{
                display: "flex",
                flexDirection: "row",
                justifyContent: "space-between",
                alignItems: "center",
                padding: "10px",
                borderBottom: "1px solid #ccc"
            }}
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
                        <PlaceIcon fontSize='small' />
                        {`${garage.location?.address}, ${garage.location?.city}`}
                    </Typography>
                    <Typography variant="body1" sx={{ color: 'grey.600' }}>
                        <AccessTimeIcon fontSize='small' />
                        {`
                            ${[...new Set(garage.employees?.flatMap(x => x.workingDaysOfWeek) || [])]
                                .map(dayIndex => t(DAYSINWEEKSHORT[dayIndex!]))}
                        `}
                    </Typography>
                    <Box>
                        <Chip
                            variant="outlined"
                            color="primary"
                            size="small"
                            label="auto op laten halen"// HasPickupService
                            icon={<ModeOfTravelIcon />}
                            sx={{ mr: 1 }}
                        />
                        <Chip
                            variant="outlined"
                            color="default"
                            size="small"
                            label="vervangend vervoer"// HasReplacementTransportService
                            icon={<PublishedWithChangesIcon />}
                            sx={{ mr: 1 }}
                        />
                        <Chip
                            variant="outlined"
                            color="success"
                            size="small"
                            label="beste prijs"// HasBestPrice
                            icon={<EuroSymbolIcon />}
                            sx={{ mr: 1 }}
                        />
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

