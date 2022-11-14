namespace Equity.Domain

type IndividualEquityPlan = {
    OrgUnit : OrgUnit
    AllocatedShares : double
    PercentAllocated : double
    PercentRemaining : double
    Status : IndividualPlanStatus
}

type EquityPlan = {
    EquityPlanId : EquityPlanId 
    PlanName : PlanName 
    PlanType : PlanType
    AllocationReason : AllocationReason
    EquityValue : EquityValue
    // VestingPeriod : VestingPeriod
    // VestingSchedule : VestingSchedule
    // DiscountRate : DiscountRate
    // EligiblePopulation : EligiblePopulation
    // Conditions : Conditions
    // IndividualEquityPlanItems : IndividualEquityPlan list
    // DateCreated : DateTime
    // DateExpired : DateTime
    // TotalShares : double
    }

 module EquityPlanModule =
    let private createEquityPlan id name planType reason equityValue =
        {
            EquityPlanId = id;
            PlanName = name;
            PlanType = planType
            AllocationReason = reason
            EquityValue = equityValue
        }
    
    let (<!>) = Validation.map
    let (<*>) = Validation.apply

    let createEquityPlanValidated id name planType reason equityValue :Validation<EquityPlan, DomainError> =
        let equityPlanId = id |> EquityPlanId.create
        let planName = name |> PlanName.create
        let planType = planType |> PlanType.create (nameof planType)
        let allocationReason = reason |> AllocationReason.create (nameof reason)
        let equityValue = equityValue |> EquityValue.create
        createEquityPlan <!> equityPlanId <*> planName <*> planType <*> allocationReason <*> equityValue
