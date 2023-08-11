import * as React from 'react';
import { useEffect } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';

// own imports
import { HasAdminCredential } from './oidcConfig';
import Header from './components/header/Header';
import StatusSnackbar from './components/snackbar/StatusSnackbar';
import Footer from './components/footer/DefaultFooter';
import SelectVehicle from './pages/select-vehicle/SelectVehicle';
import SelectGarage from './pages/select-garage/SelectGarage';

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
    const [isAdmin, SetIsAdmin] = React.useState(HasAdminCredential());

    // Setting page scroll to 0 when changing the route
    useEffect(() => {
        document.documentElement.scrollTop = 0;
    }, [window.location.pathname]);

    return <>
        <Header isAdmin={isAdmin} />
        <Routes>
            <Route path='/' element={<Navigate to="/select-vehicle" />} />
            <Route path='/select-vehicle/' element={<SelectVehicle />} />
            <Route path='/select-vehicle/:licence_plate' element={<SelectVehicle />} />
            <Route path='/select-garage/:licence_plate/:lat/:lng' element={<SelectGarage />} />
        </Routes>
        <StatusSnackbar />
        <Footer />
    </>;

}