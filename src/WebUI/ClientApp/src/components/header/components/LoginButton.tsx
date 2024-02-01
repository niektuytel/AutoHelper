import React, { useState, useRef, useCallback } from 'react';
import { Button, CircularProgress, Menu, MenuItem, styled, IconButton, Skeleton } from "@mui/material";
import PersonIcon from '@mui/icons-material/Person';
import GarageIcon from '@mui/icons-material/Garage';
import LogoutIcon from '@mui/icons-material/ExitToApp';
import { useAuth0 } from "@auth0/auth0-react";
import { useTranslation } from 'react-i18next';
import { useLocation } from 'react-router';
import { ROUTES } from '../../../constants/routes';
import { ROLES } from '../../../constants/roles';
import { COLORS } from '../../../constants/colors';
import useUserRole from '../../../hooks/useUserRole';
import useConfirmationStep from '../../../hooks/useConfirmationStep';
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from '@azure/msal-react';
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
    const location = useLocation();
    //const { loginWithRedirect, logout, isAuthenticated, isLoading } = useAuth0();
    const { userRole, setUserRole } = useUserRole()
    const { setConfigurationIndex } = useConfirmationStep();
    const { instance } = useMsal();

    let activeAccount;

    if (instance) {
        activeAccount = instance.getActiveAccount();
    }


    const handleClick = useCallback((event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    }, []);

    const handleClose = useCallback(() => {
        setAnchorEl(null);
    }, []);

    const handleLogin = (role: string) => {
        instance.loginRedirect(userLoginRequest)
            .catch((error) => console.log(error));


        //if (role === ROLES.GARAGE) {
        //    instance.loginRedirect(garageLoginRequest).catch(e => {
        //        console.log(e);
        //    });
        //} else {
        //    instance.loginRedirect(userLoginRequest).catch(e => {
        //        console.log(e);
        //    });
        //}

        //const redirectSettings = role === ROLES.GARAGE
        //    ? {
        //        returnTo: ROUTES.GARAGE_ACCOUNT.OVERVIEW
        //    }
        //    : {
        //        returnTo: ROUTES.SELECT_VEHICLE
        //    };


        //loginWithRedirect({
        //    appState: redirectSettings
        //});

        setUserRole(role);
        setConfigurationIndex(100, userRole);
        handleClose();
    };


    const handleLogout = () => {
        instance.logoutRedirect({
            account: instance.getActiveAccount(),
        });
        //instance.logoutRedirect({
        //    postLogoutRedirectUri: window.location.origin,
        //});

        //logout({ logoutParams: { returnTo: window.location.origin } });

        setUserRole("");
        handleClose();
    };


    const { accounts } = useMsal();

    return <>
        <AuthenticatedTemplate>
            <p>Signed in as: {accounts[0]?.username}</p>
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

    //if (isLoading) return <Skeleton variant='rounded' width="100px" height="100%" />;

    //if (asIcon) return <LogoutButtonIcon onLogout={handleLogout} />;

    //if (isAuthenticated) {
    //    return (
    //        <Button
    //            variant="contained"
    //            fullWidth
    //            onClick={handleLogout}
    //            sx={{ height: "100%" }}
    //        >
    //            Logout
    //        </Button>
    //    );
    //} else {
    //    return (
    //    );
    //}
};
