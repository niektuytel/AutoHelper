import { Auth0Provider, AppState } from "@auth0/auth0-react";
import React, { PropsWithChildren } from "react";
import { useNavigate } from "react-router-dom";

interface Auth0ProviderWithNavigateProps {
  children: React.ReactNode;
}

export const Auth0ProviderWithNavigate = ({
  children,
}: PropsWithChildren<Auth0ProviderWithNavigateProps>): JSX.Element | null => {
    const navigate = useNavigate();

    const domain = process.env.REACT_APP_AUTH0_DOMAIN;
    const clientId = process.env.REACT_APP_AUTH0_CLIENT_ID;
    const redirectUri = `${window.location.origin}${process.env.REACT_APP_AUTH0_CALLBACK_PATH}`;
    const audience = process.env.REACT_APP_AUTH0_AUDIENCE;

    const onRedirectCallback = (appState?: AppState) => {
        navigate(appState?.returnTo || window.location.pathname);
    };

    if (!(domain && clientId && redirectUri && audience)) {
        return <div>Invalid Auth0</div>;
    }

    return (
    <Auth0Provider
        domain={domain}
        clientId={clientId}
            authorizationParams={{
            scope: "openid profile email",
            audience: audience,
            redirect_uri: redirectUri,
        }}
        onRedirectCallback={onRedirectCallback}
    >
        {children}
    </Auth0Provider>
    );
};
