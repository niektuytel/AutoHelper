import { LogLevel } from "@azure/msal-browser";
import { msalInstance } from ".";

export const APPLICATION_ADMIN = "application_admin";
export const APPLICATION_MANAGER = "application_manager";

// For a full list of MSAL.js configuration parameters, 
// visit https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md
export const msalConfig = {
    auth: {
        clientId: "6988a490-5291-4017-a51c-ac6b662833cc",
        authority: "https://login.microsoftonline.com/b6eb5f64-7763-4f5a-8f9d-dbe172cc0d77",
        redirectUri: "/"
    },
    cache: {
        cacheLocation: "localStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false // Set this to "true" if you are having issues on IE11 or Edge
    },
    system: {	
        loggerOptions: {	
            loggerCallback: (level:any, message:any, containsPii:any) => {	
                if (containsPii) {		
                    return;		
                }		
                
                switch (level) {		
                    case LogLevel.Error:		
                        console.error(message);		
                        return;		
                    case LogLevel.Info:		
                        // console.info(message);		
                        return;		
                    case LogLevel.Verbose:		
                        // console.debug(message);		
                        return;		
                    case LogLevel.Warning:		
                        console.warn(message);		
                        return;		
                }	
            }	
        }	
    }
}

// Coordinates and required scopes for your web API
export const apiConfig = {
    resourceUri: "/",
    resourceScopes: [ "api://6988a490-5291-4017-a51c-ac6b662833cc/.default" ]
}

/** 
 * Scopes you enter here will be consented once you authenticate. For a full list of available authentication parameters, 
 * visit https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md
 */
export const loginRequest = {
    scopes: ["openid", "profile", "offline_access", ...apiConfig.resourceScopes]
}

export const logoutRequest = { 
    postLogoutRedirectUri: "/" 
}

export const IsLoggedIn = ():boolean => {
    var accounts =  msalInstance.getAllAccounts();
    
    if(accounts.length > 0)
    {
        msalInstance.setActiveAccount(accounts[0]);
        return true;
    }

    return false;
}

export const HasAdminCredential = ():boolean => {
    const account = msalInstance.getAllAccounts()[0];
    if(account)
    {
        msalInstance.setActiveAccount(account);
        return true;
        
        // const claims:any = account.idTokenClaims;
        // const roles:string[] = claims["roles"];
        // return roles.includes(APPLICATION_ADMIN);
    }
    
    return false;
}
