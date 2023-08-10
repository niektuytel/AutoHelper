import React, { useState } from "react";
import { Chip, Box } from "@mui/material";
import InfoDialog from "../dialog/InfoDialog";
import { useTranslation } from "react-i18next";
//import TagItemsStyle from "./TagItemsStyle";
import ITagSupplement, { emptyTagSupplement } from "../../interfaces/tag/ITagSupplement";

import { CSSProperties } from "react";
import { colorOnIndex } from "../../i18n/ColorValues";

export const TagStyle = (index: number): CSSProperties => ({
    borderColor: colorOnIndex(index),
    color: "#000",
    borderRadius: "5px",
    marginLeft: "1px"
});

const styles = {
    chips: {
        display: 'flex',
        justifyContent: 'center',
        flexWrap: 'wrap',
        marginBottom: "2px",
        '& > *': {
            margin: "2px",
        }
    },
    chips_ellipsis: {
        display: "-webkit-box",
        lineClamp: 2,
        boxOrient: "vertical",
        overflow: "hidden",
        textOverflow: "ellipsis",
    },
    admin_chip: {
        backgroundColor: "rgb(223 255 212)",
        borderColor: "rgb(223 255 212)",
        color: "#000",
        borderRadius: "5px"
    }
}

interface IProps {
    data: ITagSupplement[],
    ellipsis?: boolean|false,
    isAdmin?: boolean|false,
    onCreate?: () => void
    onEdit?: (tag:ITagSupplement) => void,
    onDelete?: (tag:ITagSupplement) => void,
}

export default ({data, ellipsis, isAdmin, onCreate, onEdit, onDelete}:IProps) => {
    const {t} = useTranslation();
    //const styles = TagItemsStyle();
    const [currentTag, setCurrentTag] = useState<ITagSupplement>(emptyTagSupplement);
    const [showInfo, setShowInfo] = useState(false);
    
    const showInformation = (index:number) => {
        if(!data) return;
        setCurrentTag(data[index]);
        setShowInfo(true);
    }
    
    return <>
        <Box sx={ellipsis ? styles.chips_ellipsis : styles.chips}>
            {data && data.map((productTag:ITagSupplement, index:number) => (productTag.tag) &&
                <Chip 
                    key={`label-chip-${index}`}
                    label={productTag.tag.name}
                    variant="outlined"
                    size="small" 
                    style={TagStyle(index)} 
                    onClick={
                        (isAdmin && onEdit) ? 
                            () => onEdit(productTag) 
                        : 
                            (event:any) => {
                                showInformation(index);
                                event.stopPropagation();
                                event.preventDefault();
                            }
                    }
                    onDelete={
                        (isAdmin && onDelete) ? 
                            () => onDelete(productTag) 
                        : 
                            undefined
                    }
                />
            )}
            {isAdmin && 
                <Chip 
                    label={"New Tag"}
                    variant="outlined"
                    size="small" 
                    className={`${styles.admin_chip}`} 
                    onClick={onCreate}
                />
            }
            <InfoDialog 
                tag={currentTag.tag.name}
                open={showInfo} 
                setOpen={setShowInfo}
                title={`${t("what_is")} ${currentTag.tag.name} ${t("for")}?`}
                content={currentTag.tag.description}
                hasResource={currentTag.tag.hasResource}
            />
        </Box>
    </>
}





