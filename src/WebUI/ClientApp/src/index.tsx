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
import { Auth0ProviderWithNavigate } from "./auth0-provider-with-navigate";


import { QueryClient, QueryClientProvider } from 'react-query';

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