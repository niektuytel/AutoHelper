using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities.Garages.Unused;

public class GarageBankingDetailsItem : BaseEntity
{
    public string BankName { get; set; }

    public string KvKNumber { get; set; }

    public string AccountHolderName { get; set; }

    public string IBAN { get; set; }
}
