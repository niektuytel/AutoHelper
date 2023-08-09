import React, { useRef } from "react";
import { Box, MenuItem, Select } from "@material-ui/core";
import { useTranslation } from 'react-i18next'

import '../../../i18n/config';
import { HashValues } from "../../../i18n/HashValues";
import { useHistory } from "react-router";

interface IProps {
    hash: string;
}

const NavSection = ({ hash }: IProps) => {
    const history = useHistory();
    const { t } = useTranslation();
    
    return <> 
        {hash.startsWith(HashValues.about) && 
            <Box style={{textAlign:"center"}}>
                {/* backgroundColor:"white", */}
                <Select
                    value={hash}
                    onChange={(a) => history.push(`/${a.target.value as string}`)}
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