import * as React from 'react';
import Typography from '@material-ui/core/Typography';
import { useTranslation } from 'react-i18next';

interface IProps {
    paymentId:string
}

export default ({paymentId}:IProps) => {
    const {t} = useTranslation();
    const htmlText = t("error_message").replace("{paymentId}", paymentId);

    return <>
        <Typography variant="subtitle1" style={{margin:"20px", textAlign:"center"}}>
            <div dangerouslySetInnerHTML={{ __html: htmlText }} />
        </Typography>
    </>
}
