import { Box, Card, CardContent, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import CheckedAuto from '@mui/icons-material/NoCrash';

// own imports

interface IProps {
    loading: boolean;
    totalServedVehicles: number;
}

export default ({ loading, totalServedVehicles }: IProps) => {
    const currentYear = new Date().getFullYear();
    const { t } = useTranslation(["translations", "serviceTypes"]);

    return <>
        <Card>
            <CardContent sx={{ display: 'flex', flexDirection: 'column', height: "150px", justifyContent: 'space-between' }}>
                <CheckedAuto style={{ fontSize: 40, alignSelf: 'center' }} />
                <Box>
                    <Typography variant="h5" align="center">
                        {loading ? "..." : totalServedVehicles}<br />
                        {t("GarageAccount.Overview.VehiclesIn.Title")} {currentYear}
                    </Typography>
                </Box>
            </CardContent>
        </Card>
    </>;
}
