import React, { useEffect } from "react";
import { Box, Card, CardContent, Chip, Container, Grid, Paper, Skeleton, styled, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import BuildIcon from '@mui/icons-material/Build'; // For maintenance
import CheckedAuto from '@mui/icons-material/NoCrash';
import PendingActionsIcon from '@mui/icons-material/PendingActions'; // For outstanding maintenance
import EngineeringIcon from '@mui/icons-material/Engineering'; // For services
import { ROUTES } from "../../../constants/routes";
//import { GarageClient, GarageOverview } from "../../../app/web-api-client";
import { ROLES } from "../../../constants/roles";
import { LineChart } from '@mui/x-charts/LineChart';
import useGarageOverview from "./useGarageOverview";
import { getFormatedLicense } from "../../../app/LicensePlateUtils";
import { GarageServiceType } from "../../../app/web-api-client";

let ServicesData = [2400, 1398, 9800, 3908, 4800, 3800, 4300, 1890, 2390, 3490, 1890, 2390];
let VehicleData = [4000, 3000, 2000, 2780, 1890, 2390, 3490, 1890, 2390, 3490, 1890, 2390];
const xLabels = [
    'January',
    'Februari',
    'Maart',
    'April',
    'Mei',
    'Juni',
    'Juli',
    'Augustus',
    'September',
    'Oktober',
    'November',
    'December',
];
// own imports

const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(1),
    textAlign: 'center',
    color: theme.palette.text.secondary,
}));


interface IProps {
}

export default ({ }: IProps) => {

    function createData(
        date: string,
        licensePlate: string,
        verified: boolean,
    ) {
        return { date, licensePlate, verified };
    }

    const rows = [
        createData('23/02/2023', "87-GRN-6", true),
    ];

    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('lg'));
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const navigate = useNavigate();

    const { loading, isError, garageOverview } = useGarageOverview();

    const gotoVehiclePage = (licensePlate: string) => {
        navigate(`${ROUTES.SELECT_VEHICLE}/${licensePlate}`);
    }

    const gotoServices = (e: any) => {
        navigate(`${ROUTES.GARAGE_ACCOUNT.SERVICES}/`);
    }

    const gotoServiceLogs = (e: any) => {
        navigate(`${ROUTES.GARAGE_ACCOUNT.SERVICELOGS}/`);
    }

    if (!loading && garageOverview?.chartPoints) {
        garageOverview?.chartPoints.map((row, index) => {
            ServicesData[index] = row.approvedAmount || 0;
            VehicleData[index] = row.vehiclesAmount || 0;
        })
    }

    return (
        <Box
            style={{ position: "relative" }}
        >
            <Container
                maxWidth="lg"
                sx={{
                    marginTop: 3,
                    marginBottom: 3,
                    padding: "0",
                    textAlign: "center"
                }}
            >
                <Grid container spacing={3}>
                    <Grid item xs={isMobile ? 6 : 4}>
                        <Card>
                            <CardContent sx={{ display: 'flex', flexDirection: 'column', height: "150px", justifyContent: 'space-between' }}>
                                <CheckedAuto style={{ fontSize: 40, alignSelf: 'center' }} />
                                <Box>
                                    <Typography variant="h5" align="center">
                                        23 Voertuigen<br />in 2023
                                    </Typography>
                                </Box>
                            </CardContent>
                        </Card>
                    </Grid>
                    <Grid item xs={isMobile ? 6 : 4}>
                        <Card>
                            <CardContent
                                sx={{
                                    display: 'flex', flexDirection: 'column', height: "150px", justifyContent: 'space-between',
                                    '&:hover': {
                                        backgroundColor: 'rgba(0, 0, 0, 0.08)', // or any color you want
                                        cursor: 'pointer'
                                    }
                                }}
                                onClick={gotoServiceLogs}
                            >
                                <PendingActionsIcon style={{ fontSize: 40, alignSelf: 'center' }} />
                                <Box>
                                    <Typography variant="h5" align="center">
                                        12 Diensten<br />in 2023
                                    </Typography>
                                </Box>
                            </CardContent>
                        </Card>
                    </Grid>
                    {!isMobile &&
                        <Grid item xs={4}>
                            <Card>
                                <CardContent 
                                    sx={{
                                        display: 'flex', flexDirection: 'column', height: "150px", justifyContent: 'space-between',
                                        '&:hover': {
                                            backgroundColor: 'rgba(0, 0, 0, 0.08)', // or any color you want
                                            cursor: 'pointer'
                                        }
                                    }}
                                    onClick={gotoServices}
                                >
                                    <BuildIcon style={{ fontSize: 40, alignSelf: 'center' }} />
                                    <Box>
                                        <Typography variant="h5" align="center">
                                            U ondersteund<br />
                                            20 diensten
                                        </Typography>
                                    </Box>
                                </CardContent>
                            </Card>
                        </Grid>
                    }
                    <Grid item xs={isMobile ? 12 : 8}>
                        <Item sx={{ height:"300px", width:"100%"}}>
                            <LineChart
                                series={[
                                    { data: ServicesData, label: 'services' },
                                    { data: VehicleData, label: 'vehicles' },
                                ]}
                                xAxis={[{ scaleType: 'point', data: xLabels }]}
                            />
                        </Item>
                    </Grid>
                    {!isMobile &&
                        <Grid item xs={4}>
                            <TableContainer component={Paper}>
                                <Table aria-label="simple table">
                                    <TableHead>
                                        <TableRow>
                                            <TableCell>{t("Recent onderhouden")}</TableCell>
                                            <TableCell align="right"></TableCell>
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>
                                        {loading ?
                                            Array.from({ length: 5 }).map((_, index) => (
                                                <TableRow key={index} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                                                    <TableCell>
                                                        <Skeleton variant="rounded" />
                                                    </TableCell>
                                                    <TableCell>
                                                        <Skeleton variant="rounded" />
                                                    </TableCell>
                                                </TableRow>
                                            ))
                                            :
                                            garageOverview?.recentServiceLogs?.map((row, index) => (
                                                <TableRow key={index} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                                                    <TableCell component="th">
                                                        {row.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                                                    </TableCell>
                                                    <TableCell align="right">
                                                        {<Chip
                                                            label={getFormatedLicense(row.vehicleLicensePlate!)}
                                                            variant="outlined"
                                                            onClick={() => gotoVehiclePage(getFormatedLicense(row.vehicleLicensePlate!))}
                                                        />}
                                                    </TableCell>
                                                </TableRow>
                                            ))
                                        }
                                    </TableBody>
                                </Table>
                            </TableContainer>
                        </Grid>
                    }
                    {isMobile &&
                        <>
                            <Grid item xs={12} sm={6}>
                                <TableContainer component={Paper}>
                                    <Table aria-label="simple table">
                                        <TableHead>
                                        <TableRow>
                                            <TableCell>{t("Recent onderhouden")}</TableCell>
                                                <TableCell align="right"></TableCell>
                                            </TableRow>
                                        </TableHead>
                                        <TableBody>
                                        {loading ?
                                            Array.from({ length: 5 }).map((_, index) => (
                                                <TableRow key={index} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                                                    <TableCell>
                                                        <Skeleton variant="rounded" />
                                                    </TableCell>
                                                    <TableCell>
                                                        <Skeleton variant="rounded" />
                                                    </TableCell>
                                                </TableRow>
                                            ))
                                            :
                                            garageOverview?.recentServiceLogs?.map((row, index) => (
                                                <TableRow key={index} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                                                    <TableCell component="th">
                                                        {row.date!.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
                                                    </TableCell>
                                                    <TableCell align="right">
                                                        {<Chip
                                                            label={getFormatedLicense(row.vehicleLicensePlate!)}
                                                            variant="outlined"
                                                            onClick={() => gotoVehiclePage(getFormatedLicense(row.vehicleLicensePlate!))}
                                                        />}
                                                    </TableCell>
                                                </TableRow>
                                            ))
                                        }
                                        </TableBody>
                                    </Table>
                                </TableContainer>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <TableContainer component={Paper}>
                                    <Table aria-label="simple table">
                                        <TableHead>
                                            <TableRow>
                                                <TableCell>
                                                    {t("Alle Diensten")}
                                                    {garageOverview?.supportedServices && ` (${garageOverview?.supportedServices?.length})`}
                                                </TableCell>
                                            </TableRow>
                                        </TableHead>
                                        <TableBody>
                                            {loading ?
                                                Array.from({ length: 5 }).map((_, index) => (
                                                    <TableRow key={index} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                                                        <TableCell>
                                                            <Skeleton variant="rounded" />
                                                        </TableCell>
                                                    </TableRow>
                                                ))
                                                :
                                                garageOverview?.supportedServices?.map((row, index) => (
                                                    <TableRow
                                                        key={index}
                                                        sx={{
                                                            '&:last-child td, &:last-child th': { border: 0 },
                                                            '&:hover': {
                                                                backgroundColor: 'rgba(0, 0, 0, 0.08)', // or any color you want
                                                                cursor: 'pointer'
                                                            }
                                                        }}
                                                        onClick={gotoServices}>
                                                        <TableCell align="left">
                                                            {row.title ? row.title : t(`serviceTypes:${GarageServiceType[row.type!]}.Title`)}
                                                        </TableCell>
                                                    </TableRow>
                                                ))
                                            }
                                        </TableBody>
                                    </Table>
                                </TableContainer>
                            </Grid>
                        </>
                    }
                </Grid>
            </Container>
        </Box>
    );
}
