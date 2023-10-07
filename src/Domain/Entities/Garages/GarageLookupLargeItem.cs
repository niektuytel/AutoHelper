using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Domain.Entities.Deprecated;
using System.Drawing;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageLookupLargeItem : BaseEntity
{
    public string GoogleApiDetailsJson { get; set; } = "";

}
