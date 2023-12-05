import { Divider, Drawer, Toolbar } from "@mui/material";

// Custom imports 
import LoginButton from './LoginButton';
import RoleBasedList from './RoleBasedList';

interface IProps {
    onMenu: boolean;
    setOnMenu: (value: boolean) => void;
}

export default ({ onMenu, setOnMenu }: IProps) => {
    return (
        <Drawer
            anchor="right"
            open={onMenu}
            onClose={() => setOnMenu(!onMenu)}>
            <Toolbar
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'flex-end',
                    width: "100%",
                    padding: "12px!important",
                }}
            >
                <LoginButton />
            </Toolbar>
            <Divider />
            <RoleBasedList setOnMenu={setOnMenu} />
        </Drawer>
    );
}
