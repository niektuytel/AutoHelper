import React, { useEffect, useState } from "react"
import { Theme, createStyles, makeStyles } from '@material-ui/core/styles';
import Accordion from '@material-ui/core/Accordion';
import AccordionSummary from '@material-ui/core/AccordionSummary';
import AccordionDetails from '@material-ui/core/AccordionDetails';
import Typography from '@material-ui/core/Typography';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { Box, Button, FormControl, Grid, Icon, InputLabel, Paper, Select, TextareaAutosize, TextField } from "@material-ui/core";
import SendIcon from '@material-ui/icons/Send';
import { useTranslation } from "react-i18next";
import IContactEmail, {  emptyContactEmail } from "../../../interfaces/IContactEmail";
import { httpDeleteContactFAQ, httpGetContactFAQs, httpPostContactFAQ, httpPutContactFAQ, httpSendContactMail } from "../../../services/ContactService";
import { useDispatch } from "react-redux";
import { LocalConvenienceStoreOutlined } from "@material-ui/icons";
import ControlButtons from "../../../components/control_buttons/ControlButtons";
import IContactQuestion, { emptyContactQuestion } from "../../../interfaces/IContactQuestion";
import EditKeyValueDialog from "../../../components/dialog/EditKeyValueDialog";
import ConfirmDialog from "../../../components/dialog/ConfirmDialog";
import { setErrorStatus, setSuccessStatus } from "../../../store/status/StatusActions";
import ProgressBox from "../../../components/progress/ProgressBox";

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    heading: {
      fontSize: theme.typography.pxToRem(15)
    },
  }),
);

interface IProps {
    isAdmin: boolean;
}

export default ({isAdmin}:IProps) => {
    const classes = useStyles();
    const dispatch = useDispatch();
    const {t} = useTranslation();
    const [contact, setContact] = useState<IContactEmail>(emptyContactEmail);
    const [invalidItems, setInvalidItems] = useState<string[]>([]);
    const [questions, setQuestions] = useState<IContactQuestion[]|undefined>(undefined);
    const [loading, setLoading] = useState<boolean|undefined>(undefined);
    const [loadingDialog, setLoadingDialog] = useState<boolean>(false);
    const [visableCreate, setVisableCreate] = useState<boolean>(false);
    const [visableUpdate, setVisableUpdate] = useState<boolean>(false);
    const [visableDelete, setVisableDelete] = useState<boolean>(false);
    const [newQuestion, setNewQuestion] = useState<IContactQuestion>(emptyContactQuestion);
    const [currentQuestion, setCurrentQuestion] = useState<IContactQuestion>(emptyContactQuestion);

    useEffect(() => {
        if(loading === undefined && !questions)
        {
            setLoading(true);
            httpGetContactFAQs(
                (data) => {

                    console.log(data);

                    setQuestions(data);
                    setLoading(false);
                },
                (message:string) => {
                    setLoading(false);
                    dispatch(setErrorStatus(message));
                }
            );
        }
    });
    
    const onCreate = () => {
        if(!questions) return;
        setLoadingDialog(true);

        httpPostContactFAQ(newQuestion,
            (_id:number) => {
                let item = { ...newQuestion, id:_id };
                const data = [item, ...questions];
                setQuestions(data);
                setNewQuestion(emptyContactQuestion);

                setVisableCreate(false);
                setLoadingDialog(false);
                dispatch(setSuccessStatus(`On Success: ${_id}`));
            },
            (message:string) => {
                console.log(message);
                
                setVisableCreate(false);
                setLoadingDialog(false);
                dispatch(setErrorStatus(message));
            }
        );
    }

    const onUpdate = () => {
        if(!questions) return;
        setLoadingDialog(true);

        httpPutContactFAQ(currentQuestion,
            (_id:number) => {
                let item = { ...currentQuestion, id:_id };
                const data = questions.map(elem => (elem.id === item.id) ? item : elem);
                
                setQuestions(data);
                setCurrentQuestion(emptyContactQuestion);

                dispatch(setSuccessStatus(`On Success: ${_id}`));
                setVisableUpdate(false);
                setLoadingDialog(false);
            },
            (message:string) => {
                setLoadingDialog(false);
                dispatch(setErrorStatus(message));
            }
        )
    }

    const onDelete = () => {
        if(!questions) return;
        setLoadingDialog(true);

        httpDeleteContactFAQ(currentQuestion.id,
            (_id:number) => {
                const data = questions.filter(elem => (elem.id !== currentQuestion.id));
                
                setQuestions(data);
                setCurrentQuestion(emptyContactQuestion);

                dispatch(setSuccessStatus(`On Success: ${_id}`));
                setVisableDelete(false);
                setLoadingDialog(false);
            },
            (message:string) => {
                dispatch(setErrorStatus(message));
                setLoadingDialog(false);
            }
        );
    }
    
    const visualizeUpdateQuestion = (index:number) => {
        if(!questions) return;
        setCurrentQuestion(questions[index]);
        setVisableUpdate(true);
    }

    const visualizeDeleteQuestion = (index:number) => {
        if(!questions) return;
        setCurrentQuestion(questions[index]);
        setVisableDelete(true);
    }

    const onSendValidation = () => {
        let errors:string[] = [];
        if(contact.email.length === 0)
        {
            errors.push("email");
        }

        if(contact.subject.length === 0)
        {
            errors.push("subject");
        }

        if(contact.content.length === 0)
        {
            errors.push("content");
        }

        if(errors.length === 0)
        {
            onSendEmail();
        }
        else
        {
            setInvalidItems(errors);
        }
    }

    const onSendEmail = () => {
        setLoading(true);
        httpSendContactMail(contact,
            (_) => {
                setLoading(false);
                dispatch(setSuccessStatus(`On Success: Sending message`));
            },
            (message) => {
                setLoading(false);
                dispatch(setErrorStatus(message));
            }
        );
    }

    return <>
        <Typography id="contact" variant="h6" gutterBottom style={{paddingTop:"30px", paddingBottom:"5px", backgroundColor:"#FFFFFF"}}>
            Meest gestelde vragen
        </Typography>
        <ControlButtons 
            onCreate={() => setVisableCreate(true)} 
            isAdmin={isAdmin}
        />
        {loading || !questions 
            ? 
            <ProgressBox/>
            :
            questions.map((elem:IContactQuestion, index:number) => 
                <Accordion key={`Accordion-${index}`}>
                    <AccordionSummary
                        id="panel1a-header"
                        aria-controls="panel1a-content"
                        expandIcon={<ExpandMoreIcon />}
                    >
                        <Typography className={classes.heading}>
                            {elem.question}
                            <ControlButtons 
                                onEdit={() => visualizeUpdateQuestion(index)} 
                                onDelete={() => visualizeDeleteQuestion(index)} 
                                isAdmin={isAdmin}
                            />
                        </Typography>
                    </AccordionSummary>
                    <AccordionDetails>
                        <Typography>
                            {elem.answer}
                        </Typography>
                    </AccordionDetails>
                </Accordion>
            )
        }
        <Typography variant="h6" gutterBottom style={{paddingTop:"30px", backgroundColor:"#FFFFFF"}}>
            Heeft u nog vragen?
        </Typography>
        <Paper style={{textAlign:"left", margin:"20px", paddingTop:"20px", paddingBottom:"20px", paddingLeft:"40px", paddingRight:"40px", width: "max-content"}}>
            <Typography variant="h6">
                Contact
            </Typography>
            <Box>
                {t("autohelper_street")}
            </Box>
            <Box>
                {t("autohelper_city")}, {t("autohelper_postalcode")}
            </Box>
            <Box>
                <a href={`mailto:${t("autohelper_email")}`}>{t("autohelper_email")}</a>
            </Box>
            <Box>
                <a href={`tel:${t("autohelper_phone")}`}>{t("autohelper_phone")}</a>
            </Box>
        </Paper>
        <Box style={{paddingLeft:"20px", paddingRight:"20px", paddingBottom:"20px"}}>
            <TextField
                required
                fullWidth
                style={{marginTop:"5px"}}
                error={invalidItems.includes("email")}
                id="email"
                name="email"
                label={t("email_address")}
                variant="standard"
                autoComplete="home email"
                value={contact.email}
                onChange={(event:any) => setContact({...contact, email:event.target.value})}
            />
            <TextField
                required
                fullWidth
                style={{marginTop:"5px"}}
                error={invalidItems.includes("subject")}
                id="subject"
                name="subject"
                label="Onderwerp"
                variant="standard"
                value={contact.subject}
                onChange={(event:any) => setContact({...contact, subject:event.target.value})}
            />
            <TextField
                required
                style={{marginTop:"5px", width:"100%"}}
                error={invalidItems.includes("content")}
                placeholder={t("message")}
                id="content"
                name="content"
                variant="outlined"
                InputProps={{
                    inputComponent: "textarea",
                    inputProps: {
                      style: {
                        height: "30vh"
                      }
                    }
                }}
                value={contact.content}
                onChange={(event:any) => setContact({...contact, content:event.target.value})}
            />
        </Box>
        <Button
            variant="outlined"
            disabled={loading}
            style={{margin:"20px"}}
            endIcon={<SendIcon/>}
            onClick={() => onSendValidation()}
        >
            {loading ? t("loading") : t("send")}
        </Button>
        <EditKeyValueDialog 
            title={t("create")}
            data={newQuestion}
            setData={(data:any) => setNewQuestion(data)}
            open={visableCreate} 
            setOpen={setVisableCreate} 
            isLoading={loadingDialog}
            onConfirm={onCreate}
            multilines={[
                "answer"
            ]}
            ignoredKeys={[
                "askedAmount"
            ]}
        />
        <EditKeyValueDialog 
            title={t("update")}
            data={currentQuestion}
            setData={setCurrentQuestion}
            open={visableUpdate} 
            setOpen={setVisableUpdate} 
            isLoading={loadingDialog}
            onConfirm={onUpdate}
            multilines={[
                "answer"
            ]}
            ignoredKeys={[
                "askedAmount"
            ]}
        />
        <ConfirmDialog 
            open={visableDelete} 
            setOpen={setVisableDelete} 
            isLoading={loadingDialog}
            onConfirm={onDelete}
        />
    </>;
}
