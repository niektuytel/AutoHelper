import * as React from 'react';
import { useEffect } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';

// own imports
import Header from './components/header/Header';
import StatusSnackbar from './components/snackbar/StatusSnackbar';
import Footer from './components/footer/DefaultFooter';
import SelectVehicle from './pages/select-vehicle/SelectVehicle';
import SelectGarage from './pages/select-garage/SelectGarage';
import AuthCallback from './pages/login/AuthCallback';
//import { useAuth0 } from '@auth0/auth0-react';
import { NotFoundPage } from './pages/not-found-page';

//TODO: Add routes
//<Route exact path='/about'>
//    <About isAdmin={isAdmin} />
//</Route>
//<Route exact path='/product/:id'>
//    <Product isAdmin={isAdmin} />
//</Route>
//<Route exact path='/products'>
//    <Products isAdmin={isAdmin}/>
//</Route>
//<Route exact path='/checkout'>
//    <Checkout/>
//</Route>
//<Route exact path='/dashboard'>
//    <Dashboard isAdmin={isAdmin}/>
//</Route>

export default () => {
    // Setting page scroll to 0 when changing the route
    useEffect(() => {
        document.documentElement.scrollTop = 0;
    }, [window.location.pathname]);

    return <>
        <Header isAdmin={false} />
        <Routes>
            <Route path='/' element={<Navigate to="/select-vehicle" />} />
            <Route path='/select-vehicle/' element={<SelectVehicle />} />
            <Route path='/select-vehicle/:licence_plate' element={<SelectVehicle />} />
            <Route path='/select-garage/:licence_plate/:lat/:lng' element={<SelectGarage />} />
            <Route path="/auth-callback" element={<AuthCallback />} />
            <Route path="/callback" element={<AuthCallback />} />
            <Route path="*" element={<NotFoundPage />} />
        </Routes>
        <StatusSnackbar />
        <Footer />
    </>;

}