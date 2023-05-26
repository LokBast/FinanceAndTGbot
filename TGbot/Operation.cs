using System;
using System.Collections.Generic;

namespace TGbot;

public partial class Operation
{
    public int IdOperations { get; set; }

    public string? Userid { get; set; }

    public string? UserInn { get; set; }

    public int? RequestNumber { get; set; }

    public int? ShiftNumber { get; set; }

    public int? OperationType { get; set; }

    public int? TotalSum { get; set; }

    public int? CashTotalSum { get; set; }

    public int? EcashTotalSum { get; set; }

    public string? KktRegId { get; set; }

    public int? FiscalDocumentNumber { get; set; }

    public string? FiscalDriveNumber { get; set; }

    public long? FiscakSign { get; set; }

    public int? Nds18 { get; set; }

    public int? Code { get; set; }

    public int? FiscalDocumentFormatVer { get; set; }

    public string? MachineNumber { get; set; }

    public string? BuyerPhoneOrAddress { get; set; }

    public int? PrepaidSum { get; set; }

    public int? CreditSum { get; set; }

    public int? ProvisionSum { get; set; }

    public int? InternetSign { get; set; }

    public string? SellerAddress { get; set; }

    public int? DateTime { get; set; }

    public int? TaxationType { get; set; }

    public virtual ICollection<ItemsBuy> ItemsBuys { get; set; } = new List<ItemsBuy>();
}
