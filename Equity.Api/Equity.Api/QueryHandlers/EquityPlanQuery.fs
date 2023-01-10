module Equity.Api.QueryHandlers

open System
open System.Data
open System.Threading.Tasks
open FSharp.Core
open Equity.Domain
open FsToolkit.ErrorHandling

module EquityPlanQuery =
    
    let getById (createConnection: unit -> Async<IDbConnection>) (id: Queries.EquityPlanById.Query) : Task<Result<Queries.EquityPlanById.Result, string>> =
        taskResult {
            use! connection = createConnection ()
            let equityPlanId = Logic.EquityDomain.EquityPlanId (Guid.NewGuid())
            let! resultTemplate = Logic.EquityDomain.API.Query.performanceSharesTemplate equityPlanId
            return Logic.EquityDomain.PerformanceSharesTemplateMapping.fromEquityPlan resultTemplate
        }