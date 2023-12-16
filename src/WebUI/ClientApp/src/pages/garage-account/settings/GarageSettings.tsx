import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { Box, Button, ButtonGroup, CircularProgress, Divider, Grid, IconButton, Tab, Tabs, Tooltip, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useLocation, useNavigate } from "react-router-dom";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";

// own imports
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import ProfileGeneralBox from "./components/ProfileGeneralBox";
import ProfileContactBox from "./components/ProfileContactBox";
import ProfileConversationSection from "./components/ProfileConversationBox";
import useGarageSettings from "./useGarageSettings";
import ServicesBankingSection from "./components/ServicesBankingSection";
import ServicesGeneralSection from "./components/ServicesGeneralSection";
import ServicesDeliverySection from "./components/ServicesDeliverySection";
import { COLORS } from "../../../constants/colors";
import { ROLES } from "../../../constants/roles";
import React from "react";

interface IProps
{

}

export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [activeSection, setActiveSection] = useState('profile');
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const location = useLocation();
    const { t } = useTranslation();

    const sections = ["profile", "services", "employees", "planning"];
    const handleChange = (event:any, newValue:number) => {
        handleSectionChange(sections[newValue]);
    };

    // redirect when user is on confirmation step 1
    const userRole = localStorage.getItem('userRole');
    const confirmationStepIndex = Number(localStorage.getItem('confirmationStepIndex'));
    const notFound = (confirmationStepIndex == 0)

    const { reset, handleSubmit, control, formState: { errors }, setError, setValue } = useForm();
    const { loading, isError, createGarage, updateGarageSettings, garageSettings } = useGarageSettings(reset, setError, notFound);

    // update hash
    useEffect(() => {
        if (location.hash === "#services") {
            setActiveSection("services");
        } else if (location.hash === "#employees") {
            setActiveSection("employees");
        } else if (location.hash === "#planning") {
            setActiveSection("planning");
        } else {
            setActiveSection("profile");
        }
    }, [location.hash]);

    const handleSectionChange = (section: string) => {
        navigate(`#${section}`, { state: { from: location } });
    }

    const onSubmit = async (data: any) => {

        if (notFound) {
            if (!data.longitude) {
                dispatch(showOnError(t("Select an address")));
                setError("address", {
                    type: "manual",
                    message: t("Select an address")
                });

                return;
            }

            createGarage(data);
        } else {
            updateGarageSettings(data);
        }
    }

    return <>
        <Box pt={4}>
            <Typography variant="h4" gutterBottom>
                {t("settings")}
                {loading ?
                    <CircularProgress size={20} style={{ marginLeft: '10px' }} />
                    :
                    <Tooltip title={t("settings_description")}>
                        <IconButton size="small">
                            <InfoOutlinedIcon fontSize="inherit" />
                        </IconButton>
                    </Tooltip>
                }
            </Typography>
        </Box>
        {!notFound ?
            <Box sx={{ maxWidth: { xs: 320, sm: 480 }} }>
                <Tabs
                    value={sections.indexOf(activeSection)}
                    onChange={handleChange}
                    variant="scrollable"
                    scrollButtons="auto"
                    sx={{ width:"100%" }}
                >
                    <Tab label={t('profile')} />
                    <Tab label={t('services')} disabled />
                    <Tab label={t('employees')} disabled />
                    <Tab label={t('planning')} disabled />
                </Tabs>
            </Box>
            :
            <Box py={3}></Box>
        }
        <Divider style={{ marginBottom: "20px" }} />
        <form onSubmit={handleSubmit(onSubmit)}>
            <Grid container spacing={2}>
                {
                    activeSection === 'profile' ?
                        <>
                            <ProfileGeneralBox control={control} setFormValue={setValue} defaultLocation={garageSettings} notFound={notFound} />
                            <ProfileContactBox control={control} />
                            <ProfileConversationSection control={control} />
                        </>
                        : activeSection === 'services' ?
                            <>
                                <ServicesBankingSection errors={errors} control={control} />
                                <ServicesGeneralSection errors={errors} control={control} />
                                <ServicesDeliverySection errors={errors} control={control} />
                            </>
                            : activeSection === 'employees' ?
                                <>
                                    <ServicesGeneralSection errors={errors} control={control} />
                                    <ServicesDeliverySection errors={errors} control={control} />
                                </>
                                :
                                <>
                                    <ServicesGeneralSection errors={errors} control={control} />
                                    <ServicesDeliverySection errors={errors} control={control} />
                                </>
                }
                <Grid item xs={12}>
                    <Box display="flex" justifyContent="center" alignItems="center" height="90px">
                        {loading ?
                            <Button
                                fullWidth={isMobile}
                                variant="contained"
                                disabled
                                style={{ color: 'white', padding: '10px 30px' }}
                            >
                                <CircularProgress size={24} color="inherit" />
                            </Button>
                            : isError ?
                                <div>
                                    Error fetching garage settings
                                </div>
                                :
                                <Button
                                    fullWidth={isMobile}
                                    type="submit"
                                    variant="contained"
                                    sx={{
                                        backgroundColor: COLORS.BLUE,
                                        color: 'white',
                                        padding: isMobile ? '10px 0' : '10px 30px',
                                        textTransform: 'uppercase',
                                        fontWeight: 'bold',
                                        '&:hover': { backgroundColor: COLORS.HOVERED_BLUE }
                                    }}
                                >
                                    {notFound ? t("register") : t("save")}
                                </Button>
                        }
                    </Box>
                </Grid>
            </Grid>
        </form>
    </>;
};
