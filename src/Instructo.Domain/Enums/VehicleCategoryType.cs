namespace Instructo.Domain.Enums;

/// <summary>
/// Enum representing the vehicle categories for which driving licenses are issued,
/// as defined in Article 20(2) of the Romanian Road Code.
/// </summary>
public enum VehicleCategoryType
{
    /// <summary>
    /// Mopeds
    /// </summary>
    AM,

    /// <summary>
    /// Motorcycles with maximum 125cm³ cylinder capacity, maximum power of 11kW,
    /// and power-to-weight ratio not exceeding 0.1kW/kg;
    /// Motor tricycles with maximum power of 15kW
    /// </summary>
    A1,

    /// <summary>
    /// Motorcycles with maximum power of 35kW, power-to-weight ratio not exceeding 0.2kW/kg,
    /// and not derived from a vehicle with more than twice its power
    /// </summary>
    A2,

    /// <summary>
    /// Motorcycles with or without sidecar and motor tricycles with power over 15kW
    /// </summary>
    A,

    /// <summary>
    /// Quadricycles with unladen mass not exceeding 400kg (550kg for goods transport vehicles),
    /// excluding the mass of batteries for electric vehicles,
    /// equipped with internal combustion engine not exceeding 15kW net maximum power
    /// or electric motor not exceeding 15kW continuous rated power
    /// </summary>
    B1,

    /// <summary>
    /// Vehicles with maximum authorized mass not exceeding 3,500kg and with no more than
    /// 8 seats in addition to the driver's seat;
    /// Vehicle-trailer combinations where the trailer's maximum authorized mass doesn't exceed 750kg;
    /// Vehicle-trailer combinations not exceeding 4,250kg total, where the trailer's
    /// maximum authorized mass exceeds 750kg
    /// </summary>
    B,

    /// <summary>
    /// Vehicle-trailer combinations exceeding 4,250kg total, comprising a category B vehicle
    /// and a trailer or semi-trailer with maximum authorized mass not exceeding 3,500kg
    /// </summary>
    BE,

    /// <summary>
    /// Vehicles other than those in categories D or D1, with maximum authorized mass
    /// exceeding 3,500kg but not exceeding 7,500kg, designed to carry maximum 8 passengers
    /// in addition to the driver. These vehicles may be coupled with a trailer
    /// not exceeding 750kg maximum authorized mass
    /// </summary>
    C1,

    /// <summary>
    /// Vehicle-trailer combinations comprising a C1 vehicle and a trailer or semi-trailer
    /// with maximum authorized mass exceeding 750kg, provided the total doesn't exceed 12,000kg;
    /// Combinations where the towing vehicle is category B and the trailer or semi-trailer
    /// has a maximum authorized mass exceeding 3,500kg, provided the total doesn't exceed 12,000kg
    /// </summary>
    C1E,

    /// <summary>
    /// Vehicles other than those in categories D or D1, with maximum authorized mass
    /// exceeding 3,500kg, designed to carry maximum 8 passengers in addition to the driver;
    /// Combinations comprising a category C vehicle and a trailer with maximum authorized
    /// mass not exceeding 750kg
    /// </summary>
    C,

    /// <summary>
    /// Vehicle-trailer combinations comprising a category C vehicle and a trailer or
    /// semi-trailer with maximum authorized mass exceeding 750kg
    /// </summary>
    CE,

    /// <summary>
    /// Vehicles designed to carry maximum 16 passengers in addition to the driver,
    /// with maximum length not exceeding 8m;
    /// Combinations comprising a D1 vehicle and a trailer with maximum authorized mass
    /// not exceeding 750kg
    /// </summary>
    D1,

    /// <summary>
    /// Vehicle-trailer combinations comprising a D1 vehicle and a trailer with maximum
    /// authorized mass exceeding 750kg. The trailer must not be designed to carry passengers
    /// </summary>
    D1E,

    /// <summary>
    /// Vehicles designed to carry more than 8 passengers in addition to the driver.
    /// These vehicles may be coupled with a trailer not exceeding 750kg maximum authorized mass
    /// </summary>
    D,

    /// <summary>
    /// Vehicle-trailer combinations comprising a category D vehicle and a trailer
    /// with maximum authorized mass exceeding 750kg. The trailer must not be designed
    /// to carry passengers
    /// </summary>
    DE,

    /// <summary>
    /// Agricultural or forestry tractors
    /// </summary>
    Tr,

    /// <summary>
    /// Trolleybus
    /// </summary>
    Tb,

    /// <summary>
    /// Tram
    /// </summary>
    Tv
}
