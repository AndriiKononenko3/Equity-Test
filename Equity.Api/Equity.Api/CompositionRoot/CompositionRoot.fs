module CompositionRoot

open System.Threading.Tasks
open FSharp.Core
open Equity.Domain
open Logic.EquityDomain
open Equity.Api.CompositionRoot

type CompositionRoot =
    {
      GenerateId: unit -> int64
      QueryEquityPlanBy: Queries.EquityPlanById.Query -> Task<Result<Queries.EquityPlanById.Result, string>>
      CreateEquityPlanItem: Commands.CreateEquityPlanCommand.Command -> Task<Result<PerformanceSharesTemplate, ValidationError list>>
    }
    
let compose (trunk: Trunk.Trunk) =
    {
      GenerateId = trunk.GenerateId
      QueryEquityPlanBy = trunk.QueryEquityPlanBy
      CreateEquityPlanItem = trunk.CreateEquityPlanItem
    }