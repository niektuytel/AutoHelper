using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;

namespace Mailing.Services;
internal class CustomMailingService : IMailingService
{
    public Task SendEmailAsync(string emailAddress, string subject, string message)
    {
        throw new NotImplementedException();
    }

    public Task SendEmailAsync(IEnumerable<string> emailAddresses, string subject, string message)
    {
        throw new NotImplementedException();
    }
}
