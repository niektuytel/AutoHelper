using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class BankingInfoItem : BaseAuditableEntity
{
    [Required]
    public string BankName { get; set; }

    [Required]
    public string AccountNumber { get; set; }

    public string IBAN { get; set; } // International Bank Account Number

    public string SWIFTCode { get; set; } // Also known as BIC

    //public Guid GarageItemId { get; set; }
    //[ForeignKey("GarageItemId")]
    //public GarageItem Garage { get; set; }
}
