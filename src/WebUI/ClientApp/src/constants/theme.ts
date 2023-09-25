import { createTheme } from '@mui/material/styles';
import { COLORS } from './colors';

const theme = createTheme({
    palette: {
        primary: {
            main: COLORS.BLUE,
        },
    },
});

export default theme;
