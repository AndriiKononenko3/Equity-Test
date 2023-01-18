module Equity.Domain.Logic

open System
open System.Threading.Tasks
open EventSourced

module EquityDomain =
    
    type ValidationError =
    | PlanNameNotEmpty of string
    | EquityPlanIdNotEmpty of string
    | PlanTypeDoesNotExist of string
    | AllocationReasonDoesNotExist of string
    | DecimalValueExceedsLimit of string
    | NotValidFiatCurrency of string
        override this.ToString() =
            match this with
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

    module EquityPlanIdModule =
        let create id :Validation<EquityPlanId, ValidationError> = 
            if id = Guid.Empty then
                let msg = EquityPlanIdNotEmpty "EquityPlanId must not zero"
                Error [msg]
            else
                Ok (EquityPlanId id)
                
        let createUnsafe id = 
            EquityPlanId id
                
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
        | Empty
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
            | _ -> failwith "todo"

    [<Struct>]
    type EquityValue = EquityValue of amount:decimal * currency:Currency

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
    
    type VestingDateType =
        | Grant 
        | Vesting
        | Expiry
    
    type VestingSchedule = {
        Date : DateOnly
        Type: VestingDateType
        Amount : decimal
    }

    type PerformanceSharesTemplate = {
        EquityPlanId : EquityPlanId 
        PlanName : PlanName
        AllocationReason : AllocationReason
        TotalShares : decimal
        EquityValue : EquityValue
        VestingScheduleList : VestingSchedule list
        EligiblePopulation : EligiblePopulation
        DiscountRate : decimal<percent> option
        DateCreated : DateTime
        }
    
    type PhantomStocksTemplate = {
        EquityPlanId : EquityPlanId 
        PlanName : PlanName 
        AllocationReason : AllocationReason
        TotalShares : decimal
        EquityValue : EquityValue
        EligiblePopulation : EligiblePopulation
        DateCreated : DateTime
        }
    
    type EquityPlanTemplate =
    | PerformanceSharesPlanTemplate of PerformanceSharesTemplate
    | PhantomStocksPlanTemplate of PhantomStocksTemplate
    
    type DomainError =
    | EquityPlanTemplateAlreadyCreated
    | VestingScheduleItemAlreadyAddedInEquityPlanTemplate of VestingSchedule
    
    type Event =
    | PerformanceSharesTemplateCreated of PerformanceSharesTemplate
    | EligiblePopulationUpdated of EligiblePopulation
    | ItemAddedToVestingSchedule of VestingSchedule
    | ItemRemovedFromVestingSchedule of VestingSchedule
    | TotalSharesUpdated of decimal
    | EquityValueUpdated of EquityValue
    | PlanNameChanged of string
    | DraftManagersEquityPlanAssigned of DraftManagersEquityPlan
    | DraftEmployeesEquityPlanAssigned of DraftEmployeesEquityPlan
    | DomainErrorRaised of DomainError
        static member isAnError event =
            match event with
            | DomainErrorRaised _ -> true
            | _ -> false
          
        static member getError event =
            match event with
            | DomainErrorRaised error -> error
            | _ -> failwith "error"
      
    type Command =
    | CreatePerformanceSharesTemplate of PerformanceSharesTemplate
    | ChangePlanName of string
    | UpdateEligiblePopulation of EligiblePopulation
    | AddItemToVestingScheduleToPerformancePlan of VestingSchedule
    | RemoveItemToVestingSchedule of VestingSchedule
    | UpdateTotalShares of decimal
    | UpdateEquityValue of EquityValue
    | AssignDraftManagersEquityPlan of DraftManagersEquityPlan
    | AssignDraftEmployeesEquityPlan of DraftEmployeesEquityPlan
    
    let removeItemFromVestingSchedule (scheduleItem:VestingSchedule) (vestingSchedule:VestingSchedule list) =
        vestingSchedule |> List.filter (fun sch -> sch.Date <> scheduleItem.Date)
      
    let evolvePerformanceSharesTemplate performanceSharesTemplate event :PerformanceSharesTemplate =
      match event with
        | PerformanceSharesTemplateCreated equityPlanTemplate ->
            equityPlanTemplate
            
        | EligiblePopulationUpdated eligiblePopulation ->
            { performanceSharesTemplate with EligiblePopulation = eligiblePopulation }
            
        | ItemAddedToVestingSchedule schedule ->
            { performanceSharesTemplate with VestingScheduleList = schedule :: performanceSharesTemplate.VestingScheduleList }
            
        | ItemRemovedFromVestingSchedule schedule ->
            { performanceSharesTemplate with VestingScheduleList = removeItemFromVestingSchedule schedule performanceSharesTemplate.VestingScheduleList }
            
        | _ -> failwith "todo"
        
    let private emptyPerformanceSharesTemplate : PerformanceSharesTemplate =
      {
        EquityPlanId = Guid.Empty |> EquityPlanId
        PlanName = PlanName "Empty"
        AllocationReason = Empty
        TotalShares = 0m
        EquityValue = EquityValue (0m, FiatCurrency USD)
        VestingScheduleList = List.Empty
        EligiblePopulation = { IncludedOrgUnits = None
                               IncludedEmployees = None
                               ExcludedOrgUnits = None
                               ExcludedEmployees = None }
        DiscountRate = None
        DateCreated = DateTime.UtcNow
        }
        
    let performanceSharesTemplateState (givenHistory:Event list) =
        givenHistory
        |> List.fold evolvePerformanceSharesTemplate emptyPerformanceSharesTemplate

    let performanceSharesTemplate : Projection<PerformanceSharesTemplate, Event> =
      {
        Init = emptyPerformanceSharesTemplate
        Update = evolvePerformanceSharesTemplate
      }
      
    let internal (|ItemAlreadyExistsInVestingSchedule|_|) vestingSchedule vestingScheduleItem =
      match vestingSchedule |> List.tryFind (fun sch -> sch.Date = vestingScheduleItem.Date) with
      | Some _ -> Some vestingScheduleItem
      | _ -> None
      
    let internal addNewItemToVestingSchedule vestingScheduleItem schedules =
      match vestingScheduleItem with
      | ItemAlreadyExistsInVestingSchedule schedules item ->
          [VestingScheduleItemAlreadyAddedInEquityPlanTemplate item |> DomainErrorRaised]

      | _ -> [ItemAddedToVestingSchedule vestingScheduleItem]
      
    let private handleCreatePerformanceSharesTemplate equityPlanTemplate history =
      if history |> List.isEmpty then
        [PerformanceSharesTemplateCreated equityPlanTemplate]
      else
        [EquityPlanTemplateAlreadyCreated |> DomainErrorRaised]
      
    let private handleAddNewItemToVestingScheduleToPerformancePlan vestingScheduleItem history =
      let planTemplate = history |> performanceSharesTemplateState
      addNewItemToVestingSchedule vestingScheduleItem planTemplate.VestingScheduleList
        
    let performanceSharesTemplateBehaviour command :EventProducer<Event> =
      match command with
      | CreatePerformanceSharesTemplate equityPlanTemplate ->
          handleCreatePerformanceSharesTemplate equityPlanTemplate
       
      | AddItemToVestingScheduleToPerformancePlan vestingScheduleItem ->
           handleAddNewItemToVestingScheduleToPerformancePlan vestingScheduleItem
           
      | _ -> failwith "todo"
      
    module API =
        
        type PerformanceSharesTemplateCommandApi = {
           CreatePerformanceSharesTemplate : PerformanceSharesTemplate * EquityPlanId -> CommandEnvelope<Command>
           }
        
        module Command =
        
            let private envelope (EquityPlanId eventSource) command = {
                Transaction = TransactionId.New()
                EventSource = eventSource
                Command = command
              }
        
            let createPerformanceSharesTemplate = fun (performanceSharesTemplate, equityPlanId) -> envelope equityPlanId (CreatePerformanceSharesTemplate performanceSharesTemplate)
            
            // let performanceSharesTemplateCommandApi : PerformanceSharesTemplateCommandApi = {
            //     CreatePerformanceSharesTemplate = fun (performanceSharesTemplate, equityPlanId) -> envelope equityPlanId (CreatePerformanceSharesTemplate performanceSharesTemplate)
            //     }
    
        module Query =
            
            let private projectIntoMap projection =
              fun state eventEnvelope ->
                state
                |> Map.tryFind eventEnvelope.Metadata.Source
                |> Option.defaultValue projection.Init
                |> fun projectionState -> eventEnvelope.Event |> projection.Update projectionState
                |> fun newState -> state |> Map.add eventEnvelope.Metadata.Source newState
        
            let readModel (events:EventEnvelope<Event> list) =
                events
                |> List.fold (projectIntoMap performanceSharesTemplate) Map.empty
                
            let performanceSharesTemplate (EquityPlanId equityPlanId) =
              task {
                do! Async.Sleep 500
                
                let events = [
                    {
                      Event = PerformanceSharesTemplateCreated {
                        EquityPlanId = equityPlanId |> EquityPlanId
                        PlanName = PlanName "Performance Shares"
                        AllocationReason = Hire
                        TotalShares = 0m
                        EquityValue = EquityValue (500_000m, FiatCurrency USD)
                        VestingScheduleList = List.Empty
                        EligiblePopulation = { IncludedOrgUnits = None
                                               IncludedEmployees = None
                                               ExcludedOrgUnits = None
                                               ExcludedEmployees = None }
                        DiscountRate = None
                        DateCreated = DateTime.UtcNow
                        }
                      Metadata = {
                       Transaction = TransactionId.New()
                       Source = equityPlanId
                       RecordedAtUtc = DateTime.UtcNow
                       }
                    }]
                
                let state = readModel events

                return
                   match state |> Map.tryFind equityPlanId with
                    | Some performanceSharesTemplate ->
                        Ok performanceSharesTemplate

                    | None ->
                        Error "todo not found"
              }
  
    [<RequireQualifiedAccess>]
    module CommandHandler =
      
        let applyBehaviour (behaviour : Behaviour<_,_>) command  =
          behaviour command

        let sendCommand commandEnvelope : Task<Result<Event list, DomainError list>> =
            task {
                do! Async.Sleep 500
                
                let events = List.Empty
                //let events = [EquityPlanTemplateAlreadyCreated |> DomainErrorRaised]
                let result = applyBehaviour performanceSharesTemplateBehaviour commandEnvelope.Command events
                // todo append to stream
                
                return match result |> List.filter Event.isAnError with
                        | [] -> result |> Ok
                        | errors -> errors |> List.map Event.getError |> Error
            }
            
    type EquityPlanTemplateDto = {
        Id : Guid
        Name : string
        Type : string
        AllocationReason : string
        EquityValue : decimal
        EquityCurrency : string
        }

    module public PerformanceSharesTemplateMapping =
        
        open Queries.EquityPlanById
        
        let fromEquityPlan (id:int64)(domainObj:PerformanceSharesTemplate) :Queries.EquityPlanById.Result = 
            {
                Id = id
                Name = domainObj.PlanName |> PlanName.value
                Type = PerformanceShares |> PlanType.value
                AllocationReason = domainObj.AllocationReason |> AllocationReason.value
                EquityValue = domainObj.EquityValue |> EquityValue.amountValue
                EquityCurrency = domainObj.EquityValue |> EquityValue.currencyValue
            }
        
        let toEquityPlanDomain id name reason equityValue equityCurrency :Result<PerformanceSharesTemplate,ValidationError list> =
            result {
                let! equityPlanId = id |> EquityPlanIdModule.create
                let! planName = name |> PlanName.create
                let! allocationReason = reason |> AllocationReason.create (nameof reason)
                let! price = EquityValue.create equityValue equityCurrency
                
                let equityPlan = {
                    EquityPlanId = equityPlanId
                    PlanName = planName
                    AllocationReason = allocationReason
                    TotalShares = 0m
                    EquityValue = price
                    VestingScheduleList = List.Empty
                    EligiblePopulation = { IncludedOrgUnits = None
                                           IncludedEmployees = None
                                           ExcludedOrgUnits = None
                                           ExcludedEmployees = None }
                    DiscountRate = None
                    DateCreated = DateTime.UtcNow
                }
                return equityPlan
                }
            
        let toEquityPlan (dto:EquityPlanTemplateDto) :Result<PerformanceSharesTemplate,ValidationError list> =
            result {
                let! equityPlanId = dto.Id |> EquityPlanIdModule.create
                let! planName = dto.Name |> PlanName.create
                let! allocationReason = dto.AllocationReason |> AllocationReason.create (nameof dto.AllocationReason)
                let! price = EquityValue.create dto.EquityValue dto.EquityCurrency
                
                let equityPlan = {
                    EquityPlanId = equityPlanId
                    PlanName = planName
                    AllocationReason = allocationReason
                    TotalShares = 0m
                    EquityValue = price
                    VestingScheduleList = List.Empty
                    EligiblePopulation = { IncludedOrgUnits = None
                                           IncludedEmployees = None
                                           ExcludedOrgUnits = None
                                           ExcludedEmployees = None }
                    DiscountRate = None
                    DateCreated = DateTime.UtcNow
                }
                return equityPlan
                }
    
     module PerformanceSharesTemplateModule =
        let private createPerformanceSharesTemplate id name planType reason equityValue =
            {
                EquityPlanId = id;
                PlanName = name;
                AllocationReason = reason
                EquityValue = equityValue
                TotalShares = 0m
                VestingScheduleList = List.Empty
                EligiblePopulation = { IncludedOrgUnits = None
                                       IncludedEmployees = None
                                       ExcludedOrgUnits = None
                                       ExcludedEmployees = None }
                DiscountRate = None
                DateCreated = DateTime.UtcNow
            }
        
        let (<!>) = Validation.map
        let (<*>) = Validation.apply

        let createPerformanceSharesTemplateValidated id name planType reason equityAmount equityCurrency :Validation<PerformanceSharesTemplate, ValidationError> =
            let equityPlanId = id |> EquityPlanIdModule.create
            let planName = name |> PlanName.create
            let planType = planType |> PlanType.create (nameof planType)
            let allocationReason = reason |> AllocationReason.create (nameof reason)
            let equityValue = EquityValue.create equityAmount equityCurrency
            createPerformanceSharesTemplate <!> equityPlanId <*> planName <*> planType <*> allocationReason <*> equityValue