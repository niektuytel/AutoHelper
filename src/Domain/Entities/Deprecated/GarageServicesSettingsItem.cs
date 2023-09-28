using AutoHelper.Domain.Common;

namespace AutoHelper.Domain.Entities.Deprecated;

public class GarageServicesSettingsItem : BaseEntity
{
    /// <summary>
    /// Gives and easy start for garage owners to start using the system,
    /// To avoid to much orders from the website and not being able to handle them.
    /// </summary>
    public int MaxAutomaticPlannedOrders { get; set; } = 10;

    /// <summary>
    /// If the garage owner wants to be notified when there is an new order.
    /// </summary>
    public bool TrySendMailOnNewOrders { get; set; } = true;

    /// <summary>
    /// Send garage owner an whatsapp message when there is an new order.
    /// </summary>
    public bool TrySendWhatsappMessagOnNewOrders { get; set; } = true;

    /// <summary>
    /// The owner can set the delivery as an service enabled.
    /// </summary>
    public bool IsDeliveryEnabled { get; set; } = false;

    /// <summary>
    /// We as autohelper can help the garage owner with the delivery service.
    /// </summary>
    public bool IsAuthohelperDeliveryEnabled { get; set; } = true;

    /// <summary>
    /// The price for the delivery service.
    /// </summary>
    public decimal DeliveryPrice { get; set; } = 20;

    /// <summary>
    /// The maximum amount of deliveries that can be planned automaticly.
    /// </summary>
    public int MaxAutomaticPlannedDeliveries { get; set; } = 5;

}