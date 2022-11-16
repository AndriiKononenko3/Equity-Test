namespace Equity.Domain

open System

type DraftIndividualEquityPlan = {
    Id : Guid
    UserId : Guid
    OrgUnit : OrgUnit
    AllocatedShares : decimal
    EquityPlanId : EquityPlanId
}

type InProgressIndividualEquityPlan = {
    Id : Guid
    UserId : Guid
    OrgUnit : OrgUnit
    AllocatedShares : decimal
    EquityPlanId : EquityPlanId
}

type InReviewIndividualEquityPlan = {
    Id : Guid
    UserId : Guid
    OrgUnit : OrgUnit
    AllocatedShares : decimal
    EquityPlanId : EquityPlanId
}

type AllocatedIndividualEquityPlan = {
    Id : Guid
    UserId : Guid
    OrgUnit : OrgUnit
    AllocatedShares : decimal
    PercentAllocated : decimal<percent>
    PercentRemaining : decimal<percent>
    EquityPlanId : EquityPlanId
}

type IndividualEquityPlan = {
    OrgUnit : OrgUnit
    AllocatedShares : decimal
    PercentAllocated : decimal<percent>
    PercentRemaining : decimal<percent>
    Status : IndividualPlanStatus
}

type EquityPlanTemplate = {
    EquityPlanId : EquityPlanId 
    PlanName : PlanName 
    PlanType : PlanType
    AllocationReason : AllocationReason
    EquityValue : EquityValue
    // VestingPeriod : VestingPeriod
    // VestingSchedule : VestingSchedule
    // DiscountRate : DiscountRate
    // EligiblePopulation : EligiblePopulation
    // Conditions : Conditions // assigns population
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

    let createEquityPlanValidated id name planType reason equityAmount equityCurrency :Validation<EquityPlanTemplate, DomainError> =
        let equityPlanId = id |> EquityPlanId.create
        let planName = name |> PlanName.create
        let planType = planType |> PlanType.create (nameof planType)
        let allocationReason = reason |> AllocationReason.create (nameof reason)
        let equityValue = EquityValue.create equityAmount equityCurrency
        createEquityPlan <!> equityPlanId <*> planName <*> planType <*> allocationReason <*> equityValue