namespace Equity.Api.QueryHandlers

open System
open System.Data
open System.Threading.Tasks
open FSharp.Core
open Equity.Domain
open FsToolkit.ErrorHandling

module EquityPlanQueryHandler =
    
    let getById (createConnection: unit -> Async<IDbConnection>) (id: Queries.EquityPlanById.Query) :Task<Result<Queries.EquityPlanById.Result, string>> =
        taskResult {
            // dummy code to show how to inject dependencies
            use! connection = createConnection ()
            let equityPlanId = Logic.EquityDomain.EquityPlanId (Guid.NewGuid())
            let! resultTemplate = Logic.EquityDomain.API.Query.performanceSharesTemplate equityPlanId
            return Logic.EquityDomain.PerformanceSharesTemplateMapping.fromEquityPlan resultTemplate
        }