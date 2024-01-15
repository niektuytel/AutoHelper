using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoHelper.Messaging.Templates;

public class TestTemplateModel : PageModel
{
    public string Identifier { get; set; }
    public string LicensePlate { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Content { get; set; }
    public string ConversationId { get; set; }
}
