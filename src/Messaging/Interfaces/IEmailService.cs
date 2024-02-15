using AutoHelper.Messaging.Models.GraphEmail;

namespace AutoHelper.Messaging.Interfaces;
internal interface IEmailService
{
    Task SendEmail(GraphEmail email);
    string GetTestEmailAddress();
    string GetUserId();
}