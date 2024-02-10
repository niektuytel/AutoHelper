using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Domain.Entities.Admin;

public class RequestLogItem
{
    public RequestLogItem() { }

    public RequestLogItem(
        IBaseRequest request,
        LogLevel logLevel,
        string logMessage
    )
    {
        Id = Guid.NewGuid();
        Date = DateTime.UtcNow;
        LogLevel = logLevel;
        LogMessage = logMessage;
        SetRequest(request);
    }

    [Key]
    [Required]
    public Guid Id { get; private set; }

    [Required]
    public DateTime Date { get; private set; }

    [Required]
    public LogLevel LogLevel { get; private set; }

    [Required]
    public string LogMessage { get; private set; }

    [Required]
    public string RequestTypeName { get; set; }

    [Required]
    public string RequestJson { get; set; }

    [Required]
    public bool IsSolved { get; set; } = false;

    [NotMapped]
    public IBaseRequest Request => DeserializeRequest();

    private void SetRequest(IBaseRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            var requestType = request.GetType();
            RequestTypeName = requestType.AssemblyQualifiedName ?? throw new InvalidOperationException("Request type name is null.");
            RequestJson = JsonSerializer.Serialize(request, requestType);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error during serialization: {ex.Message}");
        }
    }

    private IBaseRequest DeserializeRequest()
    {
        if (string.IsNullOrEmpty(RequestTypeName))
        {
            throw new InvalidOperationException("Request type name is null or empty.");
        }

        var requestType = Type.GetType(RequestTypeName, throwOnError: true);
        var deserializedRequest = JsonSerializer.Deserialize(RequestJson, requestType!);
        if (deserializedRequest is not IBaseRequest baseRequest)
        {
            throw new InvalidOperationException("Deserialized request is not of expected base request type.");
        }

        return baseRequest;
    }

}