﻿using RealEstate.Models.Domain.Dto;

namespace RealEstate.DataAccess;

public static class DataStore
{
    public static List<VillaDTO> villaList = new List<VillaDTO>
    {
        new VillaDTO {Id=1, Name="Pool View", Sqft=100, Occupancy=3},
        new VillaDTO {Id=2, Name="Beach View", Sqft=200, Occupancy=4}
    };
}
