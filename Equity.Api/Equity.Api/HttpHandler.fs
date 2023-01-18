namespace Api

open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.GoodRead
open HODI.DependencyInjection
open CompositionRoot
open Equity.Domain
open Logic.EquityDomain
open Equity.Api.QueryHandlers

module HttpHandler =
    let queryEquityPlanItemHandler queryEquityPlanById (id: int64)=
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! equityPlanItemResult = queryEquityPlanById (id |> Queries.EquityPlanById.Query)
                return! match equityPlanItemResult with
                        | Ok equityPlanItem -> json equityPlanItem next ctx
                        | Error error -> RequestErrors.badRequest (text error) next ctx
            }
            
    let createEquityPlanItemHandler (createEquityPlanItem: Commands.CreateEquityPlanCommand.Command -> Task<Result<PerformanceSharesTemplate, ValidationError list>>) =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! createEquityPlanItemCommandModel = ctx.BindJsonAsync<Commands.CreateEquityPlanCommand.Command>()
                let! equityPlanItemResult = createEquityPlanItem createEquityPlanItemCommandModel
                return! match equityPlanItemResult with
                        | Ok equityPlanItem -> Successful.created (text (equityPlanItem.EquityPlanId.ToString())) next ctx
                        | Error errors -> RequestErrors.badRequest (text (errors.Head.ToString())) next ctx
            }
            
    let router (compositionRoot: CompositionRoot) :HttpFunc -> HttpContext -> HttpFuncResult =
        choose [ GET >=> routef "/equityPlan/%d" (queryEquityPlanItemHandler compositionRoot.QueryEquityPlanBy)
                 GET >=> routef "/equityPlanV2/%d" (EquityPlanQueryV2Handler.getById >> Require.apply)
                 GET >=> routef "/equityPlanV3/%d" (EquityPlanQueryV3Handler.getById |> inject2Plus)
                 POST >=> route "/equityPlan" >=> (createEquityPlanItemHandler compositionRoot.CreateEquityPlanItem)
                 setStatusCode 404 >=> text "Not Found" ]