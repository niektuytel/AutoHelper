import React, { useState } from "react";
import { Chip } from "@material-ui/core";
import InfoDialog from "../dialog/InfoDialog";
import { useTranslation } from "react-i18next";
import TagItemsStyle from "./TagItemsStyle";
import TagStyle from "./TagStyle";
import ITagSupplement, { emptyTagSupplement } from "../../interfaces/tag/ITagSupplement";

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
    const classes = TagItemsStyle();
    const [currentTag, setCurrentTag] = useState<ITagSupplement>(emptyTagSupplement);
    const [showInfo, setShowInfo] = useState(false);
    
    const showInformation = (index:number) => {
        if(!data) return;
        setCurrentTag(data[index]);
        setShowInfo(true);
    }
    
    return <>
        <div className={ellipsis ? classes.chips_ellipsis : classes.chips}>
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
                    className={`${classes.admin_chip}`} 
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
        </div>
    </>
}





