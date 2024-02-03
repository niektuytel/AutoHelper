import { Typography, Box, Skeleton, Divider, Chip } from '@mui/material';

interface IProps {
    keyIndex: number;
}

export default ({ keyIndex }: IProps) => {

    return <>
        <Box key={`serviceLog-${keyIndex}`}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', m: 1 }}>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <Typography variant="subtitle1" color="text.secondary">
                        <Skeleton sx={{ width: "100px" }} />
                    </Typography>
                </Box>
            </Box>
            <Typography variant="body2" sx={{ mx: 1 }}>
                <Skeleton sx={{ width: "100%" }} />
            </Typography>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center', m: 1 }}>
                <Skeleton sx={{ width: "100%" }} />
            </Box>
            <Divider sx={{ mt: 1 }} />
        </Box>
    </>
}