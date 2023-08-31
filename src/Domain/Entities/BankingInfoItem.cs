using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Domain.Entities;

public class BankingInfoItem : BaseEntity
{
    public string BankName { get; set; }

    // Chamber of Commerce number
    public string KvKNumber { get; set; }

    // Official business name
    public string AccountHolderName { get; set; }

    // International Bank Account Number
    public string IBAN { get; set; }
}
