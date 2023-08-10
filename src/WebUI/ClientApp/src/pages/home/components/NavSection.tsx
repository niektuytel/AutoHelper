import React, { useRef } from "react";
import { Box, MenuItem, Select } from "@mui/material";
import { useTranslation } from 'react-i18next'

import '../../../i18n/config';
import { HashValues } from "../../../i18n/HashValues";
import { useNavigate } from "react-router-dom";

interface IProps {
    hash: string;
}

const NavSection = ({ hash }: IProps) => {
    const navigate = useNavigate();
    const { t } = useTranslation();
    
    return <> 
        {hash.startsWith(HashValues.about) && 
            <Box sx={{textAlign:"center"}}>
                {/* backgroundColor:"white", */}
                <Select
                    value={hash}
                    onChange={(a:any) => navigate(`/${a.target.value as string}`)}
                    displayEmpty
                >
                    <MenuItem value={HashValues.about}>{t("our_company")}</MenuItem>
                    {/* <MenuItem value={HashValues.about_delivery}>{t("our_delivery")}</MenuItem>
                    <MenuItem value={HashValues.about_payment}>{t("our_payments")}</MenuItem> */}
                    <MenuItem value={HashValues.about_conditions}>{t("our_conditions")}</MenuItem>
                </Select>
            </Box>
        }

    </>;
};


export default NavSection;