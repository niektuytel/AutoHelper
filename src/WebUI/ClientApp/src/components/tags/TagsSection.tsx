import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import { useParams } from "react-router-dom";

import ITag from "../../interfaces/tag/ITag";
import TagItems from "./TagItems";
//import { httpGetTags } from "../../services/TagService";
// import { setErrorStatus, setSuccessStatus } from "../../store/status/StatusActions";
import ITagSupplement, { emptyTagSupplement } from "../../interfaces/tag/ITagSupplement";
//import { httpDeleteProductTag, httpPostProductTag, httpPutProductTag } from "../../services/ProductService";
import EditKeyValueDialog from "../dialog/EditKeyValueDialog";

interface IProps {
    productId: number;
    default_data: ITagSupplement[];
    isAdmin: boolean|undefined;
}

export default ({productId, default_data, isAdmin}:IProps) => {
    const { t } = useTranslation();
    const { id }:any = useParams();
    // const dispatch = use// dispatch();
    const [ isLoading, setIsLoading ] = useState<boolean|undefined>(undefined);
    const [ tags, setTags ] = useState<ITag[]|undefined>(undefined);
    const [ productTag, setProductTag ] = useState<ITagSupplement>(emptyTagSupplement);
    const [ newProductTag, setNewProductTag ] = useState<ITagSupplement>(emptyTagSupplement);
    const [ productTags, setProductTags ] = useState<ITagSupplement[]>(default_data);
    const [ visableEditTags, setVisableEditTags ] = React.useState(false);
    const [ visableCreateTags, setVisableCreateTags ] = React.useState(false);
    
    // Initialize
    useEffect(() => {
        if(isAdmin && productId !== -1)
        {
            if(tags === undefined && isLoading === undefined)
            {
                setIsLoading(true);
                //httpGetTags(
                //    (data:ITag[]) => {
                //        setTags(data);
                //        setIsLoading(false);
                //    },
                //    (message:string) => {
                //        // dispatch(setErrorStatus(message));
                //    }
                //)
            }
        }
    })

    const onCreateTag = () => {
        //httpPostProductTag({...newProductTag, productId:productId}, 
        //    (_id:number) => {
        //        let item = {...newProductTag, id:_id};
        //        let items = [...productTags, item];

        //        setProductTags(items);
        //        setProductTag(emptyTagSupplement)
        //        setNewProductTag(emptyTagSupplement)

        //        setVisableCreateTags(false);
        //        // dispatch(setSuccessStatus("On Success"));
        //    },
        //    (message:string) => {
        //        // dispatch(setErrorStatus(message));
        //    }
        //);
    }
    
    const onEditTag = () => {
        //httpPutProductTag(productTag, 
        //    (id:number) => {
        //        let items = productTags.map(tag => tag.id === id ? productTag : tag);

        //        setProductTags(items);
        //        setProductTag(emptyTagSupplement)

        //        setVisableEditTags(false);
        //        // dispatch(setSuccessStatus("On Success"));
        //    },
        //    (message:string) => {
        //        // dispatch(setErrorStatus(message));
        //    }
        //);
    }
    
    const onDeleteTag = (target:ITagSupplement) => {
        //httpDeleteProductTag(String(productId), target.id,
        //    (_id:number) => {
        //        let items = productTags.filter(tag =>  tag.id !== _id);

        //        setProductTags(items);
        //        // dispatch(setSuccessStatus("On Success"));
        //    },
        //    (message: string) => {
        //        // dispatch(setErrorStatus(message));
        //    }
        //)
    }

    const editTag = (target:ITagSupplement) => {
        setProductTag(target);
        setVisableEditTags(true);
    }

    const getUniqueTags = () => {
        if(!tags) return [];
        let values = tags.filter(tag => (!productTags.find(a => a.tag.name === tag.name) || productTag.tag.name === tag.name));
        return values;
    }
    
    if(id === "-1") return <></>
    return <>
        <TagItems 
            data={productTags} 
            onCreate={() => setVisableCreateTags(true)} 
            onEdit={editTag} 
            onDelete={onDeleteTag} 
            isAdmin={isAdmin}
        />
        <EditKeyValueDialog 
            title={t("edit")} 
            data={{ id: productTag.id, tag:productTag.tag, micrograms:productTag.micrograms }} 
            setData={setProductTag} 
            open={visableEditTags} 
            setOpen={setVisableEditTags} 
            isLoading={isLoading ? true : false} 
            onConfirm={onEditTag}
            tags={getUniqueTags()}
            noteOnKeys={[
                { key:"micrograms", message:t("per_100_gram") + " of product" }
            ]}
        />
        <EditKeyValueDialog 
            title={t("create")} 
            data={{ id: newProductTag.id, tag:newProductTag.tag, micrograms:newProductTag.micrograms }} 
            setData={setNewProductTag} 
            open={visableCreateTags} 
            setOpen={setVisableCreateTags} 
            isLoading={isLoading ? true : false} 
            onConfirm={onCreateTag}
            tags={getUniqueTags()}
            noteOnKeys={[
                { key:"micrograms", message:t("per_100_gram") + " of product" }
            ]}
        />
    </>
}