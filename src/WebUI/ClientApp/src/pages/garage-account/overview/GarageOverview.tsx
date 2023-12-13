import { Box, Container, Grid, Paper, styled, useMediaQuery, useTheme } from "@mui/material";

// own imports
import useGarageOverview from "./useGarageOverview";
import ServedVehiclesCard from "./components/ServedVehiclesCard";
import ServiceLogsCard from "./components/ServiceLogsCard";
import ServicesCard from "./components/ServicesCard";
import ServedVehiclesChart from "./components/ServedVehiclesChart";
import ServiceLogsList from "./components/ServiceLogsList";
import ServicesList from "./components/ServicesList";

interface IProps {
}

export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('lg'));

    const { loading, garageOverview } = useGarageOverview();

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
                        <ServedVehiclesCard loading={loading} totalServedVehicles={garageOverview?.totalServedVehicle || 0} />
                    </Grid>
                    <Grid item xs={isMobile ? 6 : 4}>
                        <ServiceLogsCard loading={loading} totalServiceLogs={garageOverview?.totalApprovedServiceLogs || 0} />
                    </Grid>
                    {!isMobile &&
                        <Grid item xs={4}>
                            <ServicesCard loading={loading} supportedServices={garageOverview?.supportedServices || []} />
                        </Grid>
                    }
                    <Grid item xs={isMobile ? 12 : 8}>
                        <ServedVehiclesChart loading={loading} chartPoints={garageOverview?.chartPoints || []} />
                    </Grid>
                    {!isMobile &&
                        <Grid item xs={4}>
                            <ServiceLogsList loading={loading} servicelogs={garageOverview?.recentServiceLogs || []} />
                        </Grid>
                    }
                    {isMobile &&
                        <>
                            <Grid item xs={12} sm={6}>
                                <ServiceLogsList loading={loading} servicelogs={garageOverview?.recentServiceLogs || []} />
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <ServicesList loading={loading} supportedServices={garageOverview?.supportedServices || []} />
                            </Grid>
                        </>
                    }
                </Grid>
            </Container>
        </Box>
    );
}
