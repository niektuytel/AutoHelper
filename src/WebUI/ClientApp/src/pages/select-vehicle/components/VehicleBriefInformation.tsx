import React, { useEffect } from "react";
import { Box, Card, CircularProgress, Link, Paper, Skeleton, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";
import LicensePlateTextField from "./LicensePlateTextField";
import useVehicle from "../useVehicle";
import { CSSProperties } from "react";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    const { loading, vehicleBriefInfo } = useVehicle(license_plate);
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
                                    {vehicleBriefInfo?.motExpiryDate}
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