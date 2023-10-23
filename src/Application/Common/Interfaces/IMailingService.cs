using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoHelper.Application.Common.Interfaces;

public interface IMailingService
{
    Task SendEmailAsync(string emailAddress, string subject, string message);

    Task SendEmailAsync(IEnumerable<string> emailAddresses, string subject, string message);
}
