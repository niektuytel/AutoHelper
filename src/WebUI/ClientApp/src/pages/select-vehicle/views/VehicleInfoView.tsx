import React, { useEffect } from 'react';
import { Box, Card, CardContent, Grid, Hidden, Link, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
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
    <MuiAccordion elevation={0} square {...props} />
))(({ theme }) => ({
    border: `1px solid ${theme.palette.divider}`,
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
            marginRight: theme.spacing(1),
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
    const [expanded, setExpanded] = React.useState<string[]>([]);
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

    const handleChange = (panel: string) => (event: React.SyntheticEvent, newExpanded: boolean) => {
        if (newExpanded) {
            setExpanded((prev) => [...prev, panel]);
        } else {
            setExpanded((prev) => prev.filter((item) => item !== panel));
        }
    };

    return (
        <>
            <Box sx={{ marginBottom: "40px", padding: "5vh"}}>
                {isLoading ?
                    <Box display="flex" justifyContent="center">
                        <CircularProgress />
                    </Box>
                : vehicleInformation?.data &&
                    <>
                        <Box sx={{ margin: "auto", maxWidth: "600px"}}>
                            <Typography variant="body2" style={{ textAlign: 'right', margin: "5px" }}>
                                <i>
                                    (bron: <Link href="https://ovi.rdw.nl" target="_blank" rel="noopener noreferrer">rdw.nl</Link>)
                                </i>
                            </Typography>
                            <Card sx={{ alignContent: "center" }} elevation={6}>
                                <Typography variant="h6" color="black" style={{ textAlign: 'center', margin: "20px" }}>
                                    <b>Informatie: {licence_plate}</b>
                                </Typography>
                                <Table>
                                    <TableBody>
                                        {vehicleInformation.cardInfo!.map((line, rowIndex) => (
                                            <TableRow
                                                key={rowIndex}
                                                sx={{
                                                    backgroundColor: rowIndex % 2 === 0 ? 'grey.100' : 'white'
                                                }}
                                            >
                                                {line!.map((value) => (

                                                    <TableCell
                                                        style={{ width: `${(line.length / 100)}%`, textAlign: 'left' }}
                                                    >
                                                        {value}
                                                    </TableCell>
                                                ))}
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            </Card>
                        </Box>
                        <Typography variant="h6" color="#1C94F3" style={{ textAlign: 'center', marginTop:"40px"  }}>
                            <b>Alle informatie</b>
                        </Typography>
                        {vehicleInformation.data.map((section, index) => (
                            <Accordion expanded={expanded.includes(`panel${index}`)} onChange={handleChange(`panel${index}`)}>
                                <AccordionSummary aria-controls={`panel${index}d-content`} id={`panel${index}d-header`}>
                                    <Typography>{section.title}</Typography>
                                </AccordionSummary>
                                <AccordionDetails sx={{ padding: "0" }}>
                                    <Table>
                                        <TableBody>
                                            {section.values!.map((line, rowIndex) => (
                                                <TableRow
                                                    key={rowIndex}
                                                    sx={{
                                                        backgroundColor: rowIndex % 2 === 0 ? 'grey.100' : 'white'
                                                    }}
                                                >
                                                    {line!.map((value, cellIndex) => (

                                                        <TableCell
                                                            style={{ width: `${(line.length / 100)}%`, textAlign: 'left' }}
                                                        >
                                                            {value}
                                                        </TableCell>
                                                    ))}
                                                </TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </AccordionDetails>
                            </Accordion>
                        ))}
                    </>
                }
            </Box>
        </>
    );
}
