module CompositionRoot

open System.Threading.Tasks
open FSharp.Core
open Equity.Api.CompositionRoot

type CompositionRoot =
    {
      QueryEquityPlanBy: Queries.EquityPlanById.Query -> Task<Result<Queries.EquityPlanById.Result, string>>
    }
    
let compose (trunk: Trunk.Trunk) =
    {
      QueryEquityPlanBy = trunk.QueryEquityPlanBy
    }