namespace Equity.CSharp.Api.RequestModels;

public record CreateEquityPlanRequestModel(string Name, string Type, string AllocationReason, decimal Amount, string Currency);