﻿import React from "react";
import { Container, useTheme, useMediaQuery, Fab, Typography, Box, Button, CardContent, Card, CardActions, Paper, Grid} from "@mui/material";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";

// own imports
import { ROUTES } from "../../constants/routes";

interface IProps {}

export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const { from } = useParams();
    const { t } = useTranslation();
    const navigate = useNavigate();

    const getMessage = () => {
        switch (from) {
            case 'ServiceLogReview':
                return t('ThankYouPage.ServiceLogReview.Description');
            case 'UnsubscribeNotification':
                return t('ThankYouPage.UnsubscribeNotification.Description');
            case 'Payment':
                return t('ThankYouPage.Payment');
            default:
                return t('ThankYouPage.Default');
        }
    };

    const handleServiceLogsClick = () => {
        navigate(ROUTES.GARAGE_ACCOUNT.SERVICELOGS);
    };

    if (from == "UnsubscribeNotification")
    {
        return <Container maxWidth="md">
            <Box sx={{ marginTop: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                <Box sx={{ p: 2 }}>
                    <svg data-name="Layer 1" height="150px" viewBox="0 0 613.35286 700.56123">
                        <path d="M587.667,756.93133l-.78168-17.57606q-.12187-.06468-.24405-.12879c-8.37606-4.39513-17.97576,3.561-15.3004,12.63392,2.51722,8.5366,4.20846,17.73958,8.81613,25.21162a34.14441,34.14441,0,0,0,25.03847,15.77861l10.64166,6.51555a57.21525,57.21525,0,0,0-12.05953-46.36368,55.2668,55.2668,0,0,0-10.157-9.4375C591.07189,750.28541,587.667,756.93133,587.667,756.93133Z" transform="translate(-293.32357 -99.71938)" fill="#f2f2f2" /><path d="M492.3909,578.56387c-20.48783-21.12931-50.17767-33.24712-81.45666-33.24712l-1.20652.00324a116.3146,116.3146,0,0,1-58.63546-15.83485l-1.85205-1.07912,1.49167-1.537a113.37272,113.37272,0,0,0,19.99317-28.30616,34.28239,34.28239,0,0,1-27.66751,8.93789,35.10416,35.10416,0,0,1-25.77016-16.50626c-10.93231-17.73787-23.96381-42.90658-23.96381-62.0803a116.66888,116.66888,0,0,1,38.0631-86.091,72.65143,72.65143,0,0,0,23.99981-54.17137l-.00607-1.45284a164.77416,164.77416,0,0,1,4.8908-39.87268l.33005-1.31694,1.355.04732a111.39121,111.39121,0,0,0,26.04681-2.1821,87.918,87.918,0,0,1-23.35954-6.213l-1.45769-.58971.4947-1.4937C397.78172,214.464,474.7579,225.398,519.43718,123.17462,555.666,40.28535,683.49348,196.75549,683.49348,287.199c0,9.2287-9.13324,21.42423-2.29615,27.52391,73.90979,65.93827,22.91308,108.27972,6.83466,150.88534-3.81479,10.10871,4.45682,21.20613,4.45682,32.14249,0,1.85569-.04732,3.77124-.14076,5.69327l-.14237,2.955-2.46441-1.63971a81.62563,81.62563,0,0,1-13.47964-11.05649,71.13531,71.13531,0,0,1-2.25612,61.41172c-7.74552,14.619-15.23907,25.085-22.27193,31.10507a116.2904,116.2904,0,0,1-159.34268-7.65574Z" transform="translate(-293.32357 -99.71938)" fill="#e6e6e6" /><path d="M616.13152,798.499a2.09333,2.09333,0,0,0,2.06993-1.79837c.08893-.62489,8.80049-63.45379.97652-146.956-7.22548-77.11632-30.4294-188.191-99.9393-279.41478a2.0935,2.0935,0,1,0-3.33028,2.53775c68.90954,90.436,91.9264,200.69567,99.10094,277.2675,7.77849,83.01636-.86476,145.3553-.953,145.97542a2.09519,2.09519,0,0,0,2.0752,2.38851Z" transform="translate(-293.32357 -99.71938)" fill="#3f3d56" /><path d="M559.40732,499.0052a2.09343,2.09343,0,0,0,1.23242-3.78686c-.25231-.18331-25.681-18.50562-63.69657-32.425-35.14253-12.86589-88.582-24.5297-143.597-7.02036a2.0932,2.0932,0,1,0,1.26956,3.98925c53.88785-17.14953,106.35862-5.67858,140.88784,6.96312,37.47293,13.72044,62.427,31.7,62.67472,31.87988A2.08515,2.08515,0,0,0,559.40732,499.0052Z" transform="translate(-293.32357 -99.71938)" fill="#3f3d56" /><path d="M655.2196,260.37275a2.09343,2.09343,0,0,0-3.68827-1.50192c-.201.23845-20.307,24.28166-36.928,61.19609-15.36327,34.12475-30.84523,86.58548-17.34326,142.71855a2.09319,2.09319,0,1,0,4.07031-.979c-13.22427-54.983,1.99558-106.49141,17.09108-140.02058,16.38342-36.38756,36.11335-59.982,36.31062-60.21616A2.0852,2.0852,0,0,0,655.2196,260.37275Z" transform="translate(-293.32357 -99.71938)" fill="#3f3d56" /><path d="M652.38648,368.31675c1.76623,14.25037,11.64059,26,26,26a26,26,0,0,0,26-26c0-14.3594-11.70215-24.67241-26-26C663.21558,340.90809,650.11884,350.02107,652.38648,368.31675Z" transform="translate(-293.32357 -99.71938)" fill="#2f2e41" /><polygon points="397.105 687.328 407.006 687.327 411.716 649.137 397.103 649.138 397.105 687.328" fill="#ffb6b6" /><path d="M690.08055,797.37579l30.447-.00116v-.385A11.85149,11.85149,0,0,0,708.67674,785.139H708.676l-5.56152-4.21923-10.37659,4.21991-2.65783.00006Z" transform="translate(-293.32357 -99.71938)" fill="#2f2e41" /><polygon points="358.105 687.328 368.006 687.327 372.716 649.137 358.103 649.138 358.105 687.328" fill="#ffb6b6" /><path d="M651.08055,797.37579l30.447-.00116v-.385A11.85149,11.85149,0,0,0,669.67674,785.139H669.676l-5.56152-4.21923-10.37659,4.21991-2.65783.00006Z" transform="translate(-293.32357 -99.71938)" fill="#2f2e41" /><path d="M724.50065,547.85614l7,24L723.25419,673.501s4.24646,6.35511,1.24646,6.35511-6.84686,1.40942-4.4234,5.20471-5.86,17.4209-5.86,17.4209l-11.71661,72.37439-14.92914-.22046s-7.59009-4.22833-3.3305-8.504,7.25958-4.27557,3.25958-8.27557-4-2.79529-4-5.89765-4.00751-41.01574,5.49628-55.559l-2.86365-59.29529-6.74457,58.16144-13.888,77.59052-14.38171-.11847s-6.64276-6.40271-6.6305-11.14209,4.10419-5.13312,1.05823-7.93628-3.046-45.86615,5.954-69.80316c0,0,5.09108-13.37393-.01155-17.95666s-.00631-9.82652-.00631-9.82652,8.08172-2.92145-.0073-11.33883-.04369-67.94913-.04369-67.94913-8.84271-24.03067.1131-33.47977S724.50065,547.85614,724.50065,547.85614Z" transform="translate(-293.32357 -99.71938)" fill="#2f2e41" /><path d="M599.82611,589.20574l12.48925-30.1823,9.84,5.46667-5.94253,30.70307a10.93464,10.93464,0,1,1-16.38671-5.98744Z" transform="translate(-293.32357 -99.71938)" fill="#ffb6b6" /><path d="M809.4002,350.535l-19.00315,26.56751-8.34544-7.554,12.7447-28.5581A10.93464,10.93464,0,1,1,809.4002,350.535Z" transform="translate(-293.32357 -99.71938)" fill="#ffb6b6" /><path d="M726.38648,550.31675c12.32568,19.11831-49.63137,43.38055-73.40448,33.64-4.1628-1.7056-18.48212-7.33918-8.69439-15.14958s-1.1348-8.90952-1.393-13.99392c-.1726-3.39918,2.45917-10.85592-.28674-10.5362s-5.95975-4.3811.0205-6.404.06411-3.86767.11651-5.78787c.57186-20.95538,16.64979-54.2869,16.64979-54.2869l-28.971,74.05682s.214,12.81223-3.48049,8.897-3.42565,8.75675-3.42565,8.75675-5.49354,3.83389-2.33452,5.9676-3.401,8.6937-3.401,8.6937l-15.30666-1.09334s.23964-11.37068,2.85315-13.88533,1.32975-6.47144,1.32975-6.47144,6.80457-6.02348,2.24268-10.91429c-5.86712-6.29013,15.52417-112.52993,22.90637-111.47784a28.12048,28.12048,0,0,1,20.08017-21.45161l16.53221-4.55191,2.93564-11.64749h23.27852l4.0548,7.65333,17.69783,7.98657c3.33362-4.70352,5.82477-9.65991,19.47546-11.26657,0,0,36.78255-33.46683,34.98668-36.08s3.906-8.73109,3.906-8.73109,4.86956-1.59377,2.12179-2.44467,2.446-2.81826,2.446-2.81826,4.81028-1.264,2.54157-2.92832,7.57128-2.75765,7.57128-2.75765l12.767,10.4168-2.58037,4.37541s1.5733,5.958-.48,3.78289-2.43983,4.1371-2.43983,4.1371-1.0486,6.912-7.26682,8.99446S778.502,404.8635,778.502,404.8635c-14.6648,19.0673-31.97132,33.9415-52.48,43.73331C734.01483,457.77113,726.38648,550.31675,726.38648,550.31675Z" transform="translate(-293.32357 -99.71938)" fill="#1c94f3" /><circle cx="387.93692" cy="273.08556" r="23.0557" fill="#ffb6b6" /><path d="M657.38648,375.31675c.73,5.01,5,12,4,12s-8.11-20.65332-1-22c5.15722-.97681,6.31-.12,11.13-2.21l-.68.64c3.19,2.32,7.63.89,11.27-.62,3.65-1.5,8.08-2.93,11.28-.62,2.01,1.45,2.86,3.97,4.11,6.11,1.25,2.15,3.56,4.18,5.94,3.52,1.91-.53,3.01-2.67005,3-4.65s-2.58825-3.86355-1.72-5.64c2.57319-5.26444.25916-8.85275-5.35-11.33q-3.06-.465-6.12-.95a17.35842,17.35842,0,0,1,3.84-3.86,8.77436,8.77436,0,0,0-3.1-3.97c-1.63-.97-3.65-.9-5.54-.8q-6.33.33-12.65.66c-3.12.16-6.35.35-9.13995,1.78-3.48,1.79-5.76,5.28-7.42005,8.83C655.51648,360.18675,656.10645,366.60679,657.38648,375.31675Z" transform="translate(-293.32357 -99.71938)" fill="#2f2e41" /><path d="M561.476,799.97328l226.75.30734a1.19068,1.19068,0,1,0,0-2.38137l-226.75-.30733a1.19068,1.19068,0,0,0,0,2.38136Z" transform="translate(-293.32357 -99.71938)" fill="#cacaca" /><ellipse cx="471.98821" cy="695.09416" rx="5.25681" ry="7.55666" transform="translate(-577.14417 962.36498) rotate(-81.72174)" fill="#e6e6e6" /><ellipse cx="521.15973" cy="690.29611" rx="5.2568" ry="7.55666" transform="translate(-530.30434 1006.91693) rotate(-81.72174)" fill="#3f3d56" /><ellipse cx="458.5236" cy="655.286" rx="5.25681" ry="7.55666" transform="translate(-549.27676 914.96411) rotate(-81.72174)" fill="#ff6584" /><ellipse cx="441.31304" cy="640.04291" rx="4.77581" ry="3.3223" transform="translate(-495.37215 976.78425) rotate(-89.56675)" fill="#e6e6e6" /><ellipse cx="434.06731" cy="609.6713" rx="4.77581" ry="3.3223" transform="translate(-472.19234 939.39678) rotate(-89.56675)" fill="#3f3d56" /><ellipse cx="490.41248" cy="663.12322" rx="4.77581" ry="3.3223" transform="translate(-469.72363 1048.78806) rotate(-89.56675)" fill="#e6e6e6" /><ellipse cx="466.65067" cy="674.98721" rx="4.77581" ry="3.3223" transform="translate(-505.16941 1036.80122) rotate(-89.56675)" fill="#ff6584" /><ellipse cx="499.72794" cy="696.91495" rx="4.77581" ry="3.3223" transform="translate(-494.26937 1091.63947) rotate(-89.56675)" fill="#3f3d56" /><ellipse cx="849.98821" cy="541.09416" rx="5.25681" ry="7.55666" transform="translate(-101.17347 1204.59943) rotate(-81.72174)" fill="#e6e6e6" /><ellipse cx="899.15973" cy="536.29611" rx="5.2568" ry="7.55666" transform="translate(-54.33364 1249.15138) rotate(-81.72174)" fill="#3f3d56" /><ellipse cx="836.5236" cy="501.286" rx="5.25681" ry="7.55666" transform="matrix(0.14398, -0.98958, 0.98958, 0.14398, -73.30606, 1157.19856)" fill="#ff6584" /><ellipse cx="819.31304" cy="486.04292" rx="4.77581" ry="3.3223" transform="translate(33.76519 1201.93792) rotate(-89.56675)" fill="#e6e6e6" /><ellipse cx="812.06731" cy="455.6713" rx="4.77581" ry="3.3223" transform="translate(56.94499 1164.55045) rotate(-89.56675)" fill="#3f3d56" /><ellipse cx="868.41248" cy="509.12322" rx="4.77581" ry="3.3223" transform="translate(59.41371 1273.94173) rotate(-89.56675)" fill="#e6e6e6" /><ellipse cx="844.65067" cy="520.98721" rx="4.77581" ry="3.3223" transform="translate(23.96793 1261.95489) rotate(-89.56675)" fill="#ff6584" /><ellipse cx="877.72794" cy="542.91495" rx="4.77581" ry="3.3223" transform="translate(34.86796 1316.79314) rotate(-89.56675)" fill="#3f3d56" /><ellipse cx="666.98821" cy="256.09416" rx="5.25681" ry="7.55666" transform="translate(24.20544 779.54069) rotate(-81.72174)" fill="#fff" /><ellipse cx="716.15973" cy="251.29611" rx="5.2568" ry="7.55666" transform="translate(71.04527 824.09264) rotate(-81.72174)" fill="#3f3d56" /><ellipse cx="653.5236" cy="216.286" rx="5.25681" ry="7.55666" transform="translate(52.07285 732.13983) rotate(-81.72174)" fill="#ff6584" /><ellipse cx="636.31304" cy="201.04292" rx="4.77581" ry="3.3223" transform="translate(137.1408 736.09819) rotate(-89.56675)" fill="#e6e6e6" /><ellipse cx="629.06731" cy="170.6713" rx="4.77581" ry="3.3223" transform="translate(160.32061 698.71071) rotate(-89.56675)" fill="#3f3d56" /><ellipse cx="685.41248" cy="224.12322" rx="4.77581" ry="3.3223" transform="translate(162.78933 808.102) rotate(-89.56675)" fill="#e6e6e6" /><ellipse cx="661.65067" cy="235.98721" rx="4.77581" ry="3.3223" transform="translate(127.34354 796.11516) rotate(-89.56675)" fill="#ff6584" /><ellipse cx="694.72794" cy="257.91495" rx="4.77581" ry="3.3223" transform="translate(138.24358 850.95341) rotate(-89.56675)" fill="#3f3d56" />
                    </svg>
                </Box>
                <Typography variant="h4">
                    {t('ThankYouPage.UnsubscribeNotification.Title')}
                </Typography>
                <Typography variant="body1" sx={{ marginTop: 2 }}>
                    {getMessage()}
                </Typography>
            </Box>
        </Container>
    }

    return (
        <Container maxWidth="md">
            <Box sx={{ marginTop: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                <Box sx={{ p: 2 }}>
                    <svg data-name="Layer 1" height="150px" viewBox="0 0 811.75984 620">
                        <path d="M269.03284,759.032l-.03789.24566c-.04493-.23913-.08817-.4842-.12926-.72552-.0751-.39-.14026-.7804-.20168-1.17321a95.55112,95.55112,0,0,1,3.9079-45.76336q1.4428-4.30709,3.28655-8.45606a93.74713,93.74713,0,0,1,19.64776-28.70874,40.444,40.444,0,0,1,9.11275-6.99233c.49254-.26095.98777-.50827,1.494-.73628a16.67145,16.67145,0,0,1,9.998-1.44878,13.63552,13.63552,0,0,1,3.55957,1.195c.39479.192.77486.40275,1.14774.6276a19.75625,19.75625,0,0,1,8.64631,13.22952,30.86278,30.86278,0,0,1-.43488,12.03106q-.12025.56508-.25842,1.12548a62.70539,62.70539,0,0,1-2.48907,7.73053C316.31349,726.86214,294.89039,749.619,269.03284,759.032Z" transform="translate(-194.12008 -140)" fill="#f2f2f2" /><path d="M319.60412,666.55063q-11.66777,16.76675-22.04379,34.3838-10.36341,17.60241-19.37144,35.9573-5.03919,10.26928-9.63774,20.74671c-.33941.773.80562,1.44544,1.14941.66249q8.21254-18.70334,17.82456-36.7486,9.6061-18.02687,20.56148-35.2903,6.12971-9.65806,12.66693-19.04891c.48734-.7003-.66386-1.36023-1.14941-.66249Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M327.92735,692.42374a36.42583,36.42583,0,0,1-23.9464,13.87842,34.62039,34.62039,0,0,1-7.99383.15838c-.8516-.08051-.85755,1.24678-.01326,1.32661a37.74921,37.74921,0,0,0,27.36864-8.64847,35.94791,35.94791,0,0,0,5.73426-6.05245c.51143-.68053-.64145-1.33842-1.14941-.66249Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M278.00347,733.63185a30.495,30.495,0,0,1-3.36463-6.22033,32.17041,32.17041,0,0,1-2.06693-15.79595q1.4428-4.30709,3.28655-8.45606a.75827.75827,0,0,1,.06128.74918,29.11531,29.11531,0,0,0-1.85428,6.46168,30.74813,30.74813,0,0,0,5.08981,22.60357.53935.53935,0,0,1,.08813.51977A.72267.72267,0,0,1,278.00347,733.63185Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M304.81723,666.61825a30.873,30.873,0,0,0,1.36,19.17961.66706.66706,0,0,0,.90485.24695.67955.67955,0,0,0,.24695-.90486,29.38117,29.38117,0,0,1-1.23217-18.17246.66361.66361,0,0,0-1.27965-.34924Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M268.86976,726.99737l.08056.23515c-.15087-.19089-.303-.38783-.45149-.58245-.24765-.31046-.48672-.62594-.72358-.94526a95.55132,95.55132,0,0,1-17.79739-42.34159q-.723-4.4844-1.01758-9.015a93.74705,93.74705,0,0,1,4.06347-34.55018,40.4442,40.4442,0,0,1,4.82181-10.42521c.315-.45989.63863-.909.981-1.346a16.67146,16.67146,0,0,1,8.18086-5.92728,13.63548,13.63548,0,0,1,3.70734-.59523c.43877-.01341.87327-.00329,1.30792.02263a19.75622,19.75622,0,0,1,13.80225,7.69916,30.8628,30.8628,0,0,1,5.20357,10.85625q.156.55628.294,1.11673a62.70413,62.70413,0,0,1,1.38678,8.00208C295.79609,676.54608,287.39569,706.65021,268.86976,726.99737Z" transform="translate(-194.12008 -140)" fill="#f2f2f2" /><path d="M270.69414,621.608q-2.544,20.26793-3.54911,40.6888-1.00074,20.40206-.45166,40.84086.3078,11.43489,1.10248,22.84944c.0585.84217,1.38487.90579,1.32562.05274q-1.4154-20.37786-1.28578-40.823.13295-20.42616,1.81535-40.803.94185-11.4002,2.36872-22.75305c.10626-.84654-1.21975-.89619-1.32562-.05275Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M290.08349,640.65387a36.42578,36.42578,0,0,1-14.75918,23.41381,34.6199,34.6199,0,0,1-7.00546,3.85357c-.79154.32429-.18026,1.50245.6045,1.181a37.74923,37.74923,0,0,0,20.21922-20.37208,35.948,35.948,0,0,0,2.26654-8.02351c.13678-.84022-1.18977-.88729-1.32562-.05274Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M265.01488,700.33692a30.49551,30.49551,0,0,1-5.86906-3.94554,32.17063,32.17063,0,0,1-9.168-13.02816q-.723-4.4844-1.01758-9.015a.75824.75824,0,0,1,.40228.635,29.11512,29.11512,0,0,0,1.35951,6.58357,30.7482,30.7482,0,0,0,15.00719,17.65254.5393.5393,0,0,1,.31949.41935A.72267.72267,0,0,1,265.01488,700.33692Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M257.63085,628.53667a30.873,30.873,0,0,0,10.11372,16.353.66707.66707,0,0,0,.916-.20163.67954.67954,0,0,0-.20164-.916,29.38112,29.38112,0,0,1-9.53266-15.52047.66361.66361,0,0,0-1.29544.28516Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M269.44389,759.19523l.19452.15475c-.22993-.07959-.46417-.1636-.6941-.24761-.37587-.12821-.74721-.26528-1.11876-.40677a95.55124,95.55124,0,0,1-37.79944-26.09137q-3.02434-3.38909-5.71256-7.0479a93.7471,93.7471,0,0,1-15.18363-31.29991,40.44371,40.44371,0,0,1-1.55185-11.381c.0177-.55711.04857-1.1098.10168-1.66249a16.67157,16.67157,0,0,1,3.70086-9.40015,13.636,13.636,0,0,1,2.80318-2.49817c.36248-.2476.734-.47307,1.11423-.68533a19.75621,19.75621,0,0,1,15.776-.94618A30.86284,30.86284,0,0,1,241.3053,674.028q.431.38468.84911.78261a62.70373,62.70373,0,0,1,5.47811,5.99558C264.96041,702.1842,274.09533,732.07364,269.44389,759.19523Z" transform="translate(-194.12008 -140)" fill="#f2f2f2" /><path d="M214.22223,669.41313q8.772,18.44757,18.92306,36.1952,10.14459,17.72943,21.61482,34.65515,6.41775,9.4691,13.2348,18.65884c.50285.6781,1.6547.01736,1.14536-.66949Q256.9729,741.845,246.0711,724.54841q-10.88877-17.28237-20.44541-35.35767-5.34613-10.11288-10.25811-20.44709c-.36638-.77051-1.5104-.09821-1.14535.66948Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M240.81684,675.01854a36.4258,36.4258,0,0,1,.17395,27.67689,34.61982,34.61982,0,0,1-3.82731,7.01984c-.49229.69954.65729,1.363,1.14536.66949a37.74921,37.74921,0,0,0,6.06476-28.05454,35.948,35.948,0,0,0-2.41141-7.98116c-.33727-.78162-1.48034-.10685-1.14535.66948Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M251.83749,738.80763a30.495,30.495,0,0,1-7.07011-.1636,32.17048,32.17048,0,0,1-14.74127-6.0398q-3.02434-3.38909-5.71256-7.0479a.75827.75827,0,0,1,.68093.31836,29.11553,29.11553,0,0,0,4.69117,4.815,30.74817,30.74817,0,0,0,22.15184,6.79144.53938.53938,0,0,1,.495.18127A.72268.72268,0,0,1,251.83749,738.80763Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M206.94684,682.28657a30.873,30.873,0,0,0,17.32878,8.33189c.3581.04766.65985-.339.66323-.66323a.67954.67954,0,0,0-.66323-.66323,29.38112,29.38112,0,0,1-16.39083-7.94337c-.61481-.59262-1.554.34414-.938.93794Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M359.98141,584.24891a10.74268,10.74268,0,0,0,1.58187-16.3965l4.16719-93.01794-21.21552,2.3813,1.23255,90.98468a10.80091,10.80091,0,0,0,14.23391,16.04846Z" transform="translate(-194.12008 -140)" fill="#ffb8b8" /><polygon points="204.564 601.708 216.639 603.83 228.573 558.267 212.752 555.134 204.564 601.708" fill="#ffb8b8" /><path d="M395.12335,741.026h38.53073a0,0,0,0,1,0,0v14.88687a0,0,0,0,1,0,0H410.0102a14.88686,14.88686,0,0,1-14.88686-14.88686v0A0,0,0,0,1,395.12335,741.026Z" transform="translate(498.80399 1417.38394) rotate(-170.02922)" fill="#2f2e41" /><polygon points="171.422 607.752 183.682 607.751 189.514 560.463 171.42 560.464 171.422 607.752" fill="#ffb8b8" /><path d="M362.91494,744.24846h38.53073a0,0,0,0,1,0,0v14.88687a0,0,0,0,1,0,0H377.80179a14.88686,14.88686,0,0,1-14.88686-14.88686v0A0,0,0,0,1,362.91494,744.24846Z" transform="translate(570.27483 1363.36634) rotate(179.99738)" fill="#2f2e41" /><path d="M417.69451,734.8025a4.75008,4.75008,0,0,1-.57252-.03389l-14.43-1.18741a4.88077,4.88077,0,0,1-4.24251-5.65951l13.32468-74.681-9.00352-47.474a1.62706,1.62706,0,0,0-3.219.15995L388.29589,732.55435a4.92369,4.92369,0,0,1-5.2096,4.43715l-13.59479-.50632a4.88783,4.88783,0,0,1-4.53645-4.631l-.91385-151.76761,70.48116-8.80968,4.9236,76.04057-.01959.0805-16.99071,83.675A4.88582,4.88582,0,0,1,417.69451,734.8025Z" transform="translate(-194.12008 -140)" fill="#2f2e41" /><circle cx="193.15865" cy="249.99669" r="24.56103" fill="#ffb8b8" /><path d="M410.42125,591.17892a20.11,20.11,0,0,1-10.85692-3.10569c-11.89736-7.43585-25.41059-4.48057-32.40686-2.057a4.88009,4.88009,0,0,1-4.22053-.48089,4.81083,4.81083,0,0,1-2.2244-3.55163L347.99,468.35816c-2.132-19.03768,9.33586-36.93668,27.2677-42.55965h0q1.01052-.317,2.05519-.60112a39.56866,39.56866,0,0,1,32.972,5.72254,40.20349,40.20349,0,0,1,17.1668,29.35307l10.71048,114.3871a4.80738,4.80738,0,0,1-1.52715,4.0071C432.88054,582.14044,421.98336,591.17786,410.42125,591.17892Z" transform="translate(-194.12008 -140)" fill="#1c94f3" /><path d="M371.68013,492.72437l-28.70337-3.156a5.71747,5.71747,0,0,1-4.90543-7.13382l7.30606-27.84637a15.87852,15.87852,0,0,1,31.55638,3.56326l1.08461,28.67531a5.71749,5.71749,0,0,1-6.33825,5.89758Z" transform="translate(-194.12008 -140)" fill="#1c94f3" /><path d="M438.04732,580.26582a10.74264,10.74264,0,0,0-.40564-16.46763L430.56875,470.956l-20.78851,4.67965,12.20288,90.41406a10.80091,10.80091,0,0,0,16.0642,14.21613Z" transform="translate(-194.12008 -140)" fill="#ffb8b8" /><path d="M401.81017,486.888a5.7113,5.7113,0,0,1-1.81845-4.39983l1.08461-28.67532a15.87852,15.87852,0,0,1,31.55638-3.56326l7.30606,27.84637a5.71749,5.71749,0,0,1-4.90543,7.13383l-28.70337,3.156A5.711,5.711,0,0,1,401.81017,486.888Z" transform="translate(-194.12008 -140)" fill="#1c94f3" /><path d="M385.81792,415.48307a5.683,5.683,0,0,1-1.29663-.15137l-.12475-.03027c-21.59449-3.30371-26.3667-15.81153-27.41431-21.03516-1.08423-5.4082.15039-10.62842,2.94019-12.65576-1.521-4.80273-1.27686-9.061.72729-12.66211,3.49536-6.28027,11.08106-8.40381,12.09839-8.66357,6.05811-4.46924,13.3064-1.48584,14.62524-.88086,11.71851-4.33545,16.19751-.72657,17.00757.0791,5.23828.94092,8.43115,2.96435,9.49121,6.01562,1.991,5.731-4.30542,12.85987-4.57446,13.16065l-.13965.15576-9.38013.44678a6.358,6.358,0,0,0-5.9812,7.31689h0a29.60406,29.60406,0,0,0,.96045,3.35547c1.602,5.00635,2.80225,9.2832,1.25415,10.90918a2.50968,2.50968,0,0,1-2.62524.45508c-1.46655-.3916-2.4624-.30957-2.9585.24463-.77026.85937-.53515,3.03466.66211,6.12549a5.73887,5.73887,0,0,1-1.0459,5.84716A5.56805,5.56805,0,0,1,385.81792,415.48307Z" transform="translate(-194.12008 -140)" fill="#2f2e41" /><path d="M675.87992,494h-121a17.01917,17.01917,0,0,1-17-17V295.16846a17.01916,17.01916,0,0,1,17-17h121a17.01916,17.01916,0,0,1,17,17V477A17.01917,17.01917,0,0,1,675.87992,494Zm-121-213.83154a15.017,15.017,0,0,0-15,15V477a15.017,15.017,0,0,0,15,15h121a15.017,15.017,0,0,0,15-15V295.16846a15.017,15.017,0,0,0-15-15Z" transform="translate(-194.12008 -140)" fill="#3f3d56" /><path d="M657.37988,365.58447h-84a8.50951,8.50951,0,0,1-8.5-8.5v-40a8.50951,8.50951,0,0,1,8.5-8.5h84a8.50981,8.50981,0,0,1,8.5,8.5v40A8.50981,8.50981,0,0,1,657.37988,365.58447Z" transform="translate(-194.12008 -140)" fill="#1c94f3" /><path d="M651.38,403.08434h-72a8,8,0,1,1,0-16h72a8,8,0,0,1,0,16Z" transform="translate(-194.12008 -140)" fill="#ccc" /><path d="M651.38,433.08434h-72a8,8,0,1,1,0-16h72a8,8,0,0,1,0,16Z" transform="translate(-194.12008 -140)" fill="#ccc" /><path d="M651.38,463.08434h-72a8,8,0,1,1,0-16h72a8,8,0,0,1,0,16Z" transform="translate(-194.12008 -140)" fill="#ccc" /><path d="M970.87992,596h-121a17.01917,17.01917,0,0,1-17-17V397.16846a17.01916,17.01916,0,0,1,17-17h121a17.01916,17.01916,0,0,1,17,17V579A17.01917,17.01917,0,0,1,970.87992,596Zm-121-213.83154a15.017,15.017,0,0,0-15,15V579a15.017,15.017,0,0,0,15,15h121a15.017,15.017,0,0,0,15-15V397.16846a15.017,15.017,0,0,0-15-15Z" transform="translate(-194.12008 -140)" fill="#3f3d56" /><path d="M946.38,425.08434h-72a8,8,0,1,1,0-16h72a8,8,0,0,1,0,16Z" transform="translate(-194.12008 -140)" fill="#ccc" /><path d="M946.38,455.08434h-72a8,8,0,1,1,0-16h72a8,8,0,0,1,0,16Z" transform="translate(-194.12008 -140)" fill="#ccc" /><path d="M952.37988,531.584h-84a8.51013,8.51013,0,0,1-8.5-8.5v-40a8.51014,8.51014,0,0,1,8.5-8.5h84a8.51013,8.51013,0,0,1,8.5,8.5v40A8.51012,8.51012,0,0,1,952.37988,531.584Z" transform="translate(-194.12008 -140)" fill="#e6e6e6" /><path d="M946.38,567.08434h-72a8,8,0,1,1,0-16h72a8,8,0,0,1,0,16Z" transform="translate(-194.12008 -140)" fill="#ccc" /><path d="M896.87992,307h-121a17.01917,17.01917,0,0,1-17-17V177.16846a17.01916,17.01916,0,0,1,17-17h121a17.01916,17.01916,0,0,1,17,17V290A17.01917,17.01917,0,0,1,896.87992,307Zm-121-144.83154a15.017,15.017,0,0,0-15,15V290a15.017,15.017,0,0,0,15,15h121a15.017,15.017,0,0,0,15-15V177.16846a15.017,15.017,0,0,0-15-15Z" transform="translate(-194.12008 -140)" fill="#3f3d56" /><path d="M872.38,271.58434h-72a8,8,0,1,1,0-16h72a8,8,0,0,1,0,16Z" transform="translate(-194.12008 -140)" fill="#ccc" /><path d="M872.38,241.58434h-72a8,8,0,1,1,0-16h72a8,8,0,0,1,0,16Z" transform="translate(-194.12008 -140)" fill="#ccc" /><path d="M872.38,211.58434h-72a8,8,0,1,1,0-16h72a8,8,0,0,1,0,16Z" transform="translate(-194.12008 -140)" fill="#ccc" /><circle cx="488.75984" cy="143" r="23" fill="#1c94f3" /><path d="M680.60009,291.774a2.38531,2.38531,0,0,1-1.43527-.47675l-.02567-.01928-5.40515-4.13837a2.40139,2.40139,0,1,1,2.921-3.81237l3.50105,2.68456,8.27354-10.7899a2.40128,2.40128,0,0,1,3.36684-.44444l-.05144.06984.05278-.06883a2.4041,2.4041,0,0,1,.44444,3.36687L682.5106,290.8368A2.40237,2.40237,0,0,1,680.60009,291.774Z" transform="translate(-194.12008 -140)" fill="#fff" /><circle cx="788.75984" cy="246" r="23" fill="#3f3d56" /><path d="M980.60009,394.774a2.38531,2.38531,0,0,1-1.43527-.47675l-.02567-.01928-5.40515-4.13837a2.40139,2.40139,0,1,1,2.921-3.81237l3.50105,2.68456,8.27354-10.7899a2.40128,2.40128,0,0,1,3.36684-.44444l-.05144.06984.05278-.06883a2.4041,2.4041,0,0,1,.44444,3.36687L982.5106,393.8368A2.40237,2.40237,0,0,1,980.60009,394.774Z" transform="translate(-194.12008 -140)" fill="#fff" /><circle cx="715.75984" cy="23" r="23" fill="#3f3d56" /><path d="M907.60009,171.774a2.38531,2.38531,0,0,1-1.43527-.47675l-.02567-.01928-5.40515-4.13837a2.40139,2.40139,0,1,1,2.921-3.81237l3.50105,2.68456,8.27354-10.7899a2.40128,2.40128,0,0,1,3.36684-.44444l-.05144.06984.05278-.06883a2.4041,2.4041,0,0,1,.44444,3.36687L909.5106,170.8368A2.40237,2.40237,0,0,1,907.60009,171.774Z" transform="translate(-194.12008 -140)" fill="#fff" /><path d="M576.12008,760h-381a1,1,0,0,1,0-2h381a1,1,0,1,1,0,2Z" transform="translate(-194.12008 -140)" fill="#3f3d56" />
                    </svg>
                </Box>
                <Typography variant="h4">
                    {t('ThankYouPage.ServiceLogReview.Title')}
                </Typography>
                <Typography variant="body1" sx={{ marginTop: 2 }}>
                    {getMessage()}
                </Typography>
                <Box sx={{ display: 'flex', flexDirection: isMobile ? 'column' : 'row', justifyContent: 'space-around', width: '100%', marginTop: 4, gap: 2 }}>
                    <Paper sx={{ padding: 2, maxWidth: isMobile ? '100%' : '100%', width: '100%' }}>
                        <Grid container direction="column" spacing={1}>
                            <Grid item>
                                <Typography gutterBottom variant="h5">
                                    {t('ThankYouPage.ServiceLogReview.OpenGarageLogbookTitle')}
                                </Typography>
                            </Grid>
                            <Grid item>
                                <Typography gutterBottom variant="body2">
                                    {t('ThankYouPage.ServiceLogReview.OpenGarageLogbookDescription')}
                                </Typography>
                            </Grid>
                            <Grid item>
                                <Button
                                    variant="contained"
                                    onClick={handleServiceLogsClick}
                                >
                                    {t('ThankYouPage.ServiceLogReview.ManageLogbookButton')}
                                </Button>
                            </Grid>
                        </Grid>
                    </Paper>
                    <Paper sx={{ padding: 2, maxWidth: isMobile ? '100%' : '100%', width: '100%' }}>
                        <Grid container direction="column" spacing={1}>
                            <Grid item>
                                <Typography gutterBottom variant="h5">
                                    {t('ThankYouPage.ServiceLogReview.QuestionsOrCommentsTitle')}
                                </Typography>
                            </Grid>
                            <Grid item>
                                <Typography gutterBottom variant="body2">
                                    {t('ThankYouPage.ServiceLogReview.ContactDeveloperDescription')}
                                </Typography>
                            </Grid>
                            <Grid item>
                                <Typography gutterBottom variant="body2">
                                    {t('ThankYouPage.ServiceLogReview.DeveloperName')} Niek Tuijtel
                                </Typography>
                                <Typography variant="body2">
                                    {t('ThankYouPage.ServiceLogReview.DeveloperEmail')} <a href="mailto:n.tuijtel@autohelper.nl">n.tuijtel@autohelper.nl</a>
                                </Typography>
                            </Grid>
                        </Grid>
                    </Paper>
                </Box>
            </Box>
        </Container>
    );
}
