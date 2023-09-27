

import { Box, Card, CircularProgress, Link, Paper, Skeleton, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
import React, { useEffect } from "react";
import { styled } from '@mui/material/styles';
import { VehicleClient } from "../../../app/web-api-client";
import HeaderLicensePlateSearch from "../../../components/header/components/HeaderLicensePlateSearch";
import LicensePlateTextField from "./LicensePlateTextField";
import useVehicle from "../useVehicle";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    //const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    //const [isLoading, setIsLoading] = React.useState<boolean>(false);
    //const [vehicleInformation, setVehicleInformation] = React.useState<VehicleInformationResponse | undefined>(undefined);


    const { loading, isError, vehicleBriefInfo } = useVehicle(license_plate);

    return <>
        <Box sx={{ margin: "auto", maxWidth: "600px" }}>
            <Paper variant={isMobile ? "outlined" : "elevation"} sx={{ alignContent: "center" }} elevation={0}>
                <Table>
                    <TableBody>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Kenteken
                            </TableCell>
                            <TableCell style={{ textAlign: 'left' }}>
                                <LicensePlateTextField license_plate={license_plate} />
                            </TableCell>
                        </TableRow>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Merk
                            </TableCell>
                            {loading ?
                                <TableCell style={{ textAlign: 'left' }}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell style={{ textAlign: 'left' }}>
                                    {vehicleBriefInfo?.brand}
                                </TableCell>
                            }
                        </TableRow>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Verbruik
                            </TableCell>
                            {loading ?
                                <TableCell style={{ textAlign: 'left' }}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell style={{ textAlign: 'left' }}>
                                    {vehicleBriefInfo?.consumption}
                                </TableCell>
                            }
                        </TableRow>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Vervaldatum APK
                            </TableCell>
                            {loading ?
                                <TableCell style={{ textAlign: 'left' }}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell style={{ textAlign: 'left' }}>
                                    {vehicleBriefInfo?.motExpiryDate}
                                </TableCell>
                            }
                        </TableRow>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Kilometer stand
                            </TableCell>
                            {loading ?
                                <TableCell style={{ textAlign: 'left' }}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell style={{ textAlign: 'left' }}>
                                    {vehicleBriefInfo?.mileage}
                                </TableCell>
                            }
                        </TableRow>
                    </TableBody>
                </Table>
            </Paper>
            <Typography variant="body2" style={{ textAlign: 'right', margin: "5px" }}>
                <i>
                    (bron: <Link href="https://ovi.rdw.nl" target="_blank" rel="noopener noreferrer">rdw.nl</Link>)
                </i>
            </Typography>
        </Box>
    </>
}