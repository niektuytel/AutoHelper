import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth0 } from '@auth0/auth0-react';

// own imports
import { ROLES } from '../constants/roles';
import { Box, CircularProgress, Container } from '@mui/material';
import useUserClaims from '../hooks/useUserClaims';
import { GARAGE_GUID_PREFIX, ROUTES, RoutesGarageValue } from '../constants/routes';

const AuthenticatedRoute = ({ children, requiredRole }: { children: any, requiredRole?: string }) => {
    const { isAuthenticated, isLoading, loginWithRedirect } = useAuth0();
    const location = useLocation();
    const { garageGUID } = useUserClaims();
    const navigate = useNavigate();


    if (isLoading) {
        return (
            <Box
                style={{
                    position: "relative",
                    marginLeft: "10px",
                    marginRight: "10px"
                }}
            >
                <Container
                    maxWidth="lg"
                    style={{
                        padding: "0",
                        textAlign: "center"
                    }}
                >
                    <Box
                        display="flex"
                        height="30vh"
                        alignItems="center"
                        justifyContent="center"
                    >
                        <CircularProgress />
                    </Box>
                </Container>
            </Box>
        );
    }

    if (!isAuthenticated) {
        loginWithRedirect({
            appState: {
                returnTo: location.pathname
            },
            authorizationParams: {
                signUpAsGarage: requiredRole === ROLES.GARAGE
            }
        });
        return null;
    }

    // redirect to location with an replace on prefix
    // or togarage overview if invalid garage_guid is given
    if (requiredRole === ROLES.GARAGE && garageGUID) {
        if (location.pathname.includes(GARAGE_GUID_PREFIX)) {
            navigate(RoutesGarageValue(location.pathname, garageGUID!))
        }
        else if (!location.pathname.includes(garageGUID)) {
            navigate(RoutesGarageValue(ROUTES.GARAGE.OVERVIEW, garageGUID!))
        }
    }

    return children;
};


export default AuthenticatedRoute;
