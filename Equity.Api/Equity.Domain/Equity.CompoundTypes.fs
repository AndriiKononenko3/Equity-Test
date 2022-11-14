namespace Equity.Domain

type EquityPlan = {
    EquityPlanId : EquityPlanId 
    PlanName : PlanName 
    PlanType : PlanType
    AllocationReason : AllocationReason
    Price : Price
    }

 module EquityPlanModule =
    let private createEquityPlan id name planType reason price =
        {
            EquityPlanId = id;
            PlanName = name;
            PlanType = planType
            AllocationReason = reason
            Price = price
        }
    
    let (<!>) = Validation.map
    let (<*>) = Validation.apply

    let createEquityPlanValidated id name planType reason price :Validation<EquityPlan, DomainError> =
        let equityPlanId = id |> EquityPlanId.create
        let planName = name |> PlanName.create
        let planType = planType |> PlanType.create (nameof planType)
        let allocationReason = reason |> AllocationReason.create (nameof reason)
        let price = price |> Price.create
        createEquityPlan <!> equityPlanId <*> planName <*> planType <*> allocationReason <*> price