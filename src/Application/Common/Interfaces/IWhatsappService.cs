using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoHelper.Application.Common.Interfaces;

public interface IWhatsappService
{
    Task SendConfirmationAsync(string phoneNumber, string message);
    Task SendMessageAsync(string phoneNumber, string message);

    Task SendMessageAsync(IEnumerable<string> phoneNumbers, string message);
}
