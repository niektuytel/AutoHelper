import * as React from 'react';
import { Provider } from "react-redux";
import { HistoryRouter } from "redux-first-history/rr6";
import { store, history } from "./store/";
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import { CookiesProvider } from 'react-cookie';
import { createRoot } from 'react-dom/client';

// own imports
import 'bootstrap/dist/css/bootstrap.css';
import './custom.css'
import './i18n/config';
import App from './App';
import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import reportWebVitals from './reportWebVitals';
import { Auth0ProviderWithNavigate } from "./auth0-provider-with-navigate";

const element = document.getElementById('root');
const root = createRoot(element!);
root.render(
    <Provider store={store}>
        <HistoryRouter history={history}>
            <Auth0ProviderWithNavigate>
                <CookiesProvider>
                    <ThemeProvider theme={createTheme()}>
                        <CssBaseline />
                        <App />
                    </ThemeProvider>
                </CookiesProvider>
            </Auth0ProviderWithNavigate>
        </HistoryRouter>
    </Provider>
);

            
// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
serviceWorkerRegistration.register();

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();