import { styled } from '@mui/material/styles';
import { AppBar, IconButton, Toolbar, Badge, Chip, Container } from "@mui/material";
import { colorOnIndex } from "../../i18n/ColorValues";

export const StyledDivMarginLeft16 = styled('div')({
    marginLeft: "16px"
});

export const StyledDivMargin5 = styled('div')({
    margin: "5px"
});

export const StyledChip = styled(Chip)({
    margin: "4px",  // Since theme.spacing(0.5) would be 4px by default
});

export const StyledDivHeaderHeight = styled('div')({
    margin: "75px"
});

export const StyledAppBar = styled(AppBar)({
    background: "white"
});

export const StyledToolbar = styled(Toolbar)({
    minHeight: "64px",
    margin: "0",
    padding: "0"
});

export const StyledContainer = styled(Container)({
    padding: 0
});

export const StyledDivCenterVertically = styled('div')({
    display: 'flex',
    alignItems: 'center',
});

export const StyledDivIconGrid = styled('div')({
    textAlign: "right",
    color: "black"
});

export const StyledIconButton = styled(IconButton)({
    color: "black"
});

export const StyledBadge = styled(Badge)({
    '.MuiBadge-badge': {
        backgroundColor: colorOnIndex(0),
        color: "white"
    }
});
