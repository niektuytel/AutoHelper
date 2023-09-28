import { useEffect } from 'react';
import { Box, Container, Theme, ThemeProvider, useMediaQuery } from '@mui/material';
import { Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom';

// own imports
import Header from './components/header/Header';
import StatusSnackbar from './components/snackbar/StatusSnackbar';
import Footer from './components/footer/DefaultFooter';
import SelectVehicle from './pages/select-vehicle/SelectVehicle';
import SelectGarage from './pages/select-garage/SelectGarage';
import AuthCallback from './pages/AuthCallbackPage';
import { NotFoundPage } from './pages/NotFoundPage';
import { ROUTES } from './constants/routes';
import GarageOverview from './pages/garage/overview/GarageOverview';
import GarageSettings from './pages/garage/settings/GarageSettings';
import GarageEmployees from './pages/garage/employees/GarageEmployees';
import GarageServices from './pages/garage/services/GarageServices';
import GarageAgenda from './pages/garage/agenda/GarageAgenda';
import { ROLES } from './constants/roles';
import AuthenticatedRoute from './components/AuthenticatedRoute';
import RoleBasedList from './components/header/components/RoleBasedList';
import { COLORS } from './constants/colors';
import theme from './constants/theme';

export default () => {
    const location = useLocation();
    const matches = useMediaQuery((theme: Theme) => theme.breakpoints.up('md'));
    const showStaticDrawer = matches && location.pathname.startsWith('/garage');

    const GarageRouteContent = ({ children }: { children: React.ReactNode }) => {
        const navigate = useNavigate();
        const userRole = localStorage.getItem('userRole');
        const confirmationStepIndex = Number(localStorage.getItem('confirmationStepIndex'));

        // redirect when user is on confirmation step 1
        useEffect(() => {
            if (userRole === ROLES.GARAGE) {
                if (location.pathname != ROUTES.GARAGE.SETTINGS && confirmationStepIndex == 1) {
                    console.log("Confirmation is in step 1, we need more information. (redirecting to the settings page)")
                    navigate(ROUTES.GARAGE.SETTINGS);
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
            <Header showStaticDrawer={showStaticDrawer} />
            <Routes>
                <Route path="/callback" element={<AuthCallback />} />{/*TODO: Maybe able to remove???*/}
                <Route path="*" element={<NotFoundPage />} />
                <Route path='/' element={<Navigate to="/select-vehicle" />} />
                <Route path={`${ROUTES.SELECT_VEHICLE}`} element={<SelectVehicle />} />
                <Route path={`${ROUTES.SELECT_VEHICLE}/:license_plate`} element={<SelectVehicle />} />
                <Route path={`${ROUTES.SELECT_GARAGE}/:license_plate/:lat/:lng`} element={<SelectGarage />} />
                <Route path={`${ROUTES.SELECT_SERVICES}/:license_plate/:lat/:lng/:garage_id`} element={<SelectGarage />} />
                <Route path={`${ROUTES.GARAGE.OVERVIEW}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageRouteContent>
                            <GarageOverview/>
                        </GarageRouteContent>
                    </AuthenticatedRoute>
                } />
                <Route path={`${ROUTES.GARAGE.PLANNING}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageRouteContent>
                            <GarageAgenda />
                        </GarageRouteContent>
                    </AuthenticatedRoute>
                } />
                <Route path={`${ROUTES.GARAGE.SERVICES}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageRouteContent>
                            <GarageServices />
                        </GarageRouteContent>
                    </AuthenticatedRoute>
                } />
                <Route path={`${ROUTES.GARAGE.EMPLOYEES}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageRouteContent>
                            <GarageEmployees />
                        </GarageRouteContent>
                    </AuthenticatedRoute>
                } />
                <Route path={`${ROUTES.GARAGE.SETTINGS}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageRouteContent>
                            <GarageSettings />
                        </GarageRouteContent>
                    </AuthenticatedRoute>
                } />
            </Routes>
            <StatusSnackbar />
            <Footer />
        </ThemeProvider>
    </>;
}

