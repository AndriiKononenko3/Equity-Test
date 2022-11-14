namespace Equity.Domain

open System

type OrgUnit = Undefined
type VestingPeriod = Undefined
type VestingSchedule = Undefined
type DiscountRate = Undefined
type EligiblePopulation = Undefined
type Conditions = Undefined

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
type EquityValue = private EquityValue of decimal

module EquityValue =
    let value (EquityValue v) = v

    let create v :Validation<EquityValue, DomainError> = 
        let result = ConstrainedType.createDecimal "Price" EquityValue 0.0M 1_000_000_000M v
        match result with
        | Ok result -> Ok result
        | Error msg -> Error[msg]

    let multiply qty (EquityValue p) = 
        create (qty * p)
