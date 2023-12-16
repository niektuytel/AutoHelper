import { Box, Card, CardContent, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate } from "react-router-dom";
import PendingActionsIcon from '@mui/icons-material/PendingActions';

// own imports
import { ROUTES } from "../../../../constants/routes";

interface IProps {
    loading: boolean;
    totalServiceLogs: number;
}

export default ({ loading, totalServiceLogs }: IProps) => {
    const currentYear = new Date().getFullYear();
    const { t } = useTranslation(["translations"]);
    const navigate = useNavigate();
    const location = useLocation();

    const gotoServiceLogs = (e: any) => {
        navigate(`${ROUTES.GARAGE_ACCOUNT.SERVICELOGS}/`, { state: { from: location } });
    }

    return <>
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
                        {loading ? "..." : totalServiceLogs}<br />
                        {t("GarageAccount.Overview.ServicesIn.Title")} {currentYear}
                    </Typography>
                </Box>
            </CardContent>
        </Card>
    </>;
}
