import { useEffect } from 'react';
import { Box, Container, Theme, ThemeProvider, useMediaQuery } from '@mui/material';
import { Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns'

// own imports
import Header from './components/header/Header';
import StatusSnackbar from './components/snackbar/StatusSnackbar';
import Footer from './components/footer/DefaultFooter';
import SelectVehiclePage from './pages/select-vehicle/SelectVehiclePage';
import SelectGarage from './pages/select-garage/SelectGarage';
import AuthCallback from './pages/AuthCallbackPage';
import { NotFoundPage } from './pages/NotFoundPage';
import { ROUTES } from './constants/routes';
import GarageOverview from './pages/garage-account/overview/GarageOverview';
import GarageSettings from './pages/garage-account/settings/GarageSettings';
import GarageServices from './pages/garage-account/services/GarageServices';
import { ROLES } from './constants/roles';
import AuthenticatedRoute from './components/AuthenticatedRoute';
import RoleBasedList from './components/header/components/RoleBasedList';
import { COLORS } from './constants/colors';
import theme from './constants/theme';
import Garage from './pages/garage/Garage';
import GarageServiceLogs from './pages/garage-account/servicelogs/GarageServiceLogs';
import HomePage from './pages/home/HomePage';

export default () => {
    const location = useLocation();
    const navigate = useNavigate();
    const matches = useMediaQuery((theme: Theme) => theme.breakpoints.up('md'));
    const showStaticDrawer = matches && location.pathname.startsWith('/garage');

    const GarageRouteContent = ({ children }: { children: React.ReactNode }) => {
        const userRole = localStorage.getItem('userRole');
        const confirmationStepIndex = Number(localStorage.getItem('confirmationStepIndex'));

        // redirect when user is on confirmation step 1
        useEffect(() => {
            if (userRole === ROLES.GARAGE) {
                if (location.pathname != ROUTES.GARAGE_ACCOUNT.SETTINGS && confirmationStepIndex == 1) {
                    console.log("Confirmation is in step 1, we need more information. (redirecting to the settings page)")
                    navigate(ROUTES.GARAGE_ACCOUNT.SETTINGS);
                }
            }
        }, [navigate, userRole, confirmationStepIndex]);

        return (
            <Box display="flex" sx={{ marginBottom: "75px", borderBottom: `1px solid ${COLORS.BORDER_GRAY}` }}>
                {showStaticDrawer && (
                    <Box borderRight={`1px solid ${COLORS.BORDER_GRAY}`}>
                        <RoleBasedList />
                    </Box>
                )}
                <Box flexGrow={1}>
                    <Container maxWidth="lg" sx={{ minHeight: "100vh" }} >
                        {children}
                    </Container>
                </Box>
            </Box>
        );
    }


    return <>
        <ThemeProvider theme={theme}>
            <LocalizationProvider dateAdapter={AdapterDateFns}>
                <Routes>
                    <Route path="/callback" element={
                        <>
                            <Header showStaticDrawer={showStaticDrawer} />
                            <AuthCallback />
                        </>
                    } />{/*TODO: Maybe able to remove???*/}
                    {/*<Route path="*" element={*/}
                    {/*    <>*/}
                    {/*        <Header showStaticDrawer={showStaticDrawer} />*/}
                    {/*        <NotFoundPage />*/}
                    {/*    </>*/}
                    {/*} />*/}
                    <Route path='/' element={<Navigate to="/select-vehicle" />} />
                    <Route path={`${ROUTES.SELECT_VEHICLE}`} element={
                        <>
                            <Header showStaticDrawer={false} />
                            <HomePage />
                        </>
                    } />
                    <Route path={`${ROUTES.SELECT_VEHICLE}/:license_plate`} element={
                        <>
                            <Header showStaticDrawer={false} navigateGoto={() => navigate(-1)} />
                            <SelectVehiclePage />
                        </>
                    } />
                    <Route path={`${ROUTES.SELECT_GARAGE}/:license_plate/:lat/:lng`} element={
                        <>
                            <Header showStaticDrawer={false} navigateGoto={() => navigate(-1)} />
                            <SelectGarage />
                        </>
                    } />
                    <Route path={`${ROUTES.GARAGE}/:identifier`} element={
                        <>
                            <Garage />
                        </>
                    } />{/* ?licensePlate= ?lat= &lng=  */}
                    <Route path={`${ROUTES.GARAGE_ACCOUNT.OVERVIEW}`} element={
                        <>
                            <Header showStaticDrawer={showStaticDrawer} />
                            <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                                <GarageRouteContent>
                                    <GarageOverview/>
                                </GarageRouteContent>
                            </AuthenticatedRoute>
                        </>
                    } />
                    <Route path={`${ROUTES.GARAGE_ACCOUNT.SERVICELOGS}`} element={
                        <>
                            <Header showStaticDrawer={showStaticDrawer} />
                            <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                                <GarageRouteContent>
                                    <GarageServiceLogs />
                                </GarageRouteContent>
                            </AuthenticatedRoute>
                        </>
                    } />
                    <Route path={`${ROUTES.GARAGE_ACCOUNT.SERVICES}`} element={
                        <>
                            <Header showStaticDrawer={showStaticDrawer} />
                            <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                                <GarageRouteContent>
                                    <GarageServices />
                                </GarageRouteContent>
                            </AuthenticatedRoute>
                        </>
                    } />
                    <Route path={`${ROUTES.GARAGE_ACCOUNT.SETTINGS}`} element={
                        <>
                            <Header showStaticDrawer={showStaticDrawer} />
                            <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                                <GarageRouteContent>
                                    <GarageSettings />
                                </GarageRouteContent>
                            </AuthenticatedRoute>
                        </>
                    } />
                </Routes>
                <StatusSnackbar />
                <Footer />
            </LocalizationProvider>
        </ThemeProvider>
    </>;
}

