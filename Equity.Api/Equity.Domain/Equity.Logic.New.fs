module Equity.Domain.LogicNew

open NUnit.Framework
open FsUnit
open System
open Equity.Domain.Logic
open EquityDomain

type PlanFieldType = 
    | AllocationReason of AllocationReason 
    | TotalShares of decimal
    | Value of EquityValue
    | VestingScheduleList of VestingSchedule list
    | EligiblePopulation of EligiblePopulation
    | DiscountRate of decimal<percent> option
    
type PerformancePlan = {
    EquityPlanId : EquityPlanId 
    PlanName : PlanName
    DateCreated : DateTime
    Fields : PlanFieldType list
}

module PerformancePlan =
  let isVestingScheduleList =
      function
          | VestingScheduleList _ -> true
          | _ -> false
 
let internal (|IsVestingSchedule|_|) performancePlan =
  match performancePlan.Fields |> List.tryFind PerformancePlan.isVestingScheduleList with
  | Some sch -> Some sch
  | _ -> None
      
let evolvePerformancePlan performancePlan event :PerformancePlan =
  match event with
    | ItemAddedToVestingSchedule schedule ->
        match performancePlan with
        | IsVestingSchedule (VestingScheduleList schedules) ->
            { performancePlan with Fields = VestingScheduleList (schedule::schedules) :: (performancePlan.Fields |> List.filter (not << PerformancePlan.isVestingScheduleList)) }
        | _ ->
            { performancePlan with Fields = VestingScheduleList [schedule] :: performancePlan.Fields }
        
    | _ -> failwith "todo"
    
let checkPerformancePlan scheduleItem performancePlan =
    match performancePlan with
        | IsVestingSchedule (VestingScheduleList schedules) ->
            schedules.Head = scheduleItem
        | _ -> false

[<Test>]
let ``test add item to non-existing schedule`` () =
    // arrange
    let performancePlan : PerformancePlan =
      {
        EquityPlanId = Guid.Empty |> EquityPlanId
        PlanName = PlanName "Empty"
        DateCreated = DateTime.UtcNow
        Fields = []
      }
      
    let scheduleItem = {
        Date = DateOnly.MaxValue
        Type = VestingDateType.Vesting
        Amount = 100m
    }
    let event = ItemAddedToVestingSchedule scheduleItem
    
    // act
    let result = evolvePerformancePlan performancePlan event
    
    // assert
    result.Fields.Length |> should equal 1
    
    result.Fields
    |> List.find PerformancePlan.isVestingScheduleList
    |> function
        | VestingScheduleList schedules ->
            schedules.Length |> should equal 1
            schedules.Head |> should equal scheduleItem
            | _ -> failwith "missing vesting schedule"
        
[<Test>]
let ``test add item to existing schedule`` () =
    // arrange
    let performancePlan : PerformancePlan =
      {
        EquityPlanId = Guid.Empty |> EquityPlanId
        PlanName = PlanName "Empty"
        DateCreated = DateTime.UtcNow
        Fields = [
            VestingScheduleList [
                {
                    Date = DateOnly.MaxValue
                    Type = VestingDateType.Vesting
                    Amount = 999m
                }
            ]
        ]
      }
      
    let scheduleItem = {
        Date = DateOnly.MaxValue
        Type = VestingDateType.Vesting
        Amount = 100m
    }
    let event = ItemAddedToVestingSchedule scheduleItem
    
    // act
    let result = evolvePerformancePlan performancePlan event
    
    // assert
    result.Fields.Length |> should equal 1
    
    result.Fields
    |> List.find PerformancePlan.isVestingScheduleList
    |> function
        | VestingScheduleList schedules ->
            schedules.Length |> should equal 2
            schedules.Head |> should equal scheduleItem
            schedules.Tail.Head |> should equal {
                Date = DateOnly.MaxValue
                Type = VestingDateType.Vesting
                Amount = 999m
            }
        | _ -> failwith "missing vesting schedule"
