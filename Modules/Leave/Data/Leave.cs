using System.ComponentModel.DataAnnotations;
using Harc.Api.Common.Models;
using Harc.Api.Modules.Identity.Data;


namespace Harc.Api.Modules.Leave.Data;

public enum LeaveType { Annual = 1, Sick = 2, Excuse = 3, Unpaid = 4 }
public enum LeaveStatus { Pending = 1, Approved = 2, Rejected = 3 }

public class Leave : BaseEntity
{
    [Key]
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!; 
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Days { get; set; } 
    public LeaveType Type { get; set; }
    public LeaveStatus Status { get; set; }
    public string? Description { get; set; }
}