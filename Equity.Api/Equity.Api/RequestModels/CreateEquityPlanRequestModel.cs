namespace Equity.Api.RequestModels;

public record CreateEquityPlanRequestModel(string Name, string Type, string AllocationReason, decimal Price);