using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using Newtonsoft.Json;

namespace AutoHelper.Application.Vehicles._DTOs;


public class RDWVehicleDetectedDefect
{
    public RDWVehicleDetectedDefect()
    {

    }

    [JsonProperty("kenteken")]
    public string LicensePlate { get; set; }

    [JsonProperty("soort_erkenning_keuringsinstantie")]
    public string TypeOfRecognitionInspection { get; set; }

    [JsonProperty("meld_datum_door_keuringsinstantie_dt")]
    public DateTime DetectionDate { get; set; }

    [JsonProperty("gebrek_identificatie")]
    public string Identifier { get; set; }

    [JsonProperty("soort_erkenning_omschrijving")]
    public string Description { get; set; }

    [JsonProperty("aantal_gebreken_geconstateerd")]
    public int DetectedAmount { get; set; }
}
