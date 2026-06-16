using System.ComponentModel.DataAnnotations;
using Harc.Api.Common.Models;

namespace Harc.Api.Modules.Leave.Data;

public class LeaveSetting : BaseEntity
{
    [Key]
    public int Id { get; set; }
    public int ExperienceThresholdYears { get; set; }
    public double AllowanceBelowThreshold { get; set; }
    public double AllowanceAboveThreshold { get; set; }
}