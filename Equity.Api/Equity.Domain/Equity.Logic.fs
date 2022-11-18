module Equity.Domain.Logic

open System
open EventSourced

module Domain =
    
    type ValidationError =
    | PlanNameNotEmpty of string
    | EquityPlanIdNotEmpty of string
    | PlanTypeDoesNotExist of string
    | AllocationReasonDoesNotExist of string
    | DecimalValueExceedsLimit of string
    | NotValidFiatCurrency of string
        static member public getErrorMsg (error: ValidationError) =
            match error with
            | PlanNameNotEmpty planNameNotEmptyMsg -> planNameNotEmptyMsg
            | EquityPlanIdNotEmpty equityPlanIdNotEmptyMsg -> equityPlanIdNotEmptyMsg
            | PlanTypeDoesNotExist planTypeDoesNotExistMsg -> planTypeDoesNotExistMsg
            | AllocationReasonDoesNotExist allocationReasonDoesNotExistMsg -> allocationReasonDoesNotExistMsg
            | DecimalValueExceedsLimit decimalValueExceedsLimitMsg -> decimalValueExceedsLimitMsg
            | NotValidFiatCurrency notValidFiatCurrencyMsg -> notValidFiatCurrencyMsg
        
    type [<Measure>] percent
    type Conditions = Undefined // not for Alpha

    type FiatCurrency =
        | USD 
        | EUR

    module FiatCurrency =
        let create fieldName str :Validation<FiatCurrency, ValidationError> = 
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
        let create str :Validation<PlanName, ValidationError> = 
            if String.IsNullOrEmpty(str) then
                let msg = PlanNameNotEmpty "PlanName must not be null or empty"
                Error [msg]
            else
                Ok (PlanName str)
        let value (PlanName v) = v

    [<Struct>]
    type EquityPlanId = EquityPlanId of Guid

    module EquityPlanId =
        let create id :Validation<EquityPlanId, ValidationError> = 
            if id = Guid.Empty then
                let msg = EquityPlanIdNotEmpty "EquityPlanId must not zero"
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
        let create fieldName str :Validation<PlanType, ValidationError> = 
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
        let create fieldName str :Validation<AllocationReason, ValidationError> = 
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

        let create v c :Validation<EquityValue, ValidationError> =
            let currencyResult = c |> FiatCurrency.create (nameof c)
            match currencyResult with
            | Ok fiatCurrency -> Ok (EquityValue(v, FiatCurrency fiatCurrency))
            | Error msg -> Error msg
       
    type DraftManagersEquityPlan = {
        Id : int
        ManagerId : Guid
        OrgUnitId : Guid
        SharesToAllocate : decimal
        EquityPlanId : EquityPlanId
    }
    
    type ApprovedManagersEquityPlan = {
        Id : int
        ManagerId : Guid
        OrgUnitId : Guid
        AllocatedShares : decimal
        EquityPlanId : EquityPlanId
    }

    type DraftEmployeesEquityPlan = {
        Id : int
        ManagerId : Guid
        EmployeeId : Guid
        OrgUnitId : Guid
        SharesToAllocate : decimal
        EquityPlanId : EquityPlanId
    }
    
    type AllocatedEmployeesEquityPlan = {
        Id : int
        ManagerId : Guid
        EmployeeId : Guid
        OrgUnitId : Guid
        AllocatedShares : decimal
        PercentAllocated : decimal<percent>
        PercentRemaining : decimal<percent>
        EquityPlanId : EquityPlanId
    }
    
    // some thoughts around tree-like structure
    type Tree<'LeafData,'INodeData> =
    | LeafNode of 'LeafData
    | InternalNode of 'INodeData * Tree<'LeafData,'INodeData> seq
    
    type DaftIndividualEquityPlan = Tree<DraftEmployeesEquityPlan,DraftManagersEquityPlan>
    
    let fromEmployeesPlan (employeesPlan:DraftEmployeesEquityPlan) =
        LeafNode employeesPlan

    let fromManagersPlan (managersPlan:DraftManagersEquityPlan) employeesPlans =
        InternalNode (managersPlan, employeesPlans)
    
    type AllocatedIndividualEquityPlan = Tree<AllocatedEmployeesEquityPlan,ApprovedManagersEquityPlan>
    
    type EligiblePopulation = {
        IncludedOrgUnits : Guid list option
        IncludedEmployees : Guid list option
        ExcludedOrgUnits : Guid list option
        ExcludedEmployees : Guid list option
    }
    
    type VestingSchedule = {
        Date : DateOnly
        Amount : decimal
    }

    type EquityPlanTemplate = {
        EquityPlanId : EquityPlanId 
        PlanName : PlanName 
        PlanType : PlanType
        AllocationReason : AllocationReason
        TotalShares : decimal
        EquityValue : EquityValue
        VestingPeriodFrom : DateOnly
        VestingSchedule : VestingSchedule list
        EligiblePopulation : EligiblePopulation
        DiscountRate : decimal<percent> option
        DateCreated : DateTime
        DateExpired : DateOnly
        }
    
    type DomainError =
    | EquityPlanTemplateAlreadyCreated
    | VestingScheduleItemAlreadyAddedInEquityPlanTemplate of VestingSchedule
    
    type Event =
      | EquityPlanTemplateCreated of EquityPlanTemplate
      | EligiblePopulationUpdated of EligiblePopulation
      | ItemAddedToVestingSchedule of VestingSchedule
      | ItemRemovedFromVestingSchedule of VestingSchedule
      | TotalSharesUpdated of decimal
      | EquityValueUpdated of EquityValue
      | PlanNameChanged of string
      | DraftManagersEquityPlanAssigned of DraftManagersEquityPlan
      | DraftEmployeesEquityPlanAssigned of DraftEmployeesEquityPlan
      | DomainError of DomainError
      
    type Command =
      | CreateEquityPlanTemplate of EquityPlanTemplate
      | ChangePlanName of string
      | UpdateEligiblePopulation of EligiblePopulation
      | AddItemToVestingSchedule of VestingSchedule
      | RemoveItemToVestingSchedule of VestingSchedule
      | UpdateTotalShares of decimal
      | UpdateEquityValue of EquityValue
      | AssignDraftManagersEquityPlan of DraftManagersEquityPlan
      | AssignDraftEmployeesEquityPlan of DraftEmployeesEquityPlan
      
    let evolve (equityPlanTemplate : EquityPlanTemplate) event : EquityPlanTemplate =
      match event with
        | EquityPlanTemplateCreated equityPlanTemplate ->
            equityPlanTemplate
            
        | EligiblePopulationUpdated eligiblePopulation ->
            { equityPlanTemplate with EligiblePopulation = eligiblePopulation }
            
        | ItemAddedToVestingSchedule schedule ->
            { equityPlanTemplate with VestingSchedule = schedule :: equityPlanTemplate.VestingSchedule }
            
        | ItemRemovedFromVestingSchedule schedule ->
            let newSchedule =
              equityPlanTemplate.VestingSchedule
              |> List.filter (fun sch -> sch.Date <> schedule.Date)
              
            { equityPlanTemplate with VestingSchedule = newSchedule }
        
        | _ -> failwith "todo"
        
    let private emptyEquityPlanTemplate : EquityPlanTemplate =
      {
        EquityPlanId = EquityPlanId <| Guid.Empty
        PlanName = PlanName "Empty"
        PlanType = PerformanceShares  
        AllocationReason = Hire
        TotalShares = 0m
        EquityValue = EquityValue (0m, FiatCurrency USD)
        VestingPeriodFrom = DateOnly.MinValue
        VestingSchedule = List.Empty
        EligiblePopulation = { IncludedOrgUnits = None
                               IncludedEmployees = None
                               ExcludedOrgUnits = None
                               ExcludedEmployees = None }
        DiscountRate = None
        DateCreated = DateTime.UtcNow
        DateExpired = DateOnly.MaxValue
        }
        
    let equityPlanTemplateState (givenHistory : Event list) =
        givenHistory
        |> List.fold evolve emptyEquityPlanTemplate

    let equityPlanTemplate : Projection<EquityPlanTemplate, Event> =
      {
        Init = emptyEquityPlanTemplate
        Update = evolve
      }
      
    let (|ItemAlreadyExistsInVestingSchedule|_|) vestingSchedule vestingScheduleItem =
      match vestingSchedule |> List.tryFind (fun sch -> sch.Date = vestingScheduleItem.Date) with
      | Some _ -> Some vestingScheduleItem
      | _ -> None
      
    let handleCreateEquityPlanTemplate equityPlanTemplate history =
      if history |> List.isEmpty then
        [EquityPlanTemplateCreated equityPlanTemplate]
      else
        [EquityPlanTemplateAlreadyCreated |> DomainError]
  
    let addNewItemToVestingSchedule vestingScheduleItem equityPlanTemplate =
      match vestingScheduleItem with
      | ItemAlreadyExistsInVestingSchedule equityPlanTemplate.VestingSchedule _ ->
          [VestingScheduleItemAlreadyAddedInEquityPlanTemplate vestingScheduleItem |> DomainError]

      | _ -> [ItemAddedToVestingSchedule vestingScheduleItem]
      
    let private handleAddNewItemToVestingSchedule vestingScheduleItem history =
      history
      |> equityPlanTemplateState
      |> addNewItemToVestingSchedule vestingScheduleItem
        
    let behaviour (command : Command) : EventProducer<Event> =
      match command with
      | CreateEquityPlanTemplate equityPlanTemplate ->
          handleCreateEquityPlanTemplate equityPlanTemplate
       
      | AddItemToVestingSchedule vestingScheduleItem ->
           handleAddNewItemToVestingSchedule vestingScheduleItem
           
      | _ -> failwith "todo"