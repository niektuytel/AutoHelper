import React from "react";
import { Container, Typography, Paper } from "@mui/material";
import { useTranslation } from "react-i18next";

interface IPrivacyPolicyProps { }
interface IPaymentRulesProps { }

const WhatsAppPrivacyPolicy: React.FC<IPrivacyPolicyProps> = () => {
    const { t } = useTranslation();

    return (
        <Paper elevation={1} sx={{p: 1, m: 2, mt: "75px"}}>
            <Typography variant="h5">{t('privacyPolicy.title')}</Typography>
            <Typography variant="body1">{t('privacyPolicy.content')}</Typography>
        </Paper>
    );
};

const PaymentRules: React.FC<IPaymentRulesProps> = () => {
    const { t } = useTranslation();

    return (
        <Paper elevation={1} sx={{ p: 1, m: 2 }}>
            <Typography variant="h5">{t('paymentRules.title')}</Typography>
            <Typography variant="body1">{t('paymentRules.content')}</Typography>
        </Paper>
    );
};

export default function PolicyAndRulesPage() {
    return <>
        <Container>
            <h2>ALGEMENE VOORWAARDEN</h2>
            <h3>Autohelper Nederland B.V.</h3>
            <h4>Artikel 1 – Definities</h4>
            <p>In deze algemene voorwaarden wordt verstaan onder:</p>
            <ul>
                <li><strong>Klant</strong> Iedere natuurlijk persoon, niet handelend in de uitoefening van een beroep of bedrijf, die opdracht geeft tot de levering van een Dienst en/of Product respectievelijk of een Dienst en/of Product van Autohelper heeft afgenomen.</li>
                <li><strong>Autohelper</strong> Autohelper Nederland B.V.</li>
                <li><strong>APK</strong> De Algemene Periodieke Keuring, zoals door de overheid verplicht gesteld voor auto’s.</li>
                <li><strong>BW</strong> Het Burgerlijk Wetboek</li>
                <li><strong>Levering</strong> Het moment waarop Autohelper de Klant het bezit over het Product verschaft, danwel het voertuig waaraan een Dienst verleend is weer ter beschikking stelt aan de Klant.</li>
                <li><strong>Leveringsprogramma</strong> Het totaal van Diensten en Producten dat Autohelper levert of heeft geleverd, zoals vermeld op de website www.autohelper.nl.</li>
                <li><strong>Diensten</strong> Montage, onderhoud, reparatie, keuringen en alle overige werkzaamheden aan of ten behoeve van (motor) voertuigen uit het Leveringsprogramma van Autohelper.</li>
                <li><strong>Product</strong> Iedere zaak uit het Leveringsprogramma van Autohelper.</li>
                <li><strong>Overeenkomst</strong> Het verrichten van Diensten en/of de verkoop van een Product door Autohelper aan een Klant.</li>
                <li><strong>Overeenkomst op afstand</strong> De overeenkomst waarbij uit het Leveringsprogramma, Autohelper via een webwinkel, of een vergelijkbaar systeem, Producten en/of Diensten te koop aanbiedt en de Klant via internet of een vergelijkbaar systeem Producten of Diensten koopt.</li>
            </ul>

            <h3>Artikel 2 – Toepasselijkheid</h3>
            <p>2.1 Deze algemene voorwaarden zijn van toepassing op de totstandkoming, de inhoud en de nakoming van alle Overeenkomsten tussen de Klant en Autohelper, waaronder begrepen abonnementen en overige speciale overeenkomsten uit het Leveringsprogramma.</p>
            <p>2.2 Van deze algemene voorwaarden kan slechts worden afgeweken indien partijen daarmee uitdrukkelijk schriftelijk hebben ingestemd.</p>

            <h3>Artikel 3 – Conformiteit</h3>
            <p>3.1 Autohelper staat jegens de Klant in voor de deugdelijkheid van door haar aan het voertuig van de Klant gemonteerde Producten in relatie tot de prijs ervan, tenzij er sprake is van:</p>
            <ol type="I">
                <li>niet-inachtneming door de Klant van de door Autohelper of de producent of de importeur van het Product gegeven aanwijzingen of voorschriften voor het gebruik;</li>
                <li>een ander dan volgens algemene verkeersopvattingen normaal gebruik door de Klant;</li>
                <li>gebreken voortkomend uit volgens algemene verkeersopvattingen normal slijtage; of</li>
                <li>gebreken waarvoor de producent van het Product garantie heeft uitgesloten.</li>
            </ol>
            <p>3.2 Autohelper zal de door de Klant gewenste Dienst naar beste vermogen uitvoeren.</p>

            <h3>Artikel 4 – APK-keuringen</h3>
            <p>4.1 Indien de Klant Autohelper verzoekt tot het verrichten van een APK-keuring ter zake van zijn voertuig, zijn de volgende bepalingen van toepassing: De APK keuring geschiedt volgens de door de overheid vastgestelde richtlijnen. De klant is verantwoordelijk voor de staat van onderhoud van het aangeboden voertuig. De APK keuring is uit de aard van de zaak een momentopname en geeft geen garantie voor de afwezigheid van gebreken die of niet geconstateerd kunnen worden of niet onderworpen zijn aan de keuring. Voorts biedt de keuring vanwege het feit dat het een momentopname - 2 - is, geen garantie voor de veiligheid van de auto in de periode gelegen tussen twee APKkeuringen. Autohelper zal bij de APK keuring conform de richtlijnen van de Rijksdienst voor het Wegverkeer handelen en staat niet in voor gebreken die niet zichtbaar zijn bij de keuring of niet aan keuring onderworpen zijn.</p>
            <p>4.2 De Klant is te allen tijde zelf verantwoordelijk voor de voor de APK-keuring vereiste algemene staat van onderhoud van zijn voertuig. De APK keuring voldoet aan de eisen van de RDW en ziet niet op eventuele onderhoudsgebreken die niet vallen onder de richtlijnen van de RDW. Zo ziet de keuring bijvoorbeeld niet op het herkennen van de noodzaak tot de vervanging van onderdelen die noodzakelijk zijn of kunnen zijn, maar die niet vallen onder de te keuren delen van een auto, waaronder ondermeer begrepen: de distributieriem, vervanging van olie en andere vloeistoffen, vervanging van filters. Adviespunten, reparatiepunten en afkeurpunten geven slechts aan welke punten op grond van de APK regeling vermeld moeten worden op het APK formulier.</p>
            <p>4.3 Voor zover Autohelper tijdens de APK-keuring afkeurpunten ontdekt dan zal Autohelper die punten aan de Klant meedelen. Pas wanneer de Klant opdracht geeft om deze afkeurpunten te repareren, zal Autohelper hiertoe overgaan.</p>
            <p>4.4 Autohelper is niet aansprakelijk voor schade, ontstaan aan een voertuig, of onderdelen daarvan als de schade het gevolg is van de noodzakelijke maatregelen die Autohelper moet nemen ten behoeve van de keuring, waaronder begrepen, de wettelijk verplichte meting van emissiewaarden (met inbegrip van maar niet beperkt tot roetmetingen),tenzij de schade het gevolg is van opzet of bewuste roekeloosheid van Autohelper.</p>
            <h3>Artikel 5 – Onderhoudsbeurten</h3>
            <p>5.1 Naast de APK-keuring verricht Autohelper in opdracht van de Klant ook kleine en grote onderhoudsbeurten. Op de website van Autohelper (www.autohelper.nl) staat vermeld welke onderdelen van het voertuig van de Klant worden gecontroleerd (controlepunten). Na het uitvoeren van de onderhoudsbeurt ontvangt de Klant een lijst met controlepunten waarop zichtbaar is welke controles hebben plaatsgevonden. Eventuele afwijkingen op de controlepunten worden aan de Klant medegedeeld. Reparatie of vervanging door Autohelper zal pas plaatsvinden na opdracht van de Klant.</p>
            <p>5.2 De onderhoudsbeurt is uit de aard van de zaak een momentopname en geeft geen garantie voor de afwezigheid van gebreken die of niet geconstateerd kunnen worden of niet onderworpen zijn aan de onderhoudsbeurt. Voorts biedt de onderhoudsbeurt vanwege het feit dat het een momentopname is, geen garantie voor de staat van het onderhoud van de auto in de periode na afloop van de onderhoudsbeurt. Autohelper zal bij de onderhoudsbeurt conform de controlepunten de staat van het voertuig beoordelen. Zo ziet de onderhoudsbeurt bijvoorbeeld niet op het herkennen van de noodzaak tot vervanging van onderdelen die noodzakelijk zijn of kunnen zijn,maar niet vallen onder de controlepunten van de onderhoudsbeurt, of niet kenbaar zijn bij de onderhoudsbeurt, zoals bijvoorbeeld de vervanging van een distributieriem.</p>
            <p>5.3 Autohelper is niet aansprakelijk voor schade, ontstaan aan een voertuig, of onderdelen daarvan als de schade het gevolg is van de noodzakelijke maatregelen die Autohelper moet nemen ten behoeve van het verrichten van de onderhoudsbeurt, tenzij de schade het gevolg is van opzet of bewuste roekeloosheid van Autohelper.</p>

            <h3>Artikel 6 – Opdrachten tot reparatie, montage of vervanging</h3>
            <p>6.1 De levering van Producten is in beginsel inclusief montage, tenzij uit de aard van het te leveren Product of de daarbij gegeven omschrijving volgt dat geen montage kan of zal plaatsvinden.</p>
            <p>6.2 Aan Autohelper gegeven opdrachten tot reparatie, montage of vervanging houden niet mede in de opdracht tot vervanging of vernieuwing van die onderdelen, die pas na demontage zichtbaar worden en alsdan blijken te moeten worden vervangen of vernieuwd, alsmede het repareren van al die gebreken, waarvan bij demontage blijkt en waarvan reparatie raadzaam of noodzakelijk is.</p>
            <p>6.3 Autohelper voert bijkomende reparaties of vervangingen als bedoeld in artikel 6.1 slechts uit nadat de Klant daarvoor toestemming heeft gegeven.</p>
            <p>6.4 Autohelper is gerechtigd om bij de uitvoering van reparaties, montage of vervanging gebruik te maken van de diensten van derden, die de reparatie geheel of gedeeltelijk zullen uitvoeren in opdracht van Autohelper</p>
            <p>6.5 Autohelper is niet aansprakelijk voor schade, ontstaan aan een voertuig, of onderdelen daarvan als de schade het gevolg is van de noodzakelijke maatregelen die Autohelper moet nemen ten behoeve van het verrichten van de reparatie, montage of de vervanging, tenzij de schade het gevolg is van opzet of bewuste roekeloosheid van Autohelper.</p>

            <h3>Artikel 7 - Ruitschade</h3>
            <p>7.1 Op de montagewerkzaamheden en reparatiewerkzaamheden van autoruiten verleent Autohelper garantie voor de levensduur van de auto, mits voldaan is aan de door Autohelper vereiste omstandigheden en het voertuig in een staat verkeert waarin de autoruit deugdelijk geplaatst kan worden. Dit is bijvoorbeeld niet het geval, wanneer er sprake is van corrosie in de raamsponning. Wanneer de door Autohelper gerepareerde en/of geplaatste ruit vervangen wordt door derden loopt de door Autohelper verleende garantie ten einde.</p>
            <p>7.2 De garantie is niet overdraagbaar aan derden en eindigt wanneer de auto in eigendom overgedragen wordt.</p>
            <p>7.3 Autohelper maakt bij de reparatie van ruitschades gebruik van hars. Hoewel Autohelper zich tot het uiterste zal inspannen deze ruitschade te verhelpen, is het mogelijk dat ten gevolge van de werkingen van hars de ruitschade (deels) zichtbaar blijft. Hiervoor is Autohelper niet aansprakelijk.</p>
            <p>7.4 Wanneer de Klant ervoor kiest een noodruitvoorziening te laten plaatsen dient de Klant zich te houden aan de door Autohelper aangegeven maximumsnelheid en dienen alle ramen, en een eventueel zonnedak, gesloten te blijven tijdens het rijden</p>
            <p>7.5 In geval van ruitschade behoudt Autohelper zich het recht voor om de verzekeringsmaatschappij van de Klant op de hoogte te stellen van de ontstane ruitschade, tenzij de Klant hiertegen bezwaren heeft geuit.</p>
            <p>7.6 Op reparaties en montage van ruiten is het bepaalde in artikel 6 van toepassing, tenzij uit het voorstaande anders blijkt.</p>

            <h3>Artikel 8 – Koop op afstand</h3>
            <p>8.1 In geval van koop op afstand die via een webwinkel tot stand komt, maar waarbij de Levering plaatsvindt in een Autohelper filiaal door montage van de Producten aan een auto, heeft de Klant niet het recht om overeenkomstig artikel 7:46 lid 1 BW de koop op afstand binnen zeven dagen na ontvangst van de Producten te ontbinden, en wordt de verkoop gelijk gesteld aan een verkoop in een filiaal, aangezien op de auto gemonteerde Producten door hun aard niet teruggezonden kunnen worden.</p>
            <p>8.2 In geval van koop op afstand die via een webwinkel tot stand komt, waarbij de Producten rechtstreeks aan de Klant gezonden worden, zonder gebruik te maken van overige Diensten van Autohelper, heeft de Klant overeenkomstig artikel 7:46 lid 1 BW wel het recht om binnen zeven dagen na ontvangst van de Producten de koop op afstand te ontbinden.</p>

            <h3>Artikel 9 – Betaling</h3>
            <p>9.1 Tenzij uitdrukkelijk schriftelijk anders is overeengekomen, vindt betaling plaats bij de Levering. Bij specifieke voor de klant bestelde Producten kan vooruitbetaling verlangd worden. Bij koop via de Webshop vindt betaling direct bij het plaatsen van de bestelling plaats en dus voorafgaand aan de levering.</p>
            <p>9.2 Vindt betaling niet plaats bij de Levering en is niet uitdrukkelijk schriftelijk een andere wijze van betaling overeengekomen, dan heeft Autohelper recht op een rentevergoeding van 1% per maand over het bedrag van de schuld over de periode lopend vanaf veertien dagen na de Levering tot aan de dag der algehele voldoening. Voor de berekening van de omvang van het rentebedrag wordt met een maand gelijkgesteld een gedeelte van een maand.</p>
            <p>9.3 Betalingen strekken telkens eerst tot delging van eventuele verschuldigde rente en openstaan gaat de oudste het eerst teniet, ondanks eventuele andersluidende omschrijvingen bij de betaling.</p>
            <p>9.4 Artikel 9.2 is van overeenkomstige toepassing indien op grond van artikel 9.1 een ander tijdstip van betaling is overeengekomen en de betaling op het overeengekomen tijdstip uitblijft.</p>
            <p>9.5 Indien de Klant, na sommatie door Autohelper, in gebreke blijft met de betaling van zijn schuld, is Autohelper gerechtigd incassokosten in rekening te brengen. De incassokosten omvatten gerechtelijke en buitengerechtelijke incassokosten. De buitengerechtelijke incassokosten omvatten alle kosten die Autohelper in rekening gebracht krijgt door - 4 - deurwaarders, incassokantoren, advocaten, etcetera, met een maximum van 15% van de schuld die de Klant aan Autohelper moet betalen, een en ander tenzij de Klant gemotiveerd aannemelijk maakt dat de werkelijke door Autohelper gemaakte buitengerechtelijke incassokosten een lager bedrag belopen.</p>

            <h3>Artikel 10 – Garantie</h3>
            <p>10.1 Tenzij anders overeengekomen (zoals bijvoorbeeld in geval van ruitschade) verleent Autohelper, onverminderd de rechten of vorderingen die de wet de Klant toekent, een garantie van twaalf maanden op de door haar uitgevoerde werkzaamheden en de door haar geleverde onderdelen. Autohelper zal gedurende een periode van 12 maanden na uitvoering van de reparatie of na levering van een reservedeel, kosteloos zorgdragen voor herstel of reparatie van defecten die verband houden met de gebruikte materialen of de uitvoering van de werkzaamheden.</p>
            <p>10.2 Voor een aantal Producten kan een afwijkende garantietermijn gelden. Die termijn en de voorwaarden staan vermeld op www.autohelper.nl.</p>
            <p>10.3 Garantiewerkzaamheden zullen in en vanuit de werkplaatsen van Autohelper worden uitgevoerd. Eventuele vervoerskosten zijn voor rekening van de Klant.</p>
            <p>10.4 Met betrekking tot levering- en montage van katalysatoren geldt een afwijking van de bepalingen van artikel 10.1, met dien verstande dat een garantie wordt verleend op defecten die voortkomen uit productie- of fabricagefouten van de fabrikant. De beoordeling van een garantieclaim dienaangaande zal, vanwege het technische karakter van het onderdeel, worden verricht door de fabrikant in kwestie; het betrokken voertuig zal gedurende de afhandeling van deze claim in opdracht en voor rekening van de Klant worden voorzien van een nieuwe katalysator. Toekenning van de garantieclaim leidt dan tot restitutie.</p>

            <h3>Artikel 11 – Diversen</h3>
            <p>11.1 Indien na uitvoering van de aan Autohelper opgedragen werkzaamheden en kennisgeving daarvan aan de Klant het betreffende voertuig niet binnen zeven werkdagen nadat die kennisgeving heeft plaatsgehad door de Klant is afgehaald, is Autohelper gerechtigd om ter zake aan die Klant stallingskosten in rekening te brengen van € 100,-- per dag of dagdeel.</p>

            <h3>Artikel 12 – Aansprakelijkheid Autohelper</h3>
            <p>12.1 Onverminderd het elders in deze algemene voorwaarden bepaalde is Autohelper aansprakelijk indien een tekortkoming in de nakoming van een verplichting jegens een Klant is te wijten aan haar opzet of bewuste roekeloosheid, of op grond van de wet,rechtshandeling of verkeersopvattingen voor rekening van Autohelper komt.</p>
            <p>12.2 Gebreken die aan het voertuig van de Klant zijn ontstaan als gevolg van door Autohelper onjuist uitgevoerde montagewerkzaamheden zullen kosteloos door Autohelper worden hersteld.</p>
            <p>12.3 Indien Autohelper aansprakelijk is voor schade dan is Autohelper gerechtigd die schade op haar kosten te doen herstellen. Indien de Klant ervoor kiest de schade door een derde te laten herstellen dan is Autohelper slechts gehouden de schade te vergoeden tot het bedrag aan kosten dat zij zelf gemaakt zou hebben indien de Klant aan Autohelper de gelegenheid zou hebben gegeven de schade te (doen) herstellen.</p>
            <p>12.4 In geval van schade als gevolg van aansprakelijkheid van Autohelper is Autohelper uitsluitend aansprakelijk voor directe schade. Autohelper is niet aansprakelijk voor mogelijke gevolgschade van welke aard dan ook, tenzij sprake is van opzet of bewuste roekeloosheid.</p>
            <p>12.5 In geval van schade als gevolg van aansprakelijkheid van Autohelper is Autohelper uitsluitend aansprakelijk voor schade van aan haar toevertrouwde auto’s geparkeerd op het terrein van Autohelper. Autohelper is niet aansprakelijk voor schade geleden buiten het terrein van Autohelper en voor schade die ontstaat op het moment dat de Klant het voertuig zelf bestuurt.</p>
            <p>12.6 In alle gevallen is de aansprakelijkheid van Autohelper beperkt tot het bedrag van de door haar verzekering gedane uitkering, voor zover de schade gedekt wordt door de verzekering, danwel is de aansprakelijkheid van Autohelper beperkt tot het oorspronkelijke factuurbedrag.</p>

            <h3>Artikel 13 – Klachten</h3>
            <p>13.1 Eventuele klachten dient de Klant binnen acht dagen na Levering van de Producten en/of Diensten aan Autohelper te melden, tenzij op grond van artikel 7:23 BW een langere termijn geldt.</p>
            <p>13.2 Indien de Klant niet binnen de op grond van artikel 13.1 geldende termijn heeft gereclameerd, geldt de geleverde Dienst en/of het geleverde Product als deugdelijk.</p>
            <p>13.3 Eventuele door de Klant aan zijn voertuig geconstateerde schade, die volgens de Klant is veroorzaakt tijdens het verrichten van Diensten door Autohelper, dient uiterlijk de eerstvolgende werkdag nadat de Klant zijn voertuig bij het relevante Autohelper filiaal heeft afgehaald, aan dat filiaal te worden gemeld.</p>
            <p>13.4 Voor schade gemeld na de artikel 13.3 genoemde termijn is Autohelper niet aansprakelijk tenzij Autohelper de aansprakelijkheid daarvoor uitdrukkelijk schriftelijk erkent.</p>

            <h3>Artikel 14 – Toepasselijk recht, geschillen en forumkeuze</h3>
            <p>14.1 Op de Overeenkomst(en) tussen Autohelper en de Klant is Nederlands recht van toepassing.</p>
            <p>14.2 Geschillen tussen Autohelper en de Klant zullen eerst door middel van onderling overleg gepoogd te worden opgelost, zo nodig door inschakeling van een door partijen gezamenlijk gekozen expertisebureau ter vaststelling van de oorzaak van de door de Klant gestelde schade.</p>
            <p>14.3 Autohelper draagt de kosten van de expert als bedoeld in artikel 14.2, indien de uitkomst van de expertise uitwijst dat de door de Klant gestelde directe schade veroorzaakt is door Autohelper en zij voor de vergoeding van die schade aansprakelijk is.</p>
            <p>14.4 Indien partijen in onderling overleg het geschil niet tot een oplossing brengen, is de Rechtbank te Zutphen bij uitsluiting bevoegd van het geschil kennis te nemen, tenzij de in Nederland geldende wetgeving dwingend anders voorschrijft.</p>
            <p>14.5 Partijen zijn, in afwijking van het bepaalde in artikel 14.4, bevoegd om binnen de grenzen van het desbetreffende reglement, geschillen aanhangig te maken bij de ombudsman van de Vereniging VACO, Postbus 33, 2300 AA Leiden (E: vaco@kcleiden.nl).</p>

            <h3>Artikel 15 – Slotbepalingen</h3>
            <p>15.1 Nietigheid of vernietigbaarheid van één van de bepalingen of leden daarvan in deze algemene voorwaarden laat de geldigheid van de overige bepalingen dan wel de overige leden onverlet.</p>
            <p>15.2 Partijen verplichten zich dan in te stemmen met een nieuwe bepaling die zoveel mogelijk qua inhoud, bereik en doelstelling met de oude nietige bepaling overeenkomt.</p>

            <h3>Artikel 16 – Privacyverklaring Autohelper</h3>
            <p>Autohelper vindt het belangrijk er voor te zorgen dat onze dienstverlening transparant, veilig en betrouwbaar is. Wij respecteren je privacy en zullen de persoonlijke informatie die je ons verschaft altijd vertrouwelijk behandelen.</p>
            <p>Autohelper Nederland BV is verantwoordelijk voor de verwerking van persoonsgegevens zoals weergegeven in deze privacyverklaring. Hieronder leggen wij uit hoe we bij Autohelper en op onze website www.autohelper.nl met jouw gegevens omgaan, wat je rechten zijn en hoe je gebruik kunt maken van die rechten.</p>
            <p>Als je het interessant vindt, kun je op de website www.hulpbijprivacy.nl uitgebreide onafhankelijke informatie vinden over de Europese privacywetgeving.</p>
            <p>Heb je nog vragen na het lezen van onze privacyverklaring? Stuur dan een mailtje naar n.tuijtel@autohelper.nl. Onze Klantenservice helpt je graag verder!</p>

            <h4>Persoonsgegevens die wij verwerken</h4>
            <p>Een ‘persoonsgegeven’ is alle informatie die direct over iemand gaat, of naar een individuele persoon te herleiden is.</p>
            <p>Autohelper verwerkt je persoonsgegevens doordat je onze websites weleens hebt bezocht, gebruik maakt(e) van onze diensten en/of omdat je deze gegevens zelf aan ons verstrekt.</p>
            <p>Hieronder vind je een overzicht van de persoonsgegevens die wij verwerken:</p>
            <ul>
                <li>Adresgegevens</li>
                <li>E-mailadres</li>
                <li>Telefoonnummers</li>
                <li>Kenteken</li>
                <li>Identificatienummer van een voertuig (VIN)</li>
                <li>Klantnummer</li>
                <li>Bank- en betaalgegevens</li>
                <li>IP-adres, internetbrowser en soort/type apparaat</li>
                <li>Gegevens over jouw activiteiten op onze website</li>
                <li>Informatie over bestellingen, afspraken en facturen</li>
                <li>Reviews: je beoordeling van onze service/producten</li>
                <li>Beelden van beveiligingscamera’s</li>
                <li>Overige persoonsgegevens die je actief verstrekt, bijvoorbeeld in correspondentie en telefonisch</li>
            </ul>

            <h4>Met welk doel en op basis van welke grondslag wij persoonsgegevens verwerken</h4>
            <p>Autohelper verwerkt jouw persoonsgegevens met diverse doelen, waaronder:</p>
            <ul>
                <li>Het afhandelen van jouw afspraak, bestelling en/of betaling</li>
                <li>Om goederen en diensten bij je af te leveren</li>
                <li>Om je te kunnen bellen, SMS’en of e-mailen indien dit nodig is om onze dienstverlening uit te kunnen voeren</li>
                <li>Verzenden van onze nieuwsbrief, informatie over acties en aanbiedingen, serviceberichten en/of onderzoeken</li>
                <li>Om je gepersonaliseerde reclame te tonen</li>
                <li>Informeren over wijzigingen van onze diensten en producten</li>
                <li>Het aanmaken, beveiligen en bijhouden van het Mijn Autohelper-account</li>
                <li>Om jou optimaal te kunnen helpen als je een vraag, probleem of klacht hebt over onze producten en diensten</li>
                <li>Voor de afhandeling van speciale promoties</li>
                <li>Analyses voor procesverbeteringen, rapportages, marktonderzoek, ontwikkeling en verbetering van producten, diensten en applicaties</li>
            </ul>

            <h4>APK: Uitlezen brandstof- en elektriciteitsverbruik</h4>
            <p>Vanaf 1 januari 2024 is het voor APK-keurmeesters verplicht om tijdens de APK-keuring het brandstof- en eventueel elektriciteitsverbruik van voertuigen te verzamelen en samen met het identificatienummer van het voertuig (VIN) aan de RDW door te geven. Dit geldt alleen voor personen- en bedrijfsauto’s tot 3500 kg met een datum eerste toelating van na 1 januari 2021. De gegevens worden verzameld om het werkelijke verbruik van voertuigen te vergelijken met het door de fabrikant opgegeven verbruik, als onderdeel van een Europees initiatief om de nauwkeurigheid van verbruiksgegevens te controleren.</p>
            <p>Een voertuigeigenaar mag het uitlezen weigeren. Meld eventuele bezwaren voorafgaand aan de APK-keuring bij de vestigingsmanager. In dat geval wordt de APK uitgevoerd zonder de gegevens uit te lezen.</p>

            <h4>Camerabeveiliging</h4>
            <p>Onze vestigingen zijn voorzien van camera’s. Dat is van belang voor jouw veiligheid, maar ook voor die van onze collega’s en eigendommen. De camerabeelden worden na maximaal zes weken verwijderd, tenzij er goede redenen zijn om de beelden langer te bewaren, bijvoorbeeld voor onderzoek door een recherchebureau of de politie.</p>

            <h4>Gebruikmaken van ons wifinetwerk</h4>
            <p>Tijdens een bezoek aan een van onze vestigingen kun je gratis gebruikmaken van wifi. Zodra je bent ingelogd op ons wifinetwerk, worden er automatisch gegevens verzameld zoals het identificatienummer (MAC-adres) van jouw apparaat. Deze gegevens worden na een jaar verwijderd en gebruikt Autohelper alleen om eventueel misbruik te voorkomen. Mochten er illegale activiteiten plaatsvinden vanuit ons wifinetwerk, dan kunnen we de toegang van het apparaat waarmee je gebruikmaakt van ons wifinetwerk blokkeren.</p>

            <h4>Cookies, of vergelijkbare technieken, die wij gebruiken</h4>
            <p>Autohelper maakt op haar websites gebruik van cookies en andere technieken. Cookies zijn kleine tekstbestanden die bij het eerste bezoek aan de website worden opgeslagen in de browser van je computer, tablet of smartphone. Ze helpen ons om je webbrowser te herkennen en jouw voorkeuren te onthouden.</p>

            <h4>Google Signals</h4>
            <p>Autohelper maakt gebruik van Google Signals om inzicht te krijgen in het gedrag van onze websitegebruikers over verschillende apparaten heen. Dit helpt ons om onze website te optimaliseren voor een betere gebruikerservaring.</p>

            <h4>E-mails</h4>
            <p>Voor het versturen van onze e-mails gebruiken wij een softwareplatform dat ons inzicht geeft in het leesgedrag van onze nieuwsbriefabonnees. Dit helpt ons om de inhoud van onze communicatie te verbeteren.</p>

            <h4>Gegevens inzien, aanpassen of verwijderen</h4>
            <p>Je hebt het recht om je persoonsgegevens in te zien, te corrigeren of te verwijderen. Je kunt hiervoor contact opnemen met Autohelper via n.tuijtel@autohelper.nl.</p>

            <h4>Jouw gegevens in Mijn Autohelper</h4>
            <p>In Mijn Autohelper kun je zelf jouw persoonlijke gegevens beheren en bijwerken. Je hebt ook de mogelijkheid om jouw account te verwijderen.</p>

            <h4>Opslag van jouw gegevens</h4>
            <p>Autohelper slaat jouw gegevens op binnen de Europese Economische Ruimte (EER). Als gegevens buiten de EER worden opgeslagen, zorgen wij voor passende beveiligingsmaatregelen.</p>

            <h4>Hoe wij persoonsgegevens beveiligen</h4>
            <p>Autohelper neemt passende maatregelen om jouw persoonsgegevens te beschermen tegen ongeautoriseerde toegang, wijziging, openbaarmaking of vernietiging.</p>

            <h4>Websites van derden</h4>
            <p>Onze website bevat links naar externe websites. Autohelper is niet verantwoordelijk voor de privacypraktijken van deze externe websites.</p>

            <h4>Contactinformatie</h4>
            <p>Voor vragen over onze privacypraktijken kun je contact opnemen met Autohelper via n.tuijtel@autohelper.nl of onze Klantenservice.</p>

            <h4>Wijzigingen in deze privacyverklaring</h4>
            <p>Autohelper behoudt zich het recht voor om wijzigingen aan te brengen in deze privacyverklaring. Wij adviseren je om deze verklaring regelmatig te controleren op updates.</p>
        </Container>
    </>;
}