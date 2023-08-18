import React, { useEffect } from 'react';
import { Box, Card, CardContent, Grid, Hidden, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
import CircularProgress from '@mui/material/CircularProgress';
import Slider from 'react-slick';

import { styled } from '@mui/material/styles';
import ArrowForwardIosSharpIcon from '@mui/icons-material/ArrowForwardIosSharp';
import MuiAccordion, { AccordionProps } from '@mui/material/Accordion';
import MuiAccordionSummary, {
    AccordionSummaryProps,
} from '@mui/material/AccordionSummary';
import MuiAccordionDetails from '@mui/material/AccordionDetails';

// own imports
import { VehicleClient, VehicleInformationResponse } from "../../../app/web-api-client";

const Accordion = styled((props: AccordionProps) => (
    <MuiAccordion disableGutters elevation={0} square {...props} />
))(({ theme }) => ({
    border: `1px solid ${theme.palette.divider}`,
    '&:not(:last-child)': {
        borderBottom: 0,
    },
    '&:before': {
        display: 'none',
    },
}));

const AccordionSummary = styled((props: AccordionSummaryProps) => (
    <MuiAccordionSummary
        expandIcon={<ArrowForwardIosSharpIcon sx={{ fontSize: '0.9rem' }} />}
        {...props}
    />
    ))(({ theme }) => ({
        backgroundColor:
            theme.palette.mode === 'dark'
                ? 'rgba(255, 255, 255, .05)'
                : 'rgba(0, 0, 0, .03)',
        flexDirection: 'row-reverse',
        '& .MuiAccordionSummary-expandIconWrapper.Mui-expanded': {
            transform: 'rotate(90deg)',
        },
        '& .MuiAccordionSummary-content': {
            marginLeft: theme.spacing(1),
        },
    }));

const AccordionDetails = styled(MuiAccordionDetails)(({ theme }) => ({
    padding: theme.spacing(2),
    borderTop: '1px solid rgba(0, 0, 0, .125)',
}));

interface IProps {
    licence_plate: string
}

export default ({ licence_plate }: IProps) => {
    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    const [expanded, setExpanded] = React.useState<string | false>('panel1');
    const [isLoading, setIsLoading] = React.useState<boolean>(false);
    const [vehicleInformation, setVehicleInformation] = React.useState<VehicleInformationResponse | undefined>(undefined);

    const handleLoading = () => {
        setIsLoading(true);

        vehicleClient.getVehicleInformation(licence_plate)
            .then(response => {
                if (response) {
                    console.log("Response received:", response);
                    setVehicleInformation(response);
                } else {
                    // TODO: trigger snackbar
                    console.error("Failed to get vehicle by license plate");
                }
            })
            .catch(error => {
                // TODO: trigger snackbar
                console.error("Error occurred:", error);
            })
            .finally(() => {
                setIsLoading(false);
            });
    }

    useEffect(() => {
        if (vehicleInformation === undefined) {
            handleLoading();
        }
    }, []);

    const handleChange =
        (panel: string) => (event: React.SyntheticEvent, newExpanded: boolean) => {
            setExpanded(newExpanded ? panel : false);
        };

    return (
        <>
            <Box sx={{ padding: "5vh" }}>
                <Typography variant="h6" color="black" style={{ textAlign: 'center' }}>
                    Informatie over voertuig: <b>{licence_plate}</b>
                </Typography>
            </Box>
            <Box sx={{ marginBottom: "40px" }}>
                {isLoading ? (
                    <Box display="flex" justifyContent="center">
                        <CircularProgress />
                    </Box>
                ) : vehicleInformation?.data && vehicleInformation.data.map((section, index) => (
                    <Accordion expanded={expanded === `panel${index}`} onChange={handleChange(`panel${index}`)}>
                        <AccordionSummary aria-controls={`panel${index}d-content`} id={`panel${index}d-header`}>
                            <Typography>{section.title}</Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <Table>
                                <TableBody>
                                    {section.values!.map((line, rowIndex) => (
                                        <TableRow
                                            key={rowIndex}
                                            sx={{
                                                backgroundColor: rowIndex % 2 === 0 ? 'grey.100' : 'white'
                                            }}
                                        >
                                            <TableCell
                                                style={{ width: '50%', textAlign: 'left' }}
                                            >
                                                {line.name}
                                            </TableCell>
                                            <TableCell
                                                style={{ width: '50%', textAlign: 'left' }}
                                            >
                                                {line.value}
                                            </TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </AccordionDetails>
                    </Accordion>
                ))}
            </Box>
        </>
    );
}
