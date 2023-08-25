import { Divider, Drawer, Toolbar } from "@mui/material";

// custom imports 
import LoginButton from './LoginButton';
import RoleBasedList from './RoleBasedList';

interface IProps {
    onMenu: boolean;
    setOnMenu: (value: boolean) => void;
}

export default ({ onMenu, setOnMenu }: IProps) => {
    
    return (
        <Drawer open={onMenu} onClose={() => setOnMenu(!onMenu)}>
            <Toolbar
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'flex-end',
                    width: "100%",
                    padding: "12px!important",  // Add padding around the toolbar
                }}
            >
                <LoginButton/>
            </Toolbar>
            <Divider />
            <RoleBasedList setOnMenu={setOnMenu} />
        </Drawer>
    );
}

