namespace Equity.Domain

open System

type Status =
    | Default = 0
    | NotStarted = 1
    | InAllocationProcess = 2
    | InReview = 3
    | Allocated = 4
   
type IndividualEquityPlanDto = {
    Id : Guid
    UserId : Guid
    OrgUnit : OrgUnit
    AllocatedShares : Guid
    PercentAllocated : double
    PercentRemaining : double
    EquityPlanId : Guid
    Status : Status
}

module public IndividualEquityPlanMapping =
    
    let notImplemented() = failwith "not implemented"
    
    let fromIndividualEquityPlanDraft (domainObj:DraftIndividualEquityPlan) :IndividualEquityPlanDto = 
        notImplemented()
        
    let toIndividualEquityPlanDraft (dto:IndividualEquityPlanDto) :Result<DraftIndividualEquityPlan,DomainError list> =
        notImplemented()