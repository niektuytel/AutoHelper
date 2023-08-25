import { useEffect } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';

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

export default () => {

    useEffect(() => {
        document.documentElement.scrollTop = 0;
    }, [window.location.pathname]);

    return <>
        <UserProvider>
            <Header/>
            <Routes>
                <Route path="/callback" element={<AuthCallback />} />
                <Route path="*" element={<NotFoundPage />} />
                <Route path='/' element={<Navigate to="/select-vehicle" />} />
                <Route path={`${ROUTES.SELECT_VEHICLE}`} element={<SelectVehicle />} />
                <Route path={`${ROUTES.SELECT_VEHICLE}/:licence_plate` } element={<SelectVehicle />} />
                <Route path={`${ROUTES.SELECT_VEHICLE}/:licence_plate/:lat/:lng`} element={<SelectGarage />} />
                <Route path={`${ROUTES.GARAGE.OVERVIEW}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageOverview/>
                    </AuthenticatedRoute>
                } />
                <Route path={`${ROUTES.GARAGE.AGENDA}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageAgenda />
                    </AuthenticatedRoute>
                } />
                <Route path={`${ROUTES.GARAGE.SERVICES}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageServices />
                    </AuthenticatedRoute>
                } />
                <Route path={`${ROUTES.GARAGE.COLLEAGUES}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageColleagues />
                    </AuthenticatedRoute>
                } />
                <Route path={`${ROUTES.GARAGE.SETTINGS}`} element={
                    <AuthenticatedRoute requiredRole={ROLES.GARAGE}>
                        <GarageSettings />
                    </AuthenticatedRoute>
                } />
            </Routes>
            <StatusSnackbar />
            <Footer />
        </UserProvider>
    </>;
}

