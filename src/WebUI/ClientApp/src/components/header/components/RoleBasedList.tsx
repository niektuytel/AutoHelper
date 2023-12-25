﻿import React, { useContext, useEffect, useState } from 'react';
import { List, ListItem, ListItemIcon, ListItemText, ListItemButton, Collapse } from "@mui/material";
import DashboardIcon from '@mui/icons-material/Dashboard';
import SearchIcon from '@mui/icons-material/Search';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import BuildIcon from '@mui/icons-material/Build';
import GroupIcon from '@mui/icons-material/Group';
import AddIcon from '@mui/icons-material/Add';
import SettingsIcon from '@mui/icons-material/Settings';
import ServicelogsIcon from '@mui/icons-material/Notes';
import AccountBoxIcon from '@mui/icons-material/AccountBox';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import HomeIcon from '@mui/icons-material/Home';
import { useLocation, useNavigate } from "react-router-dom";

// custom imports
import { ROUTES } from '../../../constants/routes';
import { ROLES } from '../../../constants/roles';
import { useTranslation } from 'react-i18next';
import { useAuth0 } from '@auth0/auth0-react';
import useConfirmationStep from '../../../hooks/useConfirmationStep';
import useUserRole from '../../../hooks/useUserRole';
import { use } from 'i18next';

interface RoleBasedListProps {
    setOnMenu?: (value: boolean) => void;
}

export default ({ setOnMenu }: RoleBasedListProps) => {
    const { userRole } = useUserRole()
    const { configurationIndex } = useConfirmationStep();
    const [prevIndex, setPrevIndex] = useState(configurationIndex || 0);
    const [open, setOpen] = useState(false);
    const location = useLocation();
    const navigate = useNavigate();
    const { t } = useTranslation();

    // update design when index changed
    useEffect(() => {
        if (prevIndex != configurationIndex) {
            setPrevIndex(configurationIndex);
        }
    }, [configurationIndex]);


    const handleClick2 = () => setOpen(!open);

    const ListItemLink = ({ primary, icon, to, disabled = false }: { primary: string; icon: JSX.Element; to: string, disabled?: boolean }) => (
        <ListItem
            button
            disabled={disabled}
            onClick={() => { navigate(to, { state: { from: location } }); setOnMenu && setOnMenu(false); }}
            style={location.pathname === to ? { backgroundColor: '#e0e0e0' } : {}}
        >
            <ListItemIcon style={location.pathname === to ? { color: 'black' } : {}}>
                {React.cloneElement(icon, { color: location.pathname === to ? 'black' : 'action' })}
            </ListItemIcon>
            <ListItemText primary={primary} style={location.pathname === to ? { color: 'black' } : {}} />
        </ListItem>
    );

    if (userRole == ROLES.GARAGE) {
        return (
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink disabled={configurationIndex < 2} primary={t('overview_camelcase')} icon={<DashboardIcon />} to={ROUTES.GARAGE_ACCOUNT.OVERVIEW} />
                <ListItemLink disabled={configurationIndex < 2} primary={t('servicelogs_camelcase')} icon={<ServicelogsIcon />} to={ROUTES.GARAGE_ACCOUNT.SERVICELOGS} />
                <ListItemLink disabled={configurationIndex < 2} primary={t('services_camelcase')} icon={<BuildIcon />} to={ROUTES.GARAGE_ACCOUNT.SERVICES} />
                <ListItemLink primary={t('settings_camelcase')} icon={<SettingsIcon />} to={ROUTES.GARAGE_ACCOUNT.SETTINGS} />
            </List>
        );
    }
    else if (userRole == ROLES.USER) {
        return (
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink primary={t('vehicle_search_camelcase')} icon={<SearchIcon />} to={ROUTES.SELECT_VEHICLE} />
                {/*<ListItemLink primary={t('Add Maintenance')} icon={<AddIcon />} to={`${ROUTES.SELECT_VEHICLE}?open_maintenance_drawer=true`} />*/}
                <ListItemButton onClick={handleClick2}>
                    <ListItemIcon>
                        <AccountBoxIcon />
                    </ListItemIcon>
                    <ListItemText primary={t('account_camelcase')} />
                    {open ? <ExpandLess /> : <ExpandMore />}
                </ListItemButton>
                <Collapse in={open} timeout="auto" unmountOnExit>
                    <List component="div" disablePadding>
                        <ListItemLink primary={t('overview_camelcase')} icon={<HomeIcon />} to={ROUTES.USER.OVERVIEW} />
                    </List>
                </Collapse>
            </List>
        );
    }
    else {
        return (
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink primary={t('vehicle_search_camelcase')} icon={<SearchIcon />} to={ROUTES.SELECT_VEHICLE} />
            </List>
        );
    }
};
