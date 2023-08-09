import 'bootstrap/dist/css/bootstrap.css';
import './custom.css'
import './i18n/config';

import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'connected-react-router';
import { createBrowserHistory } from 'history';
import { Container, CssBaseline } from '@material-ui/core';
import { CookiesProvider } from 'react-cookie';
import { MsalProvider } from "@azure/msal-react";
import { PublicClientApplication } from '@azure/msal-browser/dist/app/PublicClientApplication';

// local
import App from './App';
import configureStore from './store/configureStore';
import Footer from './components/footer/DefaultFooter';
import StatusSnackbar from './components/snackbar/StatusSnackbar';
import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import reportWebVitals from './reportWebVitals';
import { HasAdminCredential, msalConfig } from './msalConfig';

/**
* MSAL should be instantiated outside of the component tree to prevent it from being re-instantiated on re-renders. 
* For more, visit: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/getting-started.md
*/
export const msalInstance = new PublicClientApplication(msalConfig)

// Create browser history to use in the Redux store
const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href') as string;
const history = createBrowserHistory({ basename: baseUrl });

// Get the application-wide store instance, prepopulating with state from the server where available.
const store = configureStore(history);

ReactDOM.render(
    <Provider store={store}>
        <ConnectedRouter history={history}>
            <CookiesProvider>
                <MsalProvider instance={msalInstance}>
                    <CssBaseline />
                    <App />
                </MsalProvider>
            </CookiesProvider>
        </ConnectedRouter>
    </Provider>,
    document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
serviceWorkerRegistration.register();

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();