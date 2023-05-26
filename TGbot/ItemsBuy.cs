using System;
using System.Collections.Generic;

namespace TGbot;

public partial class ItemsBuy
{
    public int IdItemsBuy { get; set; }

    public int IdOperationItemsBuy { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public decimal? Sum { get; set; }

    public double? Quantity { get; set; }

    public int? PaymentType { get; set; }

    public int? ProductType { get; set; }

    public int? Nds { get; set; }

    public virtual Operation IdOperationItemsBuyNavigation { get; set; } = null!;
}
