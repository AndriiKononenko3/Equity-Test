module Equity.Domain.Logic

open System

module Domain =
    
    type DomainError =
    | PlanNameNotEmpty of string
    | EquityPlanIdNotEmpty of string
    | PlanTypeDoesNotExist of string
    | AllocationReasonDoesNotExist of string
    | DecimalValueExceedsLimit of string
    | NotValidFiatCurrency of string
        static member public getErrorMsg (error: DomainError) =
            match error with
            | PlanNameNotEmpty planNameNotEmptyMsg -> planNameNotEmptyMsg
            | EquityPlanIdNotEmpty equityPlanIdNotEmptyMsg -> equityPlanIdNotEmptyMsg
            | PlanTypeDoesNotExist planTypeDoesNotExistMsg -> planTypeDoesNotExistMsg
            | AllocationReasonDoesNotExist allocationReasonDoesNotExistMsg -> allocationReasonDoesNotExistMsg
            | DecimalValueExceedsLimit decimalValueExceedsLimitMsg -> decimalValueExceedsLimitMsg
            | NotValidFiatCurrency notValidFiatCurrencyMsg -> notValidFiatCurrencyMsg
        
    type OrgUnit = Undefined
    type VestingPeriod = Undefined
    type VestingSchedule = Undefined
    type DiscountRate = Undefined
    type EligiblePopulation = Undefined
    type Conditions = Undefined

    type [<Measure>] percent

    type FiatCurrency =
        | USD 
        | EUR

    module FiatCurrency =
        let create fieldName str :Validation<FiatCurrency, DomainError> = 
            match str with
            | "USD" | "usd" -> 
                Ok USD
            | "EUR" | "eur" -> 
                Ok EUR
            | _ -> 
                // all other cases
                let msg = NotValidFiatCurrency $"%s{fieldName}: Must be one of 'USD', 'EUR'" 
                Error [msg]
                
        let value fiatCurrency = 
            match fiatCurrency with
            | USD -> "USD"
            | EUR -> "EUR"
            
    type CryptoCurrency =
        | BTC 
        | ETH
        | SOL
        
    type Currency =
        | FiatCurrency of FiatCurrency
        | CryptoCurrency of CryptoCurrency

    type IndividualPlanStatus =
        | NotStarted 
        | InAllocationProcess
        | InReview
        | Allocated
                
    [<Struct>]
    type PlanName = PlanName of string

    module PlanName =
        let create str :Validation<PlanName, DomainError> = 
            if String.IsNullOrEmpty(str) then
                let msg = PlanNameNotEmpty "PlanName must not be null or empty"
                Error [msg]
            else
                Ok (PlanName str)
        let value (PlanName v) = v

    [<Struct>]
    type EquityPlanId = EquityPlanId of Guid

    module EquityPlanId =
        let create id :Validation<EquityPlanId, DomainError> = 
            if Guid.Empty = id then
                let msg = EquityPlanIdNotEmpty "EquityPlanId must not empty"
                Error [msg]
            else
                Ok (EquityPlanId id)
                
        let value (EquityPlanId v) = v

    type PlanType =
        | PerformanceShares 
        | RestrictedStockUnit
        | EmployeeStockPurchasePlan
        | EmployeeStockOwnershipPlan
        
     module PlanType =
        let create fieldName str :Validation<PlanType, DomainError> = 
            match str with
            | "PerformanceShares" | "performanceShares" -> 
                Ok PerformanceShares
            | "RestrictedStockUnit" | "restrictedStockUnit" -> 
                Ok RestrictedStockUnit
            | "EmployeeStockPurchasePlan" | "employeeStockPurchasePlan" -> 
                Ok EmployeeStockPurchasePlan
            | "EmployeeStockOwnershipPlan" | "employeeStockOwnershipPlan" -> 
                Ok EmployeeStockOwnershipPlan
            | _ -> 
                // all other cases
                let msg = PlanTypeDoesNotExist $"%s{fieldName}: Must be one of 'PerformanceShares', 'RestrictedStockUnit', 'EmployeeStockPurchasePlan', 'EmployeeStockOwnershipPlan" 
                Error [msg]
                
        let value planType = 
            match planType with
            | PerformanceShares -> "PerformanceShares"
            | RestrictedStockUnit -> "RestrictedStockUnit"
            | EmployeeStockPurchasePlan -> "EmployeeStockPurchasePlan"
            | EmployeeStockOwnershipPlan -> "EmployeeStockOwnershipPlan"

    type AllocationReason =
        | Hire 
        | AnnualReward
        | RetentionReward
        
     module AllocationReason =
        let create fieldName str :Validation<AllocationReason, DomainError> = 
            match str with
            | "Hire" | "hire" -> 
                Ok Hire
            | "AnnualReward" | "annualReward" -> 
                Ok AnnualReward
            | "RetentionReward" | "retentionReward" -> 
                Ok RetentionReward
            | _ -> 
                // all other cases
                let msg = AllocationReasonDoesNotExist $"%s{fieldName}: Must be one of 'Hire', 'AnnualReward', 'RetentionReward" 
                Error [msg]
                
        let value reason = 
            match reason with
            | Hire -> "Hire"
            | AnnualReward -> "AnnualReward"
            | RetentionReward -> "RetentionReward"

    [<Struct>]
    type EquityValue = private EquityValue of amount:decimal * currency:Currency

    module EquityValue =
        let amountValue (EquityValue (v, _)) = v
        
        let currencyValue (EquityValue (_, currency)) =
            match currency with
            | FiatCurrency fiat -> match fiat with
                                        | USD -> USD |> FiatCurrency.value
                                        | EUR -> EUR |> FiatCurrency.value
            | _ -> failwith "todo"

        let create v c :Validation<EquityValue, DomainError> =
            let currencyResult = c |> FiatCurrency.create (nameof c)
            match currencyResult with
            | Ok fiatCurrency -> Ok (EquityValue(v, FiatCurrency fiatCurrency))
            | Error msg -> Error msg
            
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
    
    type MoveIndividualPlanInProgress = 
        DraftIndividualEquityPlan -> Result<InProgressIndividualEquityPlan, DomainError list>
        
    type MoveIndividualPlanInReview = 
        InProgressIndividualEquityPlan -> Result<InReviewIndividualEquityPlan, DomainError list>
        
    let notImplemented() = failwith "not implemented"

    let moveIndividualPlanInProgress : MoveIndividualPlanInProgress = 
        fun draftIndividualEquityPlan ->
            notImplemented()

    let moveIndividualPlanInReview : MoveIndividualPlanInReview = 
        fun inProgressIndividualEquityPlan ->
            notImplemented()