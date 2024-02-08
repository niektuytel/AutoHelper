import React, { CSSProperties } from "react";
import { useQuery } from "react-query";
import { Box, Link, Paper, Skeleton, styled, Table, TableBody, TableCell, TableRow, Typography } from "@mui/material";

// own imports
import LicensePlateTextField from "./EditableLicensePlate";
import useSearchVehicle from "../../../useSearchVehicle";
import { useTranslation } from "react-i18next";
import VehicleExpiryDateCell from "./VehicleExpiryDateCell";

interface IProps {
    isMobile: boolean;
    license_plate: string
}

export default ({ isMobile, license_plate }: IProps) => {
    const { t } = useTranslation();
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


    // Define a styled TableCell with hover effects
    const ClickableTableCell = styled(TableCell)({
        cursor: 'pointer',
        '&:hover': {
            backgroundColor: '#f5f5f5', // Light grey background on hover
        },
        // Add any other styles you want
    });

    // Click handler function
    const handleCellClick = () => {
        console.log('TableCell clicked');
        // Add your click handling logic here
    };

    const cellStyle: CSSProperties = isMobile ? { textAlign: 'left' } : { textAlign: 'left', paddingRight: '0' };
    return <>
        <Box sx={{ margin: "auto", maxWidth: "600px" }}>
            <Paper variant={isMobile ? "outlined" : "elevation"} sx={{ alignContent: "center" }} elevation={0}>
                <Table>
                    <TableBody>
                        <TableRow>
                            <TableCell key={`cell-licenseplate`} style={{ textAlign: 'left' }}>
                                {t("VehiclePage.SpecificationsCard.LicensePlate")}
                            </TableCell>
                            <TableCell key={`cell-licenseplate-value`} style={cellStyle}>
                                <LicensePlateTextField license_plate={license_plate} />
                            </TableCell>
                        </TableRow>
                        <TableRow>
                            <TableCell key={`cell-mark`} style={{ textAlign: 'left' }}>
                                {t("VehiclePage.SpecificationsCard.Mark")}
                            </TableCell>
                            {loading ?
                                <TableCell key={`cell-mark-value`} style={cellStyle}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell key={`cell-mark-value`} style={cellStyle}>
                                    {vehicleBriefInfo?.brand}
                                </TableCell>
                            }
                        </TableRow>
                        <TableRow>
                            <TableCell key={`cell-consumption`} style={{ textAlign: 'left' }}>
                                {t("VehiclePage.SpecificationsCard.Consumption")}
                            </TableCell>
                            {loading ?
                                <TableCell key={`cell-consumption-value`} style={cellStyle}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell key={`cell-consumption-value`} style={cellStyle}>
                                    {vehicleBriefInfo?.consumption}
                                </TableCell>
                            }
                        </TableRow>
                        <TableRow>
                            <TableCell key={`cell-expirydate`} style={{ textAlign: 'left' }}>
                                {t("VehiclePage.SpecificationsCard.MotExpiryDate")}
                            </TableCell>
                            {loading ?
                                <TableCell key={`cell-expirydate-value`} style={cellStyle}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <VehicleExpiryDateCell expiryDate={vehicleBriefInfo?.dateOfMOTExpiry} isMobile={isMobile} />
                            }
                        </TableRow>
                        <TableRow>
                            <TableCell key={`cell-mileage`} style={{ textAlign: 'left' }}>
                                {t("VehiclePage.SpecificationsCard.Mileage")}
                            </TableCell>
                            {loading ?
                                <TableCell key={`cell-mileage-value`} style={cellStyle}>
                                    <Skeleton />
                                </TableCell>
                                :
                                <TableCell key={`cell-mileage-value`} style={cellStyle}>
                                    {vehicleBriefInfo?.mileage}
                                </TableCell>
                            }
                        </TableRow>
                    </TableBody>
                </Table>
            </Paper>
            <Typography variant="body2" style={{ textAlign: 'right', margin: "5px" }}>
                <i>
                    ({t("VehiclePage.SpecificationsCard.Source")}: <Link href="https://ovi.rdw.nl" target="_blank" rel="noopener noreferrer">rdw.nl</Link>)
                </i>
            </Typography>
        </Box>
    </>
}