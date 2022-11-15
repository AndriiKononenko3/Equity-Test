module Equity.IndividualEquityPlan.Implementation

open Equity.IndividualEquityPlan.PublicTypes

module public IndividualPlanLogic =
    let notImplemented() = failwith "not implemented"

    let moveIndividualPlanInProgress : MoveIndividualPlanInProgress = 
        fun draftIndividualEquityPlan ->
            notImplemented()

    let moveIndividualPlanInReview : MoveIndividualPlanInReview = 
        fun inProgressIndividualEquityPlan ->
            notImplemented()