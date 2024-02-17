import React, { useContext, useEffect, useState } from 'react';
import { Divider, List, ListItem, ListItemIcon, ListItemText } from "@mui/material";
import DashboardIcon from '@mui/icons-material/Dashboard';
import SearchIcon from '@mui/icons-material/Search';
import BuildIcon from '@mui/icons-material/Build';
import SettingsIcon from '@mui/icons-material/Settings';
import ServicelogsIcon from '@mui/icons-material/Notes';
import MyVehiclesIcon from '@mui/icons-material/Doorbell';
import MyMaintenanceIcon from '@mui/icons-material/ManageHistory';
import { useLocation, useNavigate, useParams } from "react-router-dom";

// custom imports
import { ROUTES } from '../../../constants/routes';
import { ROLES } from '../../../constants/roles';
import { useTranslation } from 'react-i18next';
import useUserRole from '../../../hooks/useUserRole';
import { ServiceLogDrawerContext } from '../../../context/ServiceLogDrawerContext';
import { useIsAuthenticated, useMsal } from '@azure/msal-react';
import useRoleIndex from '../../../hooks/useRoleIndex';

interface RoleBasedListProps {
    setOnMenu?: (value: boolean) => void;
    showStaticDrawer: boolean;
}

export default ({ setOnMenu, showStaticDrawer }: RoleBasedListProps) => {
    const { userRole } = useUserRole()
    const { license_plate } = useParams<{ license_plate: string }>();
    const { roleIndex } = useRoleIndex();
    const [prevIndex, setPrevIndex] = useState(roleIndex || 0);
    const [shouldRender, setShouldRender] = useState(true);
    const location = useLocation();
    const navigate = useNavigate();
    const { t } = useTranslation();
    const context = useContext(ServiceLogDrawerContext);
    const isAuthenticated = useIsAuthenticated();

    if (!context) {
        throw new Error("DrawerComponent must be used within a DrawerProvider");
    }
    
    useEffect(() => {
        // Your existing logic
        if (prevIndex !== roleIndex) {
            setPrevIndex(roleIndex);
        }

        // Toggle the shouldRender state to force a re-render
        setShouldRender(prev => !prev);
    }, [roleIndex, prevIndex]); // Dependency on prevIndex

    const ListItemLink = ({ primary, icon, to, disabled = false }: { primary: string; icon: JSX.Element; to: string, disabled?: boolean }) => (
        <ListItem
            button
            disabled={disabled}
            onClick={() => { navigate(to, { state: { from: location } }); setOnMenu && setOnMenu(false); }}
            style={location.pathname === to ? { backgroundColor: '#ECECEC' } : {}}
        >
            <ListItemIcon style={location.pathname === to ? { color: 'black' } : {}}>
                {React.cloneElement(icon, { color: location.pathname === to ? 'black' : 'action' })}
            </ListItemIcon>
            <ListItemText primary={primary} style={location.pathname === to ? { color: 'black' } : {}} />
        </ListItem>
    );

    if (!isAuthenticated) {
        return <>
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink primary={t('vehicle_search_camelcase')} icon={<SearchIcon />} to="/" />
                <ListItemLink primary={t('Header.Menu.MyVehicles.Title')} icon={<MyVehiclesIcon />} to={ROUTES.VEHICLE} disabled={true} />
                <ListItemLink primary={t('Header.Menu.MyMaintenance.Title')} icon={<MyMaintenanceIcon />} to={`${ROUTES.VEHICLE}/${license_plate}#service_logs`} disabled={license_plate === undefined} />
            </List>
        </>;
    }

    if (userRole == ROLES.GARAGE)
    {
        if (showStaticDrawer)
        {
            return <>
                <List component="nav" sx={{ width: "250px" }}>
                    <ListItemLink disabled={roleIndex < 2} primary={t('overview_camelcase')} icon={<DashboardIcon />} to={ROUTES.GARAGE_ACCOUNT.OVERVIEW} />
                    <ListItemLink disabled={roleIndex < 2} primary={t('servicelogs_camelcase')} icon={<ServicelogsIcon />} to={ROUTES.GARAGE_ACCOUNT.SERVICELOGS} />
                    <ListItemLink disabled={roleIndex < 2} primary={t('services_camelcase')} icon={<BuildIcon />} to={ROUTES.GARAGE_ACCOUNT.SERVICES} />
                    <ListItemLink primary={t('settings_camelcase')} icon={<SettingsIcon />} to={ROUTES.GARAGE_ACCOUNT.SETTINGS} />
                </List>
            </>;
        }

        const onGarageAccount = location.pathname.startsWith(ROUTES.GARAGE_ACCOUNT.DEFAULT);
        return <>
            <List component="nav" sx={{ width: "250px" }}>
                {!onGarageAccount && (
                    <>
                        <ListItemLink disabled={roleIndex < 2} primary={t('overview_camelcase')} icon={<DashboardIcon />} to={ROUTES.GARAGE_ACCOUNT.OVERVIEW} />
                        <ListItemLink disabled={roleIndex < 2} primary={t('servicelogs_camelcase')} icon={<ServicelogsIcon />} to={ROUTES.GARAGE_ACCOUNT.SERVICELOGS} />
                        <ListItemLink disabled={roleIndex < 2} primary={t('services_camelcase')} icon={<BuildIcon />} to={ROUTES.GARAGE_ACCOUNT.SERVICES} />
                        <ListItemLink primary={t('settings_camelcase')} icon={<SettingsIcon />} to={ROUTES.GARAGE_ACCOUNT.SETTINGS} />
                        <Divider />
                    </>
                )}
                <ListItemLink primary={t('vehicle_search_camelcase')} icon={<SearchIcon />} to="/" />
                <ListItemLink primary={t('Header.Menu.MyVehicles.Title')} icon={<MyVehiclesIcon />} to={ROUTES.VEHICLE} disabled={true} />
                <ListItemLink primary={t('Header.Menu.MyMaintenance.Title')} icon={<MyMaintenanceIcon />} to={`${ROUTES.VEHICLE}/${license_plate}#service_logs`} disabled={license_plate === undefined} />
            </List>
        </>;
    }
    else
    {
        return <>
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink primary={t('vehicle_search_camelcase')} icon={<SearchIcon />} to="/" />
            </List>
        </>;
    }
};
