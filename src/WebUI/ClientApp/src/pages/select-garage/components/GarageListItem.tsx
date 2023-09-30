
import { Box, Chip, Typography } from '@mui/material';
import React from 'react';
import { GarageItemSearchDto } from '../../../app/web-api-client';
import PlaceIcon from '@mui/icons-material/Place';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import { DAYSINWEEKSHORT } from '../../../constants/days';
import { useTranslation } from 'react-i18next';

interface IProps {
    garage: GarageItemSearchDto;
}

export default ({ garage }: IProps) => {
    const { t } = useTranslation();


    return <>
        <Box
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
                {/*    src={garage.logo}*/}
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
                    <Typography variant="body1">
                        <PlaceIcon fontSize='small' />
                        {`${garage.location?.address}, ${garage.location?.city}`}
                    </Typography>
                    <Typography variant="body1">
                        <AccessTimeIcon fontSize='small' />
                        {`
                            ${[...new Set(garage.employees?.flatMap(x => x.workingDaysOfWeek) || [])]
                                .map(dayIndex => t(DAYSINWEEKSHORT[dayIndex!]))}
                        `}
                    </Typography>
                    <Typography variant="body1">
                        <Chip
                            avatar={<PlaceIcon />}
                            label="Pick up your car!"
                            variant="outlined"
                        />
                    </Typography>
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
        </Box>
    </>;

}

