using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructo.Domain.Enums;

/// <summary>
/// Enum representing the types of certificates issued by the Romanian Road Authority (ARR)
/// </summary>
[Flags]
public enum ARRCertificateType
{
    None = 0,

    /// <summary>
    /// Certificate for general goods transportation
    /// </summary>
    FreightTransport = 1,

    /// <summary>
    /// Certificate for passenger transportation
    /// </summary>
    PassengerTransport = 2,

    /// <summary>
    /// Certificate for dangerous goods transportation (ADR)
    /// </summary>
    DangerousGoodsTransport = 4,

    /// <summary>
    /// Certificate for oversized load transportation
    /// </summary>
    OversizedTransport = 8,

    /// <summary>
    /// Certificate for taxi transportation
    /// </summary>
    TaxiTransport = 16,

    /// <summary>
    /// Certificate for transport managers (CPI)
    /// </summary>
    TransportManager = 32,

    /// <summary>
    /// Certificate for driving instructors
    /// </summary>
    DrivingInstructor = 64,

    /// <summary>
    /// Certificate for road legislation teachers
    /// </summary>
    RoadLegislationTeacher = 128,

    /// <summary>
    /// Certificate for safety advisors for the transport of dangerous goods
    /// </summary>
    DangerousGoodsSafetyAdvisor = 256
}
