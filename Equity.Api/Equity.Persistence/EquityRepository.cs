using Equity.Domain;
using LanguageExt;

namespace Equity.Persistence;

public class EquityRepository
{
    public async Task<Option<EquityPlanTemplateDto>> GetAsync(Guid id)
    {
        var equity = new EquityPlanTemplateDto(id, "Name1", "PerformanceShares", "Hire", 24, "usd");
        
        return id == Guid.Empty ? Option<EquityPlanTemplateDto>.None : Option<EquityPlanTemplateDto>.Some(equity);
    }
}