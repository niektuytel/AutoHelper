using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoHelper.Infrastructure.Common.Models;
public class GoogleApiSearchPlaceItem
{
    public Candidate[] candidates { get; set; }
    public string status { get; set; }
}

public class Candidate
{
    public string place_id { get; set; }
}

