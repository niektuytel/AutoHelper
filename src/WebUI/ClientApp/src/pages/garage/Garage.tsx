import React, { useEffect, useState } from "react";
import { Autocomplete, Box, Button, CircularProgress, Container, Divider, Grid, Pagination, Paper, Skeleton, TextField, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useParams } from "react-router-dom";

// local
import { HashValues } from "../../i18n/HashValues";
import ImageLogo from "../../components/logo/ImageLogo";
import { colorOnIndex } from "../../i18n/ColorValues";
import { COLORS } from "../../constants/colors";
import { GarageLookupDto, PaginatedListOfGarageLookupBriefDto } from "../../app/web-api-client";
import { useQueryClient } from "react-query";
import useGarage from "./useGarage";
import Header from "../../components/header/Header";

interface IProps {
}

export default ({ }: IProps) => {
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);
    const { identifier } = useParams();
    const licensePlate = queryParams.get('licensePlate');
    const lat = queryParams.get('lat');
    const lng = queryParams.get('lng');

    const { loading, garageLookup, fetchGarageLookupByPlate } = useGarage(identifier!, licensePlate);

    if (!loading && garageLookup?.garageId)
    {
        // TODO: handle garage specific page
    }

    return <>
        <Header garageLookupIsLoading={loading} garageLookup={garageLookup} showStaticDrawer={false} />
        <Container>
            <Paper
                style={{
                    backgroundImage: `url(data:image/jpeg;base64,${garageLookup?.largeData?.firstPlacePhoto})`,
                    backgroundSize: 'cover',
                    backgroundPosition: 'center',
                    backgroundRepeat: 'no-repeat',
                    maxWidth: '100%',
                    height: '400px'
                }}
            >
                {/*<Grid container>*/}
                {/*    <Grid item md={6}>*/}
                {/*        <div>*/}
                {/*            <Typography component="h1" variant="h3" color="inherit" gutterBottom>*/}
                {/*                Title of a longer featured blog post*/}
                {/*            </Typography>*/}
                {/*            <Typography variant="h5" color="inherit" paragraph>*/}
                {/*                Multiple lines of text that form the lede, informing new readers quickly and*/}
                {/*                efficiently about what&apos;s most interesting in this post&apos;s contents…*/}
                {/*            </Typography>*/}
                {/*        </div>*/}
                {/*    </Grid>*/}
                {/*</Grid>*/}
            </Paper>
            <Box id="aboutUs">
                About Us content goes here
            </Box>
            <Box id="services">
                Services content goes here
            </Box>

            {/*<Box sx={{ marginBottom: "75px" }}>*/}
            {/*    <Box sx={{ backgroundColor: COLORS.BLUE, borderBottomLeftRadius: "5px", borderBottomRightRadius: "5px", p:1 }}>*/}
            {/*        <Paper*/}
            {/*            elevation={2}*/}
            {/*            sx={{ p: 1, width: "initial", position: "relative", marginTop:"55px" }}*/}
            {/*        >*/}
            {/*            Background + name + logo*/}
            {/*        </Paper>*/}
            {/*    </Box>*/}
            {/*    <Box>*/}
            {/*        Tabs: [ services, about us, contact ]*/}
            {/*        + Content*/}
            {/*    </Box>*/}
            {/*</Box>*/}
        </Container>
    </>;
    //<Grid container spacing={40}>
    //    {/*{Array(2).map(post => (*/}
    //    {/*    <Grid item key={post.title} xs={12} md={6}>*/}
    //    {/*        <Card>*/}
    //    {/*            <div>*/}
    //    {/*                <CardContent>*/}
    //    {/*                    <Typography component="h2" variant="h5">*/}
    //    {/*                        {post.title}*/}
    //    {/*                    </Typography>*/}
    //    {/*                    <Typography variant="subtitle1" color="textSecondary">*/}
    //    {/*                        {post.date}*/}
    //    {/*                    </Typography>*/}
    //    {/*                    <Typography variant="subtitle1" paragraph>*/}
    //    {/*                        {post.description}*/}
    //    {/*                    </Typography>*/}
    //    {/*                    <Typography variant="subtitle1" color="primary">*/}
    //    {/*                        Continue reading...*/}
    //    {/*                    </Typography>*/}
    //    {/*                </CardContent>*/}
    //    {/*            </div>*/}
    //    {/*            <Hidden xsDown>*/}
    //    {/*                <CardMedia*/}
                               
    //    {/*                    image="data:image/svg+xml;charset=UTF-8,%3Csvg%20width%3D%22288%22%20height%3D%22225%22%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20viewBox%3D%220%200%20288%20225%22%20preserveAspectRatio%3D%22none%22%3E%3Cdefs%3E%3Cstyle%20type%3D%22text%2Fcss%22%3E%23holder_164edaf95ee%20text%20%7B%20fill%3A%23eceeef%3Bfont-weight%3Abold%3Bfont-family%3AArial%2C%20Helvetica%2C%20Open%20Sans%2C%20sans-serif%2C%20monospace%3Bfont-size%3A14pt%20%7D%20%3C%2Fstyle%3E%3C%2Fdefs%3E%3Cg%20id%3D%22holder_164edaf95ee%22%3E%3Crect%20width%3D%22288%22%20height%3D%22225%22%20fill%3D%22%2355595c%22%3E%3C%2Frect%3E%3Cg%3E%3Ctext%20x%3D%2296.32500076293945%22%20y%3D%22118.8%22%3EThumbnail%3C%2Ftext%3E%3C%2Fg%3E%3C%2Fg%3E%3C%2Fsvg%3E" // eslint-disable-line max-len*/}
    //    {/*                    title="Image title"*/}
    //    {/*                />*/}
    //    {/*            </Hidden>*/}
    //    {/*        </Card>*/}
    //    {/*    </Grid>*/}
    //    {/*))}*/}
    //</Grid>
    //<Grid container spacing={40}>
        //    {/* Main content */}
        //    <Grid item xs={12} md={8}>
        //        <Typography variant="h6" gutterBottom>
        //            From the Firehose
        //        </Typography>
        //        <Divider />
        //        {/*{posts.map(post => (*/}
        //        {/*    <Markdown key={post.substring(0, 40)}>*/}
        //        {/*        {post}*/}
        //        {/*    </Markdown>*/}
        //        {/*))}*/}
        //    </Grid>
        //    {/* End main content */}
        //    {/* Sidebar */}
        //    <Grid item xs={12} md={4}>
        //        <Paper elevation={0}>
        //            <Typography variant="h6" gutterBottom>
        //                About
        //            </Typography>
        //            <Typography>
        //                Etiam porta sem malesuada magna mollis euismod. Cras mattis consectetur purus sit
        //                amet fermentum. Aenean lacinia bibendum nulla sed consectetur.
        //            </Typography>
        //        </Paper>
        //        <Typography variant="h6" gutterBottom>
        //            Archives
        //        </Typography>
        //        {/*{archives.map(archive => (*/}
        //        {/*    <Typography key={archive}>{archive}</Typography>*/}
        //        {/*))}*/}
        //        {/*<Typography variant="h6" gutterBottom>*/}
        //        {/*    Social*/}
        //        {/*</Typography>*/}
        //        {/*{social.map(network => (*/}
        //        {/*    <Typography key={network}>{network}</Typography>*/}
        //        {/*))}*/}
        //    </Grid>
        //    {/* End sidebar */}
        //</Grid>
}
