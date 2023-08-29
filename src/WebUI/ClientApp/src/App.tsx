import { useEffect } from 'react';
import { Box, Theme, useMediaQuery } from '@mui/material';
import { Navigate, Route, Routes, useLocation } from 'react-router-dom';

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
import GarageColleagues from './pages/garage/colleagues/GarageColleagues';
import GarageServices from './pages/garage/services/GarageServices';
import GarageAgenda from './pages/garage/agenda/GarageAgenda';
import { ROLES } from './constants/roles';
import { UserProvider } from './contexts/UserContext';
import AuthenticatedRoute from './components/AuthenticatedRoute';
import RoleBasedList from './components/header/components/RoleBasedList';
import { COLORS } from './constants/colors';

export default () => {
    const location = useLocation();
    const matches = useMediaQuery((theme: Theme) => theme.breakpoints.up('md'));
    const showStaticDrawer = matches && location.pathname.startsWith('/garage');

    useEffect(() => {
        document.documentElement.scrollTop = 0;
    }, [window.location.pathname]);

    const GarageRouteContent = ({ children }: { children: React.ReactNode }) => (
        <Box display="flex">
            {showStaticDrawer && (
                <Box borderRight={`1px solid ${COLORS.BORDER_GRAY}`}>
                    <RoleBasedList />
                </Box>
            )}
            <Box flexGrow={1}>
                {children}
            </Box>
        </Box>
    );

    return <>
        <UserProvider>
            <Header showStaticDrawer={showStaticDrawer} />
            <Routes>
                <Route path="/callback" element={<AuthCallback />} />
                <Route path="*" element={<NotFoundPage />} />
                <Route path='/' element={<Navigate to="/select-vehicle" />} />
                <Route path={`${ROUTES.SELECT_VEHICLE}`} element={<SelectVehicle />} />
                <Route path={`${ROUTES.SELECT_VEHICLE}/:licence_plate` } element={<SelectVehicle />} />
                <Route path={`${ROUTES.SELECT_VEHICLE}/:licence_plate/:lat/:lng`} element={<SelectGarage />} />
                <Route path={`${ROUTES.GARAGE.OVERVIEW}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageRouteContent>
                            <GarageOverview/>
                        </GarageRouteContent>
                    </AuthenticatedRoute>
                } />
                <Route path={`${ROUTES.GARAGE.AGENDA}`} element={
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
                <Route path={`${ROUTES.GARAGE.COLLEAGUES}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageRouteContent>
                            <GarageColleagues />
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
        </UserProvider>
    </>;
}

