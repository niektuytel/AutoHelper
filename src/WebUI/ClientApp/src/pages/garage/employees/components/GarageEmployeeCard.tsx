import React, { useState } from "react";
import {
    Box,
    IconButton,
    Typography,
    Card,
    CardHeader,
    CardActions,
    useTheme,
} from "@mui/material";
import { useTranslation } from "react-i18next";
import PersonOffIcon from '@mui/icons-material/PersonOff';
import PersonIcon from '@mui/icons-material/Person';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import { useDispatch } from "react-redux";

// own imports
import { GarageServiceType, GarageEmployeeItemDto } from "../../../../app/web-api-client";
import { COLORS } from "../../../../constants/colors";
import { DAYSINWEEKSHORT } from "../../../../constants/days";
import { showOnError } from "../../../../redux/slices/statusSnackbarSlice";

interface IProps {
    employee: GarageEmployeeItemDto;
    selectedItem: GarageEmployeeItemDto;
    setSelectedItem: (employee: GarageEmployeeItemDto) => void;
    updateEmployee: (employee: any) => void;
}

export default ({ employee, selectedItem, setSelectedItem, updateEmployee }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const dispatch = useDispatch();

    const toggleActiveStatus = () => {
        const updatedEmployee = new GarageEmployeeItemDto({ ...employee, isActive: !employee.isActive });

        // check if the employee has a work schema and work experience, otherwise can not been activated
        if (updatedEmployee.isActive) {
            if (!updatedEmployee.workSchema || updatedEmployee.workSchema.length == 0) {
                dispatch(showOnError(t("Employee need an workschema")));
                return;
            }
            else if (!updatedEmployee.workExperiences || updatedEmployee.workExperiences.length == 0) {
                dispatch(showOnError(t("Employee need a work experience")));
                return;
            }
        }

        updateEmployee({
            id: updatedEmployee.id,
            isActive: updatedEmployee.isActive,
            fullName: updatedEmployee.contact?.fullName,
            email: updatedEmployee.contact?.email,
            phoneNumber: updatedEmployee.contact?.phoneNumber,
            WorkSchema: updatedEmployee.workSchema,
            WorkExperiences: updatedEmployee.workExperiences,
        });
    };


    return (
        <Card
            style={{
                marginBottom: "10px",
                padding: "8px",
                cursor: "pointer",
                border: selectedItem === employee ? `1px solid black` : `1px solid ${COLORS.BORDER_GRAY}`
            }}
            onClick={() => setSelectedItem(employee)}
        >
            <CardHeader
                title={employee.contact?.fullName}
                titleTypographyProps={{ variant: "body1" }}
                style={{ paddingBottom: "4px", paddingTop: "4px", paddingLeft: "4px" }}
            />
            <CardActions style={{ padding: "0", justifyContent: "space-between" }}>
                <Box display="flex" sx={{ "align-self": "self-end"}}>
                    <AccessTimeIcon color="action" fontSize="small" />
                    <Typography variant="caption" color="textSecondary" style={{ marginLeft: "8px" }}>
                        {(employee.workSchema && employee.workSchema.length != 0) ?
                            employee.workSchema
                                .map(item => t(DAYSINWEEKSHORT[item.dayOfWeek]))
                                .filter((value, index, self) => self.indexOf(value) === index)
                                .join(', ')
                            :
                            `${t('has no work schema')}`
                        }
                    </Typography>
                </Box>
                <Box display="flex">
                    <IconButton onClick={toggleActiveStatus}>
                        {employee.isActive
                            ? <PersonIcon style={{ color: 'green' }} fontSize="small" />
                            : <PersonOffIcon style={{ color: 'red' }} fontSize="small" />
                        }
                    </IconButton>
                </Box>
            </CardActions>
        </Card>
    );
}
