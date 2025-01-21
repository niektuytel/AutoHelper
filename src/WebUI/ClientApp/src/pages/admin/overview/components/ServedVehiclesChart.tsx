import { Paper, styled } from "@mui/material";
import { useTranslation } from "react-i18next";
import { LineChart } from '@mui/x-charts/LineChart';

// own imports
import { ServiceLogsChartPoint } from "../../../../app/web-api-client";

const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(1),
    textAlign: 'center',
    color: theme.palette.text.secondary,
}));


interface IProps {
    loading: boolean;
    chartPoints: ServiceLogsChartPoint[];
}

export default ({ loading, chartPoints }: IProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);

    let ServicesData = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    let VehicleData  = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    if (!loading && chartPoints) {
        chartPoints.map((row, index) => {
            ServicesData[index] = row.approvedAmount || 0;
            VehicleData[index] = row.vehiclesAmount || 0;
        })
    }

    return <>
        <Item sx={{ height:"300px", width:"100%"}}>
            <LineChart
                series={[
                    { data: ServicesData, label: t("GarageAccount.Overview.Chart.Services") },
                    { data: VehicleData, label: t("GarageAccount.Overview.Chart.Vehicles") },
                ]}
                xAxis={[{
                    scaleType: 'point', data: [
                        t("Calendar.Jan"),
                        t("Calendar.Feb"),
                        t("Calendar.Mar"),
                        t("Calendar.Apr"),
                        t("Calendar.May"),
                        t("Calendar.Jun"),
                        t("Calendar.Jul"),
                        t("Calendar.Aug"),
                        t("Calendar.Sep"),
                        t("Calendar.Oct"),
                        t("Calendar.Nov"),
                        t("Calendar.Dec")
                    ]}]}
            />
        </Item>
    </>;
}
