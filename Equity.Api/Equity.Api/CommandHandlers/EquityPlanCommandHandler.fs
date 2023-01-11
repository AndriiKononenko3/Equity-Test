namespace Equity.Api.CommandHandlers

open System
open System.Data
open System.Threading.Tasks
open FSharp.Core
open Equity.Domain
open Logic.EquityDomain
open FsToolkit.ErrorHandling

module EquityPlanCommandHandler =
    
    let private insert (createConnection: unit -> Async<IDbConnection>) (performanceTemplateItem: PerformanceSharesTemplate) =
        task {
            use! connection = createConnection ()
            return Result.Ok(performanceTemplateItem)
        }
        
    let create (createConnection: unit -> Async<IDbConnection>) (createId: unit -> int64) (dto:Commands.CreateEquityPlanCommand.Command) :Task<Result<PerformanceSharesTemplate, ValidationError list>> =
        taskResult {
            // dummy code to show how to inject dependencies
            let id = createId()
            let guidId = Guid.NewGuid()
            let! performanceTemplateItem = PerformanceSharesTemplateMapping.toEquityPlanDomain guidId dto.Name dto.AllocationReason dto.Amount dto.Currency
            return! insert createConnection performanceTemplateItem
        }