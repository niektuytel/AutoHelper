import * as React from 'react';
import { Provider } from "react-redux";
import { HistoryRouter } from "redux-first-history/rr6";
import { store, history } from "./redux/store";
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
import { EventType, PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from './authConfig';


import { QueryClient, QueryClientProvider } from 'react-query';
import theme from './constants/theme';

/**
 * MSAL should be instantiated outside of the component tree to prevent it from being re-instantiated on re-renders.
 * For more, visit: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/getting-started.md
 */
const msalInstance = new PublicClientApplication(msalConfig);

// Default to using the first account if no account is active on page load
if (!msalInstance.getActiveAccount() && msalInstance.getAllAccounts().length > 0) {
    // Account selection logic is app dependent. Adjust as needed for different use cases.
    msalInstance.setActiveAccount(msalInstance.getAllAccounts()[0]);
}

// Optional - This will update account state if a user signs in from another tab or window
msalInstance.enableAccountStorageEvents();

// Listen for sign-in event and set active account
msalInstance.addEventCallback((event: any) => {
    if (event.eventType === EventType.LOGIN_SUCCESS && event.payload.account) {
        const account = event.payload.account;
        msalInstance.setActiveAccount(account);
    }
});

const queryConfig = {
    queries: {
        retry: (failureCount: any, error: any) => {
            if (error && error.response && error.response.status === 404) {
                return false;  // Don't retry for 404 errors
            }
            else if (error && error.response && error.response.status === 400) {
                return false;  // Don't retry for 400 errors
            }
            return failureCount < 3;  // Limit the number of retries
        },
        onError: (error: any) => {
            if (error && error.response && error.response.status !== 404) {
                console.error(error);  // Handle other errors globally, but exclude 404
            }
        }
    }
};

const queryClient = new QueryClient({ defaultOptions: queryConfig });

const element = document.getElementById('root');
const root = createRoot(element!);
root.render(
    <QueryClientProvider client={queryClient}>
        <Provider store={store}>
            <HistoryRouter history={history}>
                <CookiesProvider>
                    <ThemeProvider theme={theme}>
                        <CssBaseline />
                        <App msalInstance={msalInstance} />
                    </ThemeProvider>
                </CookiesProvider>
            </HistoryRouter>
        </Provider>
    </QueryClientProvider>
);

            
// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
serviceWorkerRegistration.register();

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();