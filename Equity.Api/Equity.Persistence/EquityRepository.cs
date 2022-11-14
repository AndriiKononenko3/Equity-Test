using Equity.Domain;
using LanguageExt;

namespace Equity.Persistence;

public class EquityRepository
{
    public async Task<Option<EquityPlanDto>> GetAsync(Guid id)
    {
        var equity = new EquityPlanDto(id, "Name1", "PerformanceShares", "Hire", 24, "usd");
        
        return id == Guid.Empty ? Option<EquityPlanDto>.None : Option<EquityPlanDto>.Some(equity);
    }
}