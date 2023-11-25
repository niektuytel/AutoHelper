namespace AutoHelper.Application.Garages._DTOs;

public class BriefBankingDetailsDtoItem
{
    public string BankName { get; set; }

    // Chamber of Commerce number
    public string KvKNumber { get; set; }

    // Official business name
    public string AccountHolderName { get; set; }

    // International Bank Account Number
    public string IBAN { get; set; }
}