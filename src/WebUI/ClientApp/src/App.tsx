import * as React from 'react';
import { useEffect } from 'react';
import { Route, Routes } from 'react-router-dom';

// own imports
import { HasAdminCredential } from './oidcConfig';
import Home from './pages/home/Home';
import Header from './components/header/Header';
import StatusSnackbar from './components/snackbar/StatusSnackbar';
import Footer from './components/footer/DefaultFooter';
import SelectVehicle from './pages/select-vehicle/SelectVehicle';
import { Redirect } from 'react-router-dom';

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
    console.log(window.location.pathname);
    useEffect(() => {
        document.documentElement.scrollTop = 0;
    }, [window.location.pathname]);

    return <>
        <Header isAdmin={isAdmin} />
        <Routes>
            <Route path='/' element={<Redirect to="/select-vehicle" />} />
            <Route path='/select-vehicle' element={<SelectVehicle isAdmin={isAdmin} />} />
        </Routes>
        <StatusSnackbar />
        <Footer />
    </>;

}