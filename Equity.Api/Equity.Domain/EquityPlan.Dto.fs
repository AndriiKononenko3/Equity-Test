namespace Equity.Domain

open System

type EquityPlanDto = {
    Id : Guid
    Name : string
    Type : string
    AllocationReason : string
    EquityValue : decimal
    EquityCurrency : string
    }

module public EquityPlanMapping =
    
    let fromEquityPlan (domainObj:EquityPlan) :EquityPlanDto = 
        {
            Id = domainObj.EquityPlanId |> EquityPlanId.value
            Name = domainObj.PlanName |> PlanName.value
            Type = domainObj.PlanType |> PlanType.value
            AllocationReason = domainObj.AllocationReason |> AllocationReason.value
            EquityValue = domainObj.EquityValue |> EquityValue.amountValue
            EquityCurrency = domainObj.EquityValue |> EquityValue.currencyValue
        }
        
    let toEquityPlan (dto:EquityPlanDto) :Result<EquityPlan,DomainError list> =
        result {
            let! equityPlanId = dto.Id |> EquityPlanId.create
            let! planName = dto.Name |> PlanName.create
            let! planType = dto.Type |> PlanType.create (nameof dto.Type)
            let! allocationReason = dto.AllocationReason |> AllocationReason.create (nameof dto.AllocationReason)
            let! price = EquityValue.create dto.EquityValue dto.EquityCurrency
            
            let equityPlan = {
                EquityPlanId = equityPlanId
                PlanName = planName
                PlanType = planType
                AllocationReason = allocationReason
                EquityValue = price;
            }
            return equityPlan
            }