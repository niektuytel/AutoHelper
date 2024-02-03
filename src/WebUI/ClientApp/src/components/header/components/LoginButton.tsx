import React, { useState, useRef, useCallback, useEffect } from 'react';
import { Button, Menu, MenuItem, styled, IconButton } from "@mui/material";
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from '@azure/msal-react';
import PersonIcon from '@mui/icons-material/Person';
import GarageIcon from '@mui/icons-material/Garage';
import LogoutIcon from '@mui/icons-material/ExitToApp';
import jwtDecode from "jwt-decode";
import { useTranslation } from 'react-i18next';


// own imports
import { ROLES } from '../../../constants/roles';
import { COLORS } from '../../../constants/colors';
import useUserRole from '../../../hooks/useUserRole';
import useRoleIndex from '../../../hooks/useRoleIndex';
import { garageLoginRequest, userLoginRequest } from '../../../authConfig';

interface IProps {
    asIcon?: boolean;
}

const StyledLoginButton = styled(Button)(({ theme }) => ({
    backgroundColor: COLORS.BLUE,
    '&:hover': {
        backgroundColor: COLORS.HOVERED_BLUE,
        transition: theme.transitions.create('background-color', {
            duration: theme.transitions.duration.short,
        }),
    },
}));

const LogoutButtonIcon = ({ onLogout }: { onLogout: () => void }) => {
    const { t } = useTranslation();
    return (
        <IconButton
            onClick={onLogout}
            style={{ color: 'black' }}
            aria-label="logout"
            title={t('logout_camelcase')}
        >
            <LogoutIcon />
        </IconButton>
    );
};

const LoginMenu = ({ onLogin }: { onLogin: (role: string) => void }) => {
    const { t } = useTranslation();
    return (
        <>
            <MenuItem onClick={() => onLogin(ROLES.USER)}>
                <PersonIcon fontSize="small" style={{ marginRight: '8px' }} />
                {t('as_user')}
            </MenuItem>
            <MenuItem onClick={() => onLogin(ROLES.GARAGE)}>
                <GarageIcon fontSize="small" style={{ marginRight: '8px' }} />
                {t('as_garage')}
            </MenuItem>
        </>
    );
};

interface IProps {
    asIcon?: boolean;
}

export default ({ asIcon }:IProps) => {
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const buttonRef = useRef<HTMLButtonElement | null>(null);
    const { userRole, setUserRole } = useUserRole()
    const { setConfigurationIndex } = useRoleIndex();
    const { instance } = useMsal();

    let activeAccount;

    if (instance) {
        activeAccount = instance.getActiveAccount();
    }


    useEffect(() => {
        const getAccessToken = async () => {
            const activeAccount = instance.getActiveAccount();
            if (!activeAccount) {
                // Handle the case where there is no active account
                console.error('No active account! Verify a user is signed in.');
                return;
            }

            if (userRole == ROLES.GARAGE) {
                try {
                    // Define the scopes/request details
                    const silentRequest = {
                        scopes: garageLoginRequest.scopes,
                        account: activeAccount
                    };

                    // Attempt to get the token silently
                    const response = await instance.acquireTokenSilent(silentRequest);
                    const accessToken = response.accessToken;

                    // Extract the roles from the token
                    const decodedToken: any = jwtDecode(accessToken); // You'll need a library like jwt-decode
                    // Now you have the user's roles
                    console.log('User roles:', decodedToken);
                } catch (error) {
                    // If silent token acquisition fails, you can fallback to interactive method
                    console.error('Silent token acquisition failed, attempting interactive method', error);
                    // You can implement acquireTokenPopup here as a fallback
                }
            } else {
                try {
                    // Define the scopes/request details
                    const silentRequest2 = {
                        scopes: userLoginRequest.scopes,
                        account: activeAccount
                    };

                    // Attempt to get the token silently
                    const response2 = await instance.acquireTokenSilent(silentRequest2);
                    const accessToken2 = response2.accessToken;

                    // Extract the roles from the token
                    const decodedToken2: any = jwtDecode(accessToken2);
                    console.log('User roles2:', decodedToken2);
                } catch (error) {
                    // If silent token acquisition fails, you can fallback to interactive method
                    console.error('Silent token acquisition failed, attempting interactive method', error);
                    // You can implement acquireTokenPopup here as a fallback
                }
            }
        };

        getAccessToken();
    }, [instance]);

    const handleClick = useCallback((event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    }, []);

    const handleClose = useCallback(() => {
        setAnchorEl(null);
    }, []);

    const handleLogin = (role: string) => {
        if (role === ROLES.GARAGE) {
            instance.loginRedirect(garageLoginRequest).then((response) => {
                setUserRole(ROLES.GARAGE);
                setConfigurationIndex(100, ROLES.GARAGE);
            });
        } else {
            instance.loginRedirect(userLoginRequest).then((response) => {
                setUserRole(ROLES.USER);
                setConfigurationIndex(100, ROLES.USER);
            });
        }

        handleClose();
    };

    const handleLogout = () => {
        instance.logoutRedirect({
            account: instance.getActiveAccount(),
        }).then((response) => {
            setUserRole("");
            //setConfigurationIndex(100, "");
        });

        handleClose();
    };

    return <>
        <AuthenticatedTemplate>
            {asIcon ?
                <LogoutButtonIcon onLogout={handleLogout} />
                :
                <Button
                    variant="contained"
                    fullWidth
                    onClick={handleLogout}
                    sx={{ height: "100%" }}
                >
                    Logout
                </Button>
            }
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
            <>
                <StyledLoginButton ref={buttonRef} variant="contained" fullWidth onClick={handleClick}>
                    Login
                </StyledLoginButton>
                <Menu
                    anchorEl={anchorEl}
                    open={Boolean(anchorEl)}
                    onClose={handleClose}
                    sx={{ left: "-3px" }}
                    MenuListProps={{ style: { width: buttonRef.current?.offsetWidth } }}
                >
                    <LoginMenu onLogin={handleLogin} />
                </Menu>
            </>
        </UnauthenticatedTemplate>
    </>
};

