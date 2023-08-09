import { makeStyles, lighten, Theme } from '@material-ui/core';
import { colorOnStatus } from '../../../i18n/ColorValues';

export default makeStyles((theme: Theme) => ({
    root: {
      paddingLeft: theme.spacing(2),
      paddingRight: theme.spacing(1),
    },
    highlight:
      theme.palette.type === 'light'
        ? {
            color: "#000000",
            backgroundColor: colorOnStatus("delivered"),
          }
        : {
            color: theme.palette.text.primary,
            backgroundColor: theme.palette.secondary.dark,
          },
    title: {
      flex: '1 1 100%',
    },
}));
