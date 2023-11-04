import React, { useEffect } from "react";
import { Box, Card, CircularProgress, Link, Paper, Skeleton, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
import LicensePlateTextField from "./LicensePlateTextField";
import { CSSProperties } from "react";
import useSearchVehicle from "../useSearchVehicle";
import { useQuery } from "react-query";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    const { loading, fetchVehicleByPlate } = useSearchVehicle();
    const { data: vehicleBriefInfo } = useQuery(
        [`vehicleBriefInfo-${license_plate}`],
        () => fetchVehicleByPlate(license_plate),
        {
            enabled: true,
            retry: 1,
            refetchOnWindowFocus: false,
            cacheTime: 30 * 60 * 1000,  // 30 minutes
            staleTime: 60 * 60 * 1000, // 1 hour
        }
    );

    const cellStyle: CSSProperties = isMobile ? { textAlign: 'left' } : { textAlign: 'left', paddingRight: '0' };
    return <>
        <Box sx={{ margin: "auto", maxWidth: "600px" }}>
            <Paper variant={isMobile ? "outlined" : "elevation"} sx={{ alignContent: "center" }} elevation={0}>
                <Table>
                    <TableBody>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Kenteken
                            </TableCell>
                            <TableCell style={cellStyle}>
                                <LicensePlateTextField license_plate={license_plate} />
                            </TableCell>
                        </TableRow>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Merk
                            </TableCell>
                            {loading ?
                                <TableCell style={cellStyle}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell  style={cellStyle}>
                                    {vehicleBriefInfo?.brand}
                                </TableCell>
                            }
                        </TableRow>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Verbruik
                            </TableCell>
                            {loading ?
                                <TableCell style={cellStyle}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell  style={cellStyle}>
                                    {vehicleBriefInfo?.consumption}
                                </TableCell>
                            }
                        </TableRow>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Vervaldatum APK
                            </TableCell>
                            {loading ?
                                <TableCell style={cellStyle}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell style={cellStyle}>
                                    {vehicleBriefInfo?.motExpiryDate?.toDateString()}
                                </TableCell>
                            }
                        </TableRow>
                        <TableRow>
                            <TableCell style={{ textAlign: 'left' }}>
                                Kilometer stand
                            </TableCell>
                            {loading ?
                                <TableCell style={cellStyle}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell style={cellStyle}>
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