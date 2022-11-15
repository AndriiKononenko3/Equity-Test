module Equity.IndividualEquityPlan.PublicTypes

open Equity.Domain

type MoveIndividualPlanInProgress = 
    DraftIndividualEquityPlan -> Result<InProgressIndividualEquityPlan, DomainError list>
    
type MoveIndividualPlanInReview = 
    InProgressIndividualEquityPlan -> Result<InReviewIndividualEquityPlan, DomainError list>