import { Paper, Skeleton, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate } from "react-router-dom";


// own imports
import { ROUTES } from "../../../../constants/routes";
import { GarageServiceDtoItem, GarageServiceType } from "../../../../app/web-api-client";

interface IProps {
    loading: boolean;
    supportedServices: GarageServiceDtoItem[];
}

export default ({ loading, supportedServices }: IProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const navigate = useNavigate();
    const location = useLocation();

    const gotoServices = (e: any) => {
        navigate(`${ROUTES.GARAGE_ACCOUNT.SERVICES}/`, { state: { from: location } });
    }

    return <>
        <TableContainer component={Paper}>
            <Table aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell>
                            {t("GarageAccount.Overview.AllServices.Title")}
                            {supportedServices && ` (${supportedServices?.length})`}
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
                        supportedServices?.map((row, index) => (
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
    </>;
}
