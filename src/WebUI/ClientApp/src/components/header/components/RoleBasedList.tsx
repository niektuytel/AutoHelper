import React, { useContext, useEffect, useState } from 'react';
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
import MyVehiclesIcon from '@mui/icons-material/Doorbell';
import MyMaintenanceIcon from '@mui/icons-material/ManageHistory';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import HomeIcon from '@mui/icons-material/Home';
import { useLocation, useNavigate, useParams } from "react-router-dom";

// custom imports
import { ROUTES } from '../../../constants/routes';
import { ROLES } from '../../../constants/roles';
import { useTranslation } from 'react-i18next';
import { useAuth0 } from '@auth0/auth0-react';
import useConfirmationStep from '../../../hooks/useConfirmationStep';
import useUserRole from '../../../hooks/useUserRole';
import { use } from 'i18next';
import { ServiceLogDrawerContext } from '../../../context/ServiceLogDrawerContext';

interface RoleBasedListProps {
    setOnMenu?: (value: boolean) => void;
}

export default ({ setOnMenu }: RoleBasedListProps) => {
    const { userRole } = useUserRole()
    const { license_plate } = useParams<{ license_plate: string }>();
    const { configurationIndex } = useConfirmationStep();
    const [prevIndex, setPrevIndex] = useState(configurationIndex || 0);
    const [shouldRender, setShouldRender] = useState(true);
    const [open, setOpen] = useState(false);
    const location = useLocation();
    const navigate = useNavigate();
    const { t } = useTranslation();
    const context = useContext(ServiceLogDrawerContext);
    const { loginWithRedirect, logout, isAuthenticated, isLoading } = useAuth0();

    if (!context) {
        throw new Error("DrawerComponent must be used within a DrawerProvider");
    }

    const { toggleDrawer } = context;

    useEffect(() => {
        // Your existing logic
        if (prevIndex !== configurationIndex) {
            setPrevIndex(configurationIndex);
        }

        // Toggle the shouldRender state to force a re-render
        setShouldRender(prev => !prev);
    }, [configurationIndex, prevIndex]); // Dependency on prevIndex

    const ListItemLink = ({ primary, icon, to, disabled = false }: { primary: string; icon: JSX.Element; to: string, disabled?: boolean }) => (
        <ListItem
            button
            disabled={disabled}
            onClick={() => { navigate(to, { state: { from: location } }); setOnMenu && setOnMenu(false); }}
        >
            {/*style={location.pathname === to ? { backgroundColor: '#e0e0e0' } : {}}*/}
            <ListItemIcon style={location.pathname === to ? { color: 'black' } : {}}>
                {React.cloneElement(icon, { color: location.pathname === to ? 'black' : 'action' })}
            </ListItemIcon>
            <ListItemText primary={primary} style={location.pathname === to ? { color: 'black' } : {}} />
        </ListItem>
    );

    if (!isAuthenticated) {
        return <>
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink primary={t('vehicle_search_camelcase')} icon={<SearchIcon />} to={ROUTES.SELECT_VEHICLE} />
                <ListItemLink primary={t('Header.Menu.MyVehicles.Title')} icon={<MyVehiclesIcon />} to={ROUTES.SELECT_VEHICLE} disabled={true} />
                <ListItemLink primary={t('Header.Menu.MyMaintenance.Title')} icon={<MyMaintenanceIcon />} to={license_plate ? `${ROUTES.SELECT_VEHICLE}/${license_plate}#service_logs` : ROUTES.SELECT_VEHICLE} />
            </List>
        </>;
    }

    if (userRole == ROLES.GARAGE)
    {
        return <>
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink primary={t('vehicle_search_camelcase')} icon={<SearchIcon />} to={ROUTES.SELECT_VEHICLE} />
                <ListItemLink disabled={configurationIndex < 2} primary={t('overview_camelcase')} icon={<DashboardIcon />} to={ROUTES.GARAGE_ACCOUNT.OVERVIEW} />
                <ListItemLink disabled={configurationIndex < 2} primary={t('servicelogs_camelcase')} icon={<ServicelogsIcon />} to={ROUTES.GARAGE_ACCOUNT.SERVICELOGS} />
                <ListItemLink disabled={configurationIndex < 2} primary={t('services_camelcase')} icon={<BuildIcon />} to={ROUTES.GARAGE_ACCOUNT.SERVICES} />
                <ListItemLink primary={t('settings_camelcase')} icon={<SettingsIcon />} to={ROUTES.GARAGE_ACCOUNT.SETTINGS} />
            </List>
        </>;
    }
    else
    {
        return <>
            <List component="nav" sx={{ width: "250px" }}>
                <ListItemLink primary={t('vehicle_search_camelcase')} icon={<SearchIcon />} to={ROUTES.SELECT_VEHICLE} />
            </List>
        </>;
    }
};
