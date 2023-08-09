import React, { useEffect, useState } from "react";
import { Link, useHistory, useLocation, useParams } from 'react-router-dom';
import { useDispatch } from "react-redux";
import { Box, Breadcrumbs, Chip, Container, Grid, Hidden } from "@material-ui/core";

import TypesSection from "./sections/TypesSection";
import TagsSection from "../../components/tags/TagsSection";
import DescriptionSection from "./sections/DescriptionSection";
import SpecificationSection from "./sections/SpecificationSection";
import TitleSection from "./sections/TitleSection";
import ConfirmDialog from "../../components/dialog/ConfirmDialog";
import ProductStyle from "./ProductStyle";
import ProgressBox from "../../components/progress/ProgressBox";
import { setErrorStatus, setSuccessStatus } from "../../store/status/StatusActions";
import { httpDeleteProduct, httpGetProduct } from "../../services/ProductService";
import { removeProduct } from "../../store/products/ProductsActions";
import DisplayImage from "../../components/display/DisplayImage";
import IProduct, { emptyProduct } from "../../interfaces/product/IProduct";
import { useTranslation } from "react-i18next";
import ExtraInfoSection from "./sections/ExtraInfoSection";

interface IProps {
    isAdmin: boolean;
}

export default ({isAdmin}:IProps) => {
    const { id }:any = useParams();
    const { t } = useTranslation();
    const classes = ProductStyle();
    const dispatch = useDispatch();
    const history = useHistory();
    const [product, setProduct] = useState<IProduct>(emptyProduct);
    const [loadingDialog, setLoadingDialog] = useState<boolean>(false);
    const [isloading, setIsLoading] = useState<boolean|undefined>(undefined);
    const [prevId, setPrevId] = useState<string>("-1");
    const [image, setImage] = useState<string|undefined>(undefined);
    const [visableConfirm, setVisableConfirm] = React.useState(false);

    // Initialize
    useEffect(() => {
        if(prevId !== id && isloading === undefined)
        {
            setIsLoading(true);
            httpGetProduct(id,
                (product:IProduct) => {
                    setProduct(product);
                    setIsLoading(false);
                    setPrevId(id);
                },
                (message:string) => {
                    dispatch(setErrorStatus(message));
                }
            );
        }
    });

    const onDelete = () => {
        setLoadingDialog(true);
        httpDeleteProduct(id,
            (id:number) => {
                setVisableConfirm(false);
                dispatch(setSuccessStatus(`Success: ${id}`))
                dispatch(removeProduct(id));
                history.push("/products");
                setLoadingDialog(false);
            },
            (message:string) => {
                dispatch(setErrorStatus(message))
                history.push("/products");
                setLoadingDialog(false);
            }
        )
    }

    return <>
        <Container maxWidth="lg" style={{padding:"0"}}>
            <Hidden mdDown>
                <Breadcrumbs style={{ margin:"5px" }}>
                    <Link style={{color:"black", fontSize:"0.95em"}} to="/">{`${t("about")} ${t("us")}`}</Link>
                    <Link style={{color:"black", fontSize:"0.95em"}} to={`/products`}>{t("products")}</Link>
                    <Chip
                        style={{margin:"5px"}}
                        label={product.title}
                        onDelete={(isAdmin) ? () => setVisableConfirm(true) : undefined}
                    />
                </Breadcrumbs>
            </Hidden>
            {(isloading) ?
                <ProgressBox fillScreen/>
                :
                <Grid container className={classes.root} spacing={2}>
                    <Grid item xs={12} md={7} lg={6}>
                        <DisplayImage image={image}/>
                        <Box className={classes.margin_10}>
                            <Hidden mdUp>
                                <TitleSection 
                                    default_data={product.title} 
                                    isAdmin={isAdmin}
                                />
                                <TagsSection 
                                    productId={id}
                                    default_data={product.supplements} 
                                    isAdmin={isAdmin}
                                />
                            </Hidden>
                            <Hidden smDown>
                                <DescriptionSection 
                                    default_data={product.description} 
                                    isAdmin={isAdmin}
                                />
                                <ExtraInfoSection 
                                    default_data={product.extraInformation} 
                                    isAdmin={isAdmin}
                                />
                            </Hidden>
                        </Box>
                    </Grid>
                    <Grid item xs={12} md={5} lg={6}>
                        <Hidden smDown>
                            <TitleSection 
                                default_data={product.title} 
                                isAdmin={isAdmin}
                            />
                            <TagsSection 
                                productId={id}
                                default_data={product.supplements} 
                                isAdmin={isAdmin}
                            />
                        </Hidden>
                        <TypesSection 
                            product={product}
                            setImage={setImage} 
                            isAdmin={isAdmin} 
                        />
                        <Hidden smDown>
                            <SpecificationSection 
                                className={classes.margin_top_10}
                                default_data={product.specifications} 
                                isAdmin={isAdmin}
                            />
                        </Hidden>
                        <Hidden mdUp>
                            <DescriptionSection 
                                default_data={product.description} 
                                isAdmin={isAdmin}
                            />
                            <SpecificationSection 
                                default_data={product.specifications} 
                                isAdmin={isAdmin} 
                                collapsible
                            />
                            <ExtraInfoSection 
                                default_data={product.extraInformation} 
                                isAdmin={isAdmin} 
                                // collapsible
                            />
                        </Hidden>
                    </Grid>
                </Grid>
            }
        </Container>
        <ConfirmDialog 
            open={visableConfirm} 
            setOpen={setVisableConfirm} 
            isLoading={loadingDialog}
            onConfirm={onDelete}
        />
    </>
}