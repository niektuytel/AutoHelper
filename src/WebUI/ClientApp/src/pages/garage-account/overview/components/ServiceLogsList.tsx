import { Chip, Paper, Skeleton, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

// own imports
import { ROUTES } from "../../../../constants/routes";
import { getFormatedLicense } from "../../../../app/LicensePlateUtils";
import { VehicleServiceLogAsGarageDtoItem } from "../../../../app/web-api-client";

interface IProps {
    loading : boolean;
    servicelogs: VehicleServiceLogAsGarageDtoItem[];
}

export default ({ loading, servicelogs }: IProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const navigate = useNavigate();

    const gotoVehiclePage = (licensePlate: string) => {
        navigate(`${ROUTES.SELECT_VEHICLE}/${licensePlate}`);
    }

    return <>
        <TableContainer component={Paper}>
            <Table aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell>{t("GarageAccount.Overview.ServiceLogs.Title")}</TableCell>
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
                        servicelogs?.map((row, index) => (
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
    </>;
}
