using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoHelper.Messaging.Templates;
public class TestTemplate
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IEnumerable<Order> Orders { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int Qty { get; set; }
    public double Price { get; set; }
}
