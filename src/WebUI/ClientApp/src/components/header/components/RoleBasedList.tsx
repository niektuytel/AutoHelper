import React, { useContext, useEffect, useState } from 'react';
import { List, ListItem, ListItemIcon, ListItemText, ListItemButton, Collapse } from "@mui/material";
import DashboardIcon from '@mui/icons-material/Dashboard';
import SearchIcon from '@mui/icons-material/Search';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import BuildIcon from '@mui/icons-material/Build';
import GroupIcon from '@mui/icons-material/Group';
import SettingsIcon from '@mui/icons-material/Settings';
import AccountBoxIcon from '@mui/icons-material/AccountBox';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import HomeIcon from '@mui/icons-material/Home';
import { useLocation, useNavigate } from "react-router-dom";

// custom imports
import { ROUTES, RoutesGarageAgenda, RoutesGarageColleagues, RoutesGarageOverview, RoutesGarageServices, RoutesGarageSettings } from '../../../constants/routes';
import { ROLES } from '../../../constants/roles';
import { useTranslation } from 'react-i18next';
import useUserClaims from '../../../hooks/useUserClaims';
import { useAuth0 } from '@auth0/auth0-react';

interface RoleBasedListProps {
    setOnMenu: (value: boolean) => void;
}

export default ({ setOnMenu }: RoleBasedListProps) => {
    const [open, setOpen] = useState(false);
    const { userRoles, garageGUID } = useUserClaims();
    const location = useLocation();
    const navigate = useNavigate();
    const { t } = useTranslation();

    const handleClick2 = () => setOpen(!open);

    const ListItemLink = ({ primary, icon, to }: { primary: string; icon: JSX.Element; to: string }) => (
        <ListItem
            button
            onClick={() => { navigate(to); setOnMenu(false); }}
            style={location.pathname === to ? { backgroundColor: '#e0e0e0' } : {}}
        >
            <ListItemIcon style={location.pathname === to ? { color: 'black' } : {}}>
                {React.cloneElement(icon, { color: location.pathname === to ? 'black' : 'action' })}
            </ListItemIcon>
            <ListItemText primary={primary} style={location.pathname === to ? { color: 'black' } : {}} />
        </ListItem>
    );

    if (userRoles?.includes(ROLES.GARAGE)) {
        return (
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink primary={t('overview_camelcase')} icon={<DashboardIcon />} to={RoutesGarageOverview(garageGUID!)} />
                <ListItemLink primary={t('agenda_camelcase')} icon={<CalendarTodayIcon />} to={RoutesGarageAgenda(garageGUID!)} />
                <ListItemLink primary={t('services_camelcase')} icon={<BuildIcon />} to={RoutesGarageServices(garageGUID!)} />
                <ListItemLink primary={t('colleagues_camelcase')} icon={<GroupIcon />} to={RoutesGarageColleagues(garageGUID!)} />
                <ListItemLink primary={t('settings_camelcase')} icon={<SettingsIcon />} to={RoutesGarageSettings(garageGUID!)} />
            </List>
        );
    }
    else if (userRoles) {
        return (
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink primary={t('vehicle_search_camelcase')} icon={<SearchIcon />} to={ROUTES.SELECT_VEHICLE} />
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
