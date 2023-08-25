import React, { useState, MouseEvent, useRef } from 'react';
import { Button, CircularProgress, Menu, MenuItem, styled } from "@mui/material";
import PersonIcon from '@mui/icons-material/Person';
import GarageIcon from '@mui/icons-material/Garage';
import { useAuth0 } from "@auth0/auth0-react";
import { useTranslation } from 'react-i18next';

// custom imports
import { ROUTES, RoutesGarageOverview } from '../../../constants/routes';
import { ROLES } from '../../../constants/roles';
import { COLORS } from '../../../constants/colors';
import { useLocation } from 'react-router';

interface IProps { }

const StyledButton = styled(Button)({
    backgroundColor: COLORS.BLUE,
    '&:hover': {
        backgroundColor: COLORS.HOVERED_BLUE,
        transition: 'background-color 0.3s ease'
    }
});

export default ({  }:IProps) => {
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const buttonRef = useRef<HTMLButtonElement | null>(null);
    const { loginWithRedirect, logout, isAuthenticated, isLoading } = useAuth0();
    const { t } = useTranslation();
    const location = useLocation();

    const handleClick = (event: MouseEvent<HTMLElement>) => setAnchorEl(event.currentTarget);
    const handleClose = () => setAnchorEl(null);

    const handleLogin = (role: string) => {
        if (role === ROLES.GARAGE) {
            loginWithRedirect({
                appState: {
                    returnTo: ROUTES.GARAGE.OVERVIEW
                },
                authorizationParams: {
                    signUpAsGarage: true
                }
            });
        } else {
            loginWithRedirect({
                appState: {
                    returnTo: location.pathname
                }
            });
        }

        handleClose();
    };

    if (isLoading) return <CircularProgress color="secondary" />;
    if (isAuthenticated) {
        return (
            <Button
                variant="contained"
                fullWidth
                onClick={() => {
                    logout({ logoutParams: { returnTo: window.location.origin } })
                }}
                style={{ backgroundColor: 'black' }}
            >
                {t('logout_camelcase')}
            </Button>
        );
    } else {
        // Determine the button width dynamically
        const buttonWidth = buttonRef.current ? buttonRef.current.offsetWidth : 0;

        return (
            <>
                <StyledButton ref={buttonRef} variant="contained" fullWidth onClick={handleClick}>
                    {t('login_camelcase')}
                </StyledButton>
                <Menu
                    anchorEl={anchorEl}
                    open={Boolean(anchorEl)}
                    onClose={handleClose}
                    sx={{
                        left: "-3px", // Ensure the menu is aligned with the button (TODO: why -3, make no sense???)
                    }}
                    MenuListProps={{
                        style: {
                            width: buttonWidth, // Ensure the menu has the same width as the button
                        }
                    }}
                >
                    <MenuItem onClick={() => handleLogin(ROLES.USER)}>
                        <PersonIcon fontSize="small" style={{ marginRight: '8px' }} />
                        {t('as_user')}
                    </MenuItem>
                    <MenuItem onClick={() => handleLogin(ROLES.GARAGE)}>
                        <GarageIcon fontSize="small" style={{ marginRight: '8px' }} />
                        {t('as_garage')}
                    </MenuItem>
                </Menu>
            </>
        );
    }
};
