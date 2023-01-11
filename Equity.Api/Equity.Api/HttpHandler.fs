namespace Api

open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open CompositionRoot
open Equity.Domain
open Logic.EquityDomain

module HttpHandler =
    let queryEquityPlanItemHandler queryEquityPlanById (id: int64): HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! equityPlanItemResult = queryEquityPlanById (id |> Queries.EquityPlanById.Query)
                return! match equityPlanItemResult with
                        | Ok equityPlanItem -> json equityPlanItem next ctx
                        | Error error -> RequestErrors.badRequest (text error) next ctx
            }
            
    let createEquityPlanItemHandler (createEquityPlanItem: Commands.CreateEquityPlanCommand.Command -> Task<Result<PerformanceSharesTemplate, ValidationError list>>) :HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! createEquityPlanItemCommandModel = ctx.BindJsonAsync<Commands.CreateEquityPlanCommand.Command>()
                let! equityPlanItemResult = createEquityPlanItem createEquityPlanItemCommandModel
                return! match equityPlanItemResult with
                        | Ok equityPlanItem -> Successful.created (text (equityPlanItem.EquityPlanId.ToString())) next ctx
                        | Error errors -> RequestErrors.badRequest (text (errors.Head.ToString())) next ctx
            }
            
    let router (compositionRoot: CompositionRoot): HttpFunc -> HttpContext -> HttpFuncResult =
        choose [ GET >=> routef "/equityPlan/%d" (queryEquityPlanItemHandler compositionRoot.QueryEquityPlanBy)
                 POST >=> route "/equityPlan" >=> (createEquityPlanItemHandler compositionRoot.CreateEquityPlanItem)
                 setStatusCode 404 >=> text "Not Found" ]

