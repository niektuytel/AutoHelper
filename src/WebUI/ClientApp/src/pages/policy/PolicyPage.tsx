import React from "react";
import { Container, Typography, Paper } from "@mui/material";
import { useTranslation } from "react-i18next";

interface IPrivacyPolicyProps { }
interface IPaymentRulesProps { }

const WhatsAppPrivacyPolicy: React.FC<IPrivacyPolicyProps> = () => {
    const { t } = useTranslation();

    return (
        <Paper elevation={1} sx={{p: 1, m: 2, mt: "75px"}}>
            <Typography variant="h5">{t('privacyPolicy.title')}</Typography>
            <Typography variant="body1">{t('privacyPolicy.content')}</Typography>
        </Paper>
    );
};

const PaymentRules: React.FC<IPaymentRulesProps> = () => {
    const { t } = useTranslation();

    return (
        <Paper elevation={1} sx={{ p: 1, m: 2 }}>
            <Typography variant="h5">{t('paymentRules.title')}</Typography>
            <Typography variant="body1">{t('paymentRules.content')}</Typography>
        </Paper>
    );
};

export default function PolicyAndRulesPage() {
    return <>
        <Container>
            <WhatsAppPrivacyPolicy />
            <PaymentRules />
        </Container>
    </>;
}
