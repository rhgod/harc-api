namespace Harc.Api.Modules.Leave.Features.GetMyLeaveBalance;

public record GetMyLeaveBalanceResponse(
    double TotalLeaveQuota, 
    double UsedLeaveDays,   
    double RemainingLeaveDays,
    DateTime NextAnniversaryDate, // Bir sonraki hakedişin yükleneceği tarih
    int DaysUntilNextAnniversary, // Kalan gün sayısı (Frontend'de kolayca "15 gün sonra" yazabilmek için)
    double NextAllowanceAmount    // O tarihte yüklenecek olan miktar (20 veya 25)
);