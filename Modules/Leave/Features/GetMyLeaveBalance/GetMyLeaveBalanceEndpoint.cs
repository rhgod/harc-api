using FastEndpoints;
using Harc.Api.Modules.Leave.Data;
using Microsoft.EntityFrameworkCore;
using Harc.Api.Modules.Identity.Data;

namespace Harc.Api.Modules.Leave.Features.GetMyLeaveBalance;

public class GetMyLeaveBalanceEndpoint : EndpointWithoutRequest<GetMyLeaveBalanceResponse>
{
    private readonly IdentityDbContext _dbContext;

    public GetMyLeaveBalanceEndpoint(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("api/leave/my-balance");
        Description(b => b.WithName("GetMyLeaveBalance").WithTags("Leave"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdString = User.FindFirst("harc_user_id")?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var user = await _dbContext.Users
            .Include(u => u.Leaves)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        // 1. Veritabanından İzin Ayarlarını Çek
        // Eğer tabloda hiç kayıt yoksa patlamaması için kod içinde fallback (varsayılan) atıyoruz.
        var settings = await _dbContext.LeaveSettings.FirstOrDefaultAsync(ct)
                       ?? new LeaveSetting
                       {
                           ExperienceThresholdYears = 15,
                           AllowanceBelowThreshold = 20,
                           AllowanceAboveThreshold = 25
                       };

        // 2. Şirketteki yılları hesapla
        double totalLeaveQuota = 0;
        var today = DateTime.UtcNow.Date;
        int yearsInCompany = today.Year - user.EmploymentStartDate.Year;
        if (user.EmploymentStartDate.Date > today.AddYears(-yearsInCompany)) yearsInCompany--;

        double startingPriorExperienceYears = user.PriorExperienceMonths / 12.0;

        // 3. HARDCODED DEĞERLER YERİNE SETTINGS'İ KULLAN
        if (yearsInCompany >= 1)
        {
            for (int i = 1; i <= yearsInCompany; i++)
            {
                double totalExperienceAtAnniversary = startingPriorExperienceYears + i;

                // Ayarlar tablosundan okuyoruz
                if (totalExperienceAtAnniversary >= settings.ExperienceThresholdYears)
                    totalLeaveQuota += settings.AllowanceAboveThreshold;
                else
                    totalLeaveQuota += settings.AllowanceBelowThreshold;
            }
        }

        double usedLeaveDays = user.Leaves
            .Where(l => l.Status == LeaveStatus.Approved && l.Type == LeaveType.Annual)
            .Sum(l => l.Days);

        // --- GELECEK HAKEDİŞ HESAPLAMA ---
        var thisYearAnniversary = new DateTime(today.Year, user.EmploymentStartDate.Month, user.EmploymentStartDate.Day);
        DateTime nextAnniversaryDate = thisYearAnniversary < today ? thisYearAnniversary.AddYears(1) : thisYearAnniversary;

        int daysUntilNextAnniversary = (nextAnniversaryDate - today).Days;

        int yearsAtNextAnniversary = nextAnniversaryDate.Year - user.EmploymentStartDate.Year;
        double experienceAtNextAnniversary = startingPriorExperienceYears + yearsAtNextAnniversary;

        double nextAllowanceAmount = (experienceAtNextAnniversary >= settings.ExperienceThresholdYears) ? settings.AllowanceAboveThreshold : settings.AllowanceBelowThreshold;

        var response = new GetMyLeaveBalanceResponse(
            totalLeaveQuota,
            usedLeaveDays,
            totalLeaveQuota - usedLeaveDays,
            nextAnniversaryDate,
            daysUntilNextAnniversary,
            nextAllowanceAmount
        );

        await Send.OkAsync(response, ct);
    }
}