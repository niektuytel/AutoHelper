import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth0 } from '@auth0/auth0-react';

// own imports
import { ROLES } from '../constants/roles';
import { Box, CircularProgress, Container } from '@mui/material';
import { ROUTES } from '../constants/routes';

const AuthenticatedRoute = ({ children, requiredRole }: { children: any, requiredRole?: string }) => {
    const { isAuthenticated, isLoading, loginWithRedirect } = useAuth0();
    const location = useLocation();
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
            }
        });
        return null;
    }

    return children;
};


export default AuthenticatedRoute;
