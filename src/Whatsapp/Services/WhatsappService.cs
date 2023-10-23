﻿using System.Net.Http;
using AutoHelper.Application.Common.Interfaces;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace AutoHelper.Whatsapp.Services;

internal class WhatsappService : IWhatsappService
{
    private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;

    public WhatsappService(IWhatsAppBusinessClient whatsAppBusinessClient)
    {
        _whatsAppBusinessClient = whatsAppBusinessClient;
    }

    public async Task SendConfirmationAsync(string phoneNumber, string message)
    {

        TextTemplateMessageRequest textTemplateMessage = new TextTemplateMessageRequest();
        textTemplateMessage.To = phoneNumber;
        textTemplateMessage.Template = new TextMessageTemplate();
        textTemplateMessage.Template.Name = "hello_world";
        textTemplateMessage.Template.Language = new TextMessageLanguage();
        textTemplateMessage.Template.Language.Code = "en_US";

        var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(textTemplateMessage);

        //TextMessageRequest textMessageRequest = new TextMessageRequest();
        //textMessageRequest.To = phoneNumber;
        //textMessageRequest.Text = new WhatsAppText();
        //textMessageRequest.Text.Body = "Message Body";
        //textMessageRequest.Text.PreviewUrl = false;

        //var results = await _whatsAppBusinessClient.SendTextMessageAsync(textMessageRequest);
    }

    public Task SendMessageAsync(string phoneNumber, string message)
    {
        throw new NotImplementedException();
    }

    public Task SendMessageAsync(IEnumerable<string> phoneNumbers, string message)
    {
        throw new NotImplementedException();
    }
}