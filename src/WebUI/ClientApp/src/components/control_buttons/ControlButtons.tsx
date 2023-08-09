import React from "react";
import { Button, ButtonGroup } from "@material-ui/core";
import AddIcon from '@material-ui/icons/Add';
import SaveIcon from '@material-ui/icons/Save';
import EditIcon from '@material-ui/icons/Edit';
import DeleteForeverIcon from '@material-ui/icons/DeleteForever';
import { CSSProperties } from "@material-ui/core/styles/withStyles";

const ControlButtonsStyle = (): CSSProperties => ({
    margin:"10px", 
    color: "black",
    textAlign:"center", 
    width:"100%", 
    justifyContent:"center"
})

interface IProps {
    onCreate?: () => void,
    onEdit?: () => void,
    onDelete?: () => void,
    onSave?: () => void,
    size?: "large" | "small" | "medium",
    submitOn?: "create" | "edit" | "delete" | "save",
    containStyle?: boolean,
    style?: CSSProperties,
    isAdmin?: boolean|true 
}

export default ({onCreate, onEdit, onDelete, onSave, size, submitOn, containStyle, style, isAdmin}:IProps) => {
    if(!isAdmin) return <></>;

    return <>
        <ButtonGroup 
            size={size}
            aria-label="Buttons used for create, edit and delete" 
            style={containStyle === false ? style : ControlButtonsStyle()}
        >
            { onCreate !== undefined && ((submitOn === "create") ? 
                <Button type="submit"><AddIcon/></Button> 
                : 
                <Button onClick={onCreate}><AddIcon/></Button>
            )}
            { onEdit !== undefined && ((submitOn === "edit") ? 
                <Button type="submit"><EditIcon/></Button> 
                : 
                <Button onClick={onEdit}><EditIcon/></Button>
            )}
            { onDelete !== undefined && ((submitOn === "delete") ? 
                <Button type="submit"><DeleteForeverIcon/></Button> 
                : 
                <Button onClick={onDelete}><DeleteForeverIcon/></Button>
            )}
            { onSave !== undefined && ((submitOn === "save") ? 
                <Button type="submit"><SaveIcon/></Button> 
                : 
                <Button onClick={onSave}><SaveIcon/></Button>
            )}
        </ButtonGroup>
    </>
}
