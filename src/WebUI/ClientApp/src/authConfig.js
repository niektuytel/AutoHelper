import { LogLevel } from "@azure/msal-browser";

/**
 * Enter here the user flows and custom policies for your B2C application
 */
export const b2cPolicies = {
    names: {
        signUpSignIn: process.env.REACT_APP_B2C_POLICY_NAME
    },
    authorities: {
        signUpSignIn: {
            authority: `https://${process.env.REACT_APP_B2C_AUTHORITY_DOMAIN}/${process.env.REACT_APP_B2C_TENANT}/${process.env.REACT_APP_B2C_POLICY_NAME}`,
        },
    },
    authorityDomain: process.env.REACT_APP_B2C_AUTHORITY_DOMAIN,
};

/**
 * Configuration object to be passed to MSAL instance on creation.
 */
export const msalConfig: Configuration = {
    auth: {
        clientId: process.env.REACT_APP_CLIENT_ID,
        authority: b2cPolicies.authorities.signUpSignIn.authority,
        knownAuthorities: [b2cPolicies.authorityDomain],
        redirectUri: process.env.REACT_APP_REDIRECT_URI,
        navigateToLoginRequestUrl: true,
    },
    cache: {
        cacheLocation: "localStorage",
        storeAuthStateInCookie: true,
    },
    system: {
        loggerOptions: {
            loggerCallback: (level, message, containsPii) => {
                if (containsPii) {
                    return;
                }
                switch (level) {
                    case LogLevel.Error:
                        console.error(message);
                        return;
                    case LogLevel.Info:
                        console.info(message);
                        return;
                    case LogLevel.Verbose:
                        console.debug(message);
                        return;
                    case LogLevel.Warning:
                        console.warn(message);
                        return;
                    default:
                        return;
                }
            }
        }
    }
};

/**
 * Add here the endpoints and scopes when obtaining an access token for protected web APIs.
 */
export const protectedResources = {
    autoHelperAPI: {
        scopes: {
            user_write: [process.env.REACT_APP_USER_WRITE_SCOPE],
            garage_write: [process.env.REACT_APP_GARAGE_WRITE_SCOPE],
        },
    },
};

/**
 * Scopes for user and garage login requests.
 */
export const userLoginRequest: RedirectRequest = {
    scopes: ["openid", "profile", ...protectedResources.autoHelperAPI.scopes.user_write]
};
export const garageLoginRequest: RedirectRequest = {
    scopes: ["openid", "profile", ...protectedResources.autoHelperAPI.scopes.garage_write]
};

/**
 * Configuration for MS Graph API.
 */
export const graphConfig = {
    graphMeEndpoint: process.env.REACT_APP_GRAPH_ME_ENDPOINT
};
