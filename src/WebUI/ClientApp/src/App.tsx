import * as React from 'react';
import { useEffect } from 'react';
import { Route, useLocation } from 'react-router';

import Dashboard from './pages/dashboard/Dashboard';
import Checkout from './pages/checkout/Checkout';
import Home from './pages/home/Home';
import Product from './pages/product/Product';
import Products from './pages/products/Products';
import { HasAdminCredential } from './msalConfig';
import Header from './components/header/Header';
import StatusSnackbar from './components/snackbar/StatusSnackbar';
import { Container } from '@material-ui/core';
import Footer from './components/footer/DefaultFooter';
import About from './pages/about/About';



export default () => {
    const { pathname } = useLocation();
    const [isAdmin, SetIsAdmin] = React.useState(HasAdminCredential());

    // Setting page scroll to 0 when changing the route
    useEffect(() => {
        document.documentElement.scrollTop = 0;
    }, [pathname]);

    return <>
        <Header isAdmin={isAdmin}/>
        <Route exact path='/'>
            <Home isAdmin={isAdmin} />
        </Route>

        <Route exact path='/about'>
            <About isAdmin={isAdmin} />
        </Route>
        <Route exact path='/product/:id'>
            <Product isAdmin={isAdmin} />
        </Route>
        <Route exact path='/products'>
            <Products isAdmin={isAdmin}/>
        </Route>
        <Route exact path='/checkout'>
            <Checkout/>
        </Route>
        <Route exact path='/dashboard'>
            <Dashboard isAdmin={isAdmin}/>
        </Route>

        <StatusSnackbar/>
        <Footer/>
    </>;
}
