import { Box, Card, CardContent, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import BuildIcon from '@mui/icons-material/Build';


// own imports
import { ROUTES } from "../../../../constants/routes";
import { GarageServiceDtoItem } from "../../../../app/web-api-client";

interface IProps {
    loading: boolean;
    supportedServices: GarageServiceDtoItem[]
}

export default ({ loading, supportedServices }: IProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const navigate = useNavigate();

    const gotoServices = (e: any) => {
        navigate(`${ROUTES.GARAGE_ACCOUNT.SERVICES}/`);
    }

    return <>
        <Card>
            <CardContent 
                sx={{
                    display: 'flex', flexDirection: 'column', height: "150px", justifyContent: 'space-between',
                    '&:hover': {
                        backgroundColor: 'rgba(0, 0, 0, 0.08)',
                        cursor: 'pointer'
                    }
                }}
                onClick={gotoServices}
            >
                <BuildIcon style={{ fontSize: 40, alignSelf: 'center' }} />
                <Box>
                    <Typography variant="h5" align="center">
                        {t("GarageAccount.Overview.AllServicesIn.Title")}<br /> 
                        {loading ? "..." : supportedServices?.length || 0} {t("GarageAccount.Overview.AllServicesIn.Title2")}
                    </Typography>
                </Box>
            </CardContent>
        </Card>
    </>;
}
