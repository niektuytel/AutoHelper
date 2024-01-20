# AutoHelper
Able to let customers help with their car

## Updating/Creating garage service type
- Add service type to the Autohelper Domain GarageServiceType.cs file.
- Add service type represented string value inside i18n folder serviceTypes.json files.
- Add service type to representing vehicle type(s) inside the 'GarageService.cs'


# Will use following 3th parties
- https://opendata.rdw.nl/Voertuigen/Open-Data-RDW-Gekentekende_voertuigen/m9d7-ebf2
- Hangfire
- 

following types of using whatsapp we accepted:
- send whatsapp message -> {"object":"whatsapp_business_account","entry":[{"id":"107289168858080","changes":[{"value":{"messaging_product":"whatsapp","metadata":{"display_phone_number":"15550285719","phone_number_id":"113037821608895"},"contacts":[{"profile":{"name":"Niek Tuytel"},"wa_id":"31618395668"}],"messages":[{"from":"31618395668","id":"wamid.HBgLMzE2MTgzOTU2NjgVAgASGCAxODdBNTAxRDYxM0ZDRDY0REREQ0JCOERBMEEyOTgwQgA=","timestamp":"1705697501","text":{"body":"Mag ik een onderhou plamnen?"},"type":"text"}]},"field":"messages"}]}]}
- send whatsapp message (a reply on prev message) -> {"object":"whatsapp_business_account","entry":[{"id":"107289168858080","changes":[{"value":{"messaging_product":"whatsapp","metadata":{"display_phone_number":"15550285719","phone_number_id":"113037821608895"},"contacts":[{"profile":{"name":"Niek Tuytel"},"wa_id":"31618395668"}],"messages":[{"context":{"from":"15550285719","id":"wamid.HBgLMzE2MTgzOTU2NjgVAgARGBI4RkFFNEFEQzdENUZGMTM3RTAA"},"from":"31618395668","id":"wamid.HBgLMzE2MTgzOTU2NjgVAgASGCAxRjJFQUFCMzEwNzg4REE5RTVERjBEMzA3QUI2OTRGMAA=","timestamp":"1705697907","text":{"body":"Okee is goed hoor graag meer van u"},"type":"text"}]},"field":"messages"}]}]}


