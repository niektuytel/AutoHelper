import * as React from 'react';
import { useLocation } from 'react-router';

import Header from '../../components/header/Header';
import OrderPanelSection from './sections/OrderPanelSection';
import ProductPanelSection from './sections/ProductPanelSection';
import TagSection from './sections/TagSection';
import TagFilterSection from './sections/TagFilterSection';
import { Container } from '@material-ui/core';
import TagTargetSection from './sections/TagTargetSection';
import TagSituationSection from './sections/TagSituationSection';

interface IProps {
    isAdmin: boolean;
}

export default ({isAdmin}:IProps) => {
    const location = useLocation();

    if(!isAdmin) return <></>;
    return <>
        <Container maxWidth="lg" style={{padding:"0"}}>
            {location.hash === "#tags" && <TagSection/>}
            {location.hash === "#tagtargets" && <TagTargetSection/>}
            {location.hash === "#tagfilters" && <TagFilterSection/>}
            {location.hash === "#tagsituations" && <TagSituationSection/>}
            {location.hash === "#orders" && <OrderPanelSection/>}
            {location.hash === "#products" && <ProductPanelSection/>}
            {location.hash === "" && <OrderPanelSection/>}
        </Container>
    </>
    // msalInstance.loginRedirect(loginRequest)
      
}