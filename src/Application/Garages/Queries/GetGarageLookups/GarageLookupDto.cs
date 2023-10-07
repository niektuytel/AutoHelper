﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;

namespace AutoHelper.Application.Garages.Queries.GetGaragesLookups;

public class GarageLookupDto
{
    public GarageLookupDto(GarageLookupItem garageLookupItem, double distanceInMeters)
    {
        GarageId = garageLookupItem.GarageId;
        Name = garageLookupItem.Name;
        Address = garageLookupItem.Address;
        City = garageLookupItem.City;
        Website = garageLookupItem.Website;
        FirstPlacePhoto = garageLookupItem.FirstPlacePhoto;
        DaysOfWeek = garageLookupItem.DaysOfWeek == null ? new int[0] : garageLookupItem.DaysOfWeek;
        KnownServices = garageLookupItem.KnownServices;
        DistanceInMeter = (int)distanceInMeters;
        Rating = garageLookupItem.Rating;
        UserRatingsTotal = garageLookupItem.UserRatingsTotal;

        HasPickupService = true;// TODO: Implement pickup service
        HasReplacementTransportService = true;// TODO: Implement replacement transport service
        HasBestPrice = true;// TODO: Implement best price
    }

    public Guid? GarageId { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string? Website { get; set; }

    public string? FirstPlacePhoto { get; set; }

    public int[] DaysOfWeek { get; set; }

    public string[] KnownServices { get; set; }

    public float? Rating { get; set; }

    public int? UserRatingsTotal { get; set; }

    /// <summary>
    /// Only used for filtering and ordering
    /// </summary>
    public int DistanceInMeter { get; set; }

    public bool HasPickupService { get; set; } = false;

    public bool HasReplacementTransportService { get; set; } = false;

    public bool HasBestPrice { get; set; } = false;
}