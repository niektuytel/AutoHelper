

import { Box, Card, CircularProgress, Link, Skeleton, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
import React, { useEffect } from "react";
import { styled } from '@mui/material/styles';
import { VehicleClient } from "../../../app/web-api-client";
import HeaderLicensePlateSearch from "../../../components/header/components/HeaderLicensePlateSearch";
import LicensePlateTextField from "./LicensePlateTextField";
import useVehicle from "../useVehicle";

interface IProps {
    license_plate: string
}

export default ({ license_plate }: IProps) => {
    //const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
    //const [isLoading, setIsLoading] = React.useState<boolean>(false);
    //const [vehicleInformation, setVehicleInformation] = React.useState<VehicleInformationResponse | undefined>(undefined);


    const { loading, isError, vehicleBriefInfo } = useVehicle(license_plate);

    //const handleLoading = () => {
    //    setIsLoading(true);

    //    vehicleClient.getVehicleInformation(license_plate)
    //        .then(response => {
    //            if (response) {
    //                console.log("Response received:", response);
    //                setVehicleInformation(response);
    //            } else {
    //                // TODO: trigger snackbar
    //                console.error("Failed to get vehicle by license plate");
    //            }
    //        })
    //        .catch(error => {
    //            // TODO: trigger snackbar
    //            console.error("Error occurred:", error);
    //        })
    //        .finally(() => {
    //            setIsLoading(false);
    //        });
    //}

    //useEffect(() => {
    //    if (vehicleInformation === undefined) {
    //        handleLoading();
    //    }
    //}, []);

    return <>
        <Box sx={{ margin: "auto", maxWidth: "600px" }}>
            <Card sx={{ alignContent: "center" }} elevation={6}>
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

                        {/*{isLoading || !vehicleInformation ?*/}
                        {/*    <Box display="flex" justifyContent="center">*/}
                        {/*        <CircularProgress />*/}
                        {/*    </Box>*/}
                        {/*    :*/}
                        {/*    vehicleInformation.cardInfo!.map((line, rowIndex) => (*/}
                        {/*    <TableRow*/}
                        {/*        key={rowIndex}*/}
                               
                        {/*    >*/}
                        {/*        {line!.map((value) => (*/}

                        {/*            <TableCell*/}
                        {/*                style={{ textAlign: 'left' }}*/}
                        {/*            >*/}
                        {/*                {value}*/}
                        {/*            </TableCell>*/}
                        {/*        ))}*/}
                        {/*    </TableRow>*/}
                        {/*))}*/}
                    </TableBody>
                </Table>
            </Card>
            <Typography variant="body2" style={{ textAlign: 'right', margin: "5px" }}>
                <i>
                    (bron: <Link href="https://ovi.rdw.nl" target="_blank" rel="noopener noreferrer">rdw.nl</Link>)
                </i>
            </Typography>
        </Box>
    </>
}