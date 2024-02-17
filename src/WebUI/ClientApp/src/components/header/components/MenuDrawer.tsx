import { Box, Divider, Drawer, IconButton, Toolbar, useMediaQuery, useTheme } from "@mui/material";
import CloseIcon from '@mui/icons-material/Close';

// Custom imports 
import LoginButton from './LoginButton';
import RoleBasedList from './RoleBasedList';

interface IProps {
    onMenu: boolean;
    setOnMenu: (value: boolean) => void;
    showStaticDrawer: boolean;
}

export default ({ onMenu, setOnMenu, showStaticDrawer }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    return (
        <Drawer
            anchor="right"
            open={onMenu}
            onClose={() => setOnMenu(!onMenu)}
            sx={{
                '& .MuiDrawer-paper': {
                    width: isMobile ? '100%' : '250px',
                },
            }}
        >
            <Toolbar
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: "space-between",
                    width: "100%",
                    padding: "12px!important"
                }}
            >
                <Box sx={{ flexGrow: 1 }} />
                <IconButton onClick={() => setOnMenu(false)}>
                    <CloseIcon />
                </IconButton>
            </Toolbar>
            <Divider />
            <RoleBasedList setOnMenu={setOnMenu} showStaticDrawer={showStaticDrawer} />
        </Drawer>
    );
}
