namespace Equity.Api.QueryHandlers

open System
open FSharp.Core
open Microsoft.Extensions.Logging
open Giraffe
open Giraffe.GoodRead
open Equity.Domain
open Microsoft.Extensions.Options

module EquityPlanQueryV2Handler =
    
    let getById (id: int64) =
        require {
            let! logger = service<ILogger<Logic.EquityDomain.DomainError>> ()
            let! settings = service<IOptions<IdGenerator.Settings>>()

            return
                task {
                    // dummy code to show how to inject dependencies
                    let generateId = IdGenerator.create settings.Value
                    let id = generateId ()
                    logger.LogInformation($"Gets plan by {id}")
                    let equityPlanId = Logic.EquityDomain.EquityPlanId(Guid.NewGuid())
                    let! resultTemplate = Logic.EquityDomain.API.Query.performanceSharesTemplate equityPlanId

                    match resultTemplate with
                    | Ok template ->
                        let res = Logic.EquityDomain.PerformanceSharesTemplateMapping.fromEquityPlan id template
                        return json (Ok res)
                    | Error err ->
                        logger.LogError(err, "Error while getting all users")
                        return setStatusCode 500 >=> json (Error "Internal server error occured")
                }
        }
