import React, { useState, useRef, useCallback, useEffect } from 'react';
import { Button, Menu, MenuItem, styled, IconButton, Hidden } from "@mui/material";
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from '@azure/msal-react';
import PersonIcon from '@mui/icons-material/Person';
import GarageIcon from '@mui/icons-material/Garage';
import LogoutIcon from '@mui/icons-material/ExitToApp';
import jwtDecode from "jwt-decode";
import { useTranslation } from 'react-i18next';
import LoginIcon from '@mui/icons-material/Login';

// own imports
import { ROLES } from '../../../constants/roles';
import { COLORS } from '../../../constants/colors';
import useUserRole from '../../../hooks/useUserRole';
import useRoleIndex from '../../../hooks/useRoleIndex';
import { garageLoginRequest, userLoginRequest } from '../../../authConfig';
import { t } from 'i18next';

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
    const { setRoleIndex } = useRoleIndex();
    const { instance } = useMsal();

    let activeAccount: any;
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
        if (role === ROLES.GARAGE) {
            instance.loginRedirect(garageLoginRequest).then((response) => {
                setUserRole(ROLES.GARAGE);
                setRoleIndex(100, ROLES.GARAGE);
            });
        } else {
            instance.loginRedirect(userLoginRequest).then((response) => {
                console.log(response);


                //let activeAccount: any;
                //if (instance) {
                //    activeAccount = instance.getActiveAccount();
                //}
                //console.log(activeAccount.idToken);


                setUserRole(ROLES.USER);
                setRoleIndex(100, ROLES.USER);
            });
        }

        handleClose();
    };

    const handleLogout = () => {
        instance.logoutRedirect({
            account: activeAccount,
        }).then((response) => {
            setUserRole("");
            //setroleIndex(100, "");
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
                    onClick={handleLogout}
                    sx={{
                        m: 1,
                        p: 1
                    }}
                >
                    Logout
                </Button>
            }
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
            <>
                <Hidden mdUp>
                    <StyledLoginButton
                        ref={buttonRef}
                        variant="contained"
                        onClick={handleClick}
                        sx={{
                            m: 1,
                            p: 1
                        }}
                    >
                        Login
                    </StyledLoginButton>
                </Hidden>
                <Hidden mdDown>
                    <IconButton
                        onClick={handleClick}
                        style={{ color: 'black' }}
                        aria-label="logout"
                        title={t('login_camelcase')}
                    >
                        <LoginIcon />
                    </IconButton>
                </Hidden>
                <Menu
                    anchorEl={anchorEl}
                    open={Boolean(anchorEl)}
                    onClose={handleClose}
                    sx={{ left: "8px" }}
                    MenuListProps={{ style: { width: buttonRef.current?.offsetWidth } }}
                >
                    <LoginMenu onLogin={handleLogin} />
                </Menu>
            </>
        </UnauthenticatedTemplate>
    </>
};

