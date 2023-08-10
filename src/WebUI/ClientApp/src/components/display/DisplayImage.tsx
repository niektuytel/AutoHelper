import React from "react"; 
import { Paper } from "@mui/material";
import { Swiper, SwiperSlide } from "swiper/react";

// Import Swiper styles
import "swiper/css";
import "swiper/css/pagination";
import "swiper/css/navigation";
import { Pagination, Navigation } from "swiper";

interface IProps {
    image: string|undefined;
    dependOnHeight?: boolean;
}

export default ({image, dependOnHeight}:IProps) => {
    return <>
        <Paper>
            <Swiper 
                style={{height:"400px"}}
                slidesPerView={1}
                spaceBetween={30}
                // loop={true}
                // pagination={{
                //     clickable: true,
                // }}
                // navigation={true}
                modules={[Pagination, Navigation]}
                // className="mySwiper"
            >
                <SwiperSlide>
                    {image && <img src={image} style={{width: "fit-content"}}/>}
                </SwiperSlide>
            </Swiper>
        </Paper>
    </>
}