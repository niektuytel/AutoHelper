import { Box, CircularProgress, Container, Theme, useMediaQuery } from '@mui/material';
import { Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns'
import { MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType, RedirectRequest } from '@azure/msal-browser';

// own imports
import Header from './components/header/Header';
import StatusSnackbar from './components/StatusSnackbar';
import Footer from './components/footer/DefaultFooter';
import SelectVehiclePage from './pages/vehicle/SelectVehiclePage';
import SelectGarage from './pages/garages/SelectGarage';
import { NotFoundPage } from './pages/NotFoundPage';
import { ROUTES } from './constants/routes';
import GarageOverview from './pages/garage-account/overview/GarageOverview';
import GarageSettings from './pages/garage-account/settings/GarageSettings';
import GarageServices from './pages/garage-account/services/GarageServices';
import RoleBasedList from './components/header/components/RoleBasedList';
import { COLORS } from './constants/colors';
import Garage from './pages/garage/Garage';
import GarageServicelogs from './pages/garage-account/servicelogs/GarageServicelogs';
import HomePage from './pages/home/HomePage';
import { ServiceLogDrawerProvider } from './context/ServiceLogDrawerContext';
import { garageLoginRequest } from './authConfig';


function ErrorComponent({ error }: any) {
    return <p>An Error Occurred: {error}</p>;
}

function LoadingComponent() {
    return <>
        <Box
            style={{
                position: "relative",
                marginLeft: "10px",
                marginRight: "10px"
            }}
        >
            <Container
                maxWidth="lg"
                style={{
                    padding: "0",
                    textAlign: "center"
                }}
            >
                <Box
                    display="flex"
                    height="30vh"
                    alignItems="center"
                    justifyContent="center"
                >
                    <CircularProgress />
                </Box>
            </Container>
        </Box>
    </>;
}

export default ({ msalInstance }:any) => {
    const location = useLocation();
    const navigate = useNavigate();

    const GarageRouteContent = ({ children }: { children: React.ReactNode }) => {
        const matches = useMediaQuery((theme: Theme) => theme.breakpoints.up('md'));
        const showStaticDrawer = matches && location.pathname.startsWith(`${ROUTES.GARAGE_ACCOUNT.DEFAULT}`);

        return <>
            <Header showStaticDrawer={false} />
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                authenticationRequest={garageLoginRequest}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}
            >
                <Box display="flex" sx={{ marginBottom: "75px", borderBottom: `1px solid ${COLORS.BORDER_GRAY}` }}>
                    {showStaticDrawer && (
                        <Box borderRight={`1px solid ${COLORS.BORDER_GRAY}`}>
                            <RoleBasedList showStaticDrawer={showStaticDrawer} />
                        </Box>
                    )}
                    <Box flexGrow={1}>
                        <Container maxWidth="lg" sx={{ minHeight: "100vh" }} >
                            {children}
                        </Container>
                    </Box>
                </Box>
            </MsalAuthenticationTemplate>
        </>;
    }

    return <>
        <LocalizationProvider dateAdapter={AdapterDateFns}>
            <ServiceLogDrawerProvider>
                <MsalProvider instance={msalInstance}>
                    <Routes>
                        <Route path="*" element={
                            <>
                                <Header showStaticDrawer={false} />
                                <NotFoundPage />
                            </>
                        } />
                        <Route path="/" element={
                            <>
                                <Header showStaticDrawer={false} />
                                <HomePage />
                            </>
                        } />
                        <Route path={`${ROUTES.VEHICLE}/:license_plate`} element={
                            <>
                                <Header showStaticDrawer={false} navigateGoto={() => navigate(-1)} />
                                <SelectVehiclePage />
                            </>
                        } />
                        <Route path={`${ROUTES.GARAGES}/:license_plate/:lat/:lng`} element={
                            <>
                                <Header showStaticDrawer={false} navigateGoto={() => navigate(-1)} />
                                <SelectGarage />
                            </>
                        } />
                        <Route path={`${ROUTES.GARAGE}/:identifier`} element={//?licensePlate= ?lat= &lng=
                            <Garage />
                        } />
                        <Route path={`${ROUTES.GARAGE_ACCOUNT.OVERVIEW}`} element={
                            <GarageRouteContent>
                                <GarageOverview/>
                            </GarageRouteContent>
                        } />
                        <Route path={`${ROUTES.GARAGE_ACCOUNT.SERVICELOGS}`} element={
                            <GarageRouteContent>
                                <GarageServicelogs />
                            </GarageRouteContent>
                        } />
                        <Route path={`${ROUTES.GARAGE_ACCOUNT.SERVICES}`} element={
                            <GarageRouteContent>
                                <GarageServices />
                            </GarageRouteContent>
                        } />
                        <Route path={`${ROUTES.GARAGE_ACCOUNT.SETTINGS}`} element={
                            <GarageRouteContent>
                                <GarageSettings />
                            </GarageRouteContent>
                        } />
                    </Routes>
                </MsalProvider>
            </ServiceLogDrawerProvider>
            <StatusSnackbar />
            <Footer />
        </LocalizationProvider>
    </>;
}

