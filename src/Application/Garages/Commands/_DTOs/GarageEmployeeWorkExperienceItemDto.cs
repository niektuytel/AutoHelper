﻿using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities;

namespace AutoHelper.Application.Garages.Commands.DTOs;

public class GarageEmployeeWorkExperienceItemDto
{
    [Required]
    public Guid ServiceId { get; set; }

    [Required]
    public string Description { get; set; } = "";

}