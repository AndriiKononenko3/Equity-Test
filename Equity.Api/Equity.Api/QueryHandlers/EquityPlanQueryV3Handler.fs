namespace Equity.Api.QueryHandlers

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options
open Giraffe
open Equity.Domain

module EquityPlanQueryV3Handler =
    let getById =
        fun
            (id: int64)
            (logger: ILogger<Logic.EquityDomain.DomainError>)
            (settings: IOptions<IdGenerator.Settings>)
            (next: HttpContext -> Task<HttpContext option>)
            (ctx: HttpContext) ->
            task {
                // dummy code to show how to inject dependencies
                let generateId = IdGenerator.create settings.Value
                let generatedId = generateId ()
                logger.LogInformation($"Gets plan by {generatedId}")
                let equityPlanId = Logic.EquityDomain.EquityPlanId(Guid.NewGuid())
                let! resultTemplate = Logic.EquityDomain.API.Query.performanceSharesTemplate equityPlanId

                match resultTemplate with
                | Ok template ->
                    let res = Logic.EquityDomain.PerformanceSharesTemplateMapping.fromEquityPlan generatedId template
                    return! json res next ctx
                | Error err ->
                    logger.LogError(err, "Error while getting all users")
                    return! RequestErrors.badRequest (text err) next ctx
            }
