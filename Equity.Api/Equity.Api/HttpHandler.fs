namespace Api

open Giraffe
open Microsoft.AspNetCore.Http
open CompositionRoot

module HttpHandler =
    let queryEquityPlanItemHandler queryEquityPlanById (id: int64): HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! equityPlanItem = queryEquityPlanById (id |> Queries.EquityPlanById.Query)
                return! match equityPlanItem with
                        | Ok stockItem -> json stockItem next ctx
                        | Error _ -> RequestErrors.notFound (text "Not Found") next ctx
            }
            
    let router (compositionRoot: CompositionRoot): HttpFunc -> HttpContext -> HttpFuncResult =
        choose [ GET >=> routef "/equityPlan/%d" (queryEquityPlanItemHandler compositionRoot.QueryEquityPlanBy)
                 setStatusCode 404 >=> text "Not Found" ]

