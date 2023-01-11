namespace Equity.Api.CompositionRoot

open FSharp.Core
open Settings
open System.Threading.Tasks
open Equity.Domain
open Logic.EquityDomain
open Equity.Api.QueryHandlers
open Equity.Api.CommandHandlers
open Equity.PostgresDao

module Trunk = 
    type Trunk =
        {
            GenerateId: unit -> int64
            QueryEquityPlanBy: Queries.EquityPlanById.Query -> Task<Result<Queries.EquityPlanById.Result, string>>
            CreateEquityPlanItem: Commands.CreateEquityPlanCommand.Command -> Task<Result<PerformanceSharesTemplate, ValidationError list>>
        }
        
    let compose (settings: Settings) =
        let createDbConnection = DapperFSharp.createSqlConnection settings.SqlConnectionString
        let generateId = IdGenerator.create settings.IdGeneratorSettings
        {
            GenerateId = IdGenerator.create settings.IdGeneratorSettings
            QueryEquityPlanBy = EquityPlanQueryHandler.getById createDbConnection
            CreateEquityPlanItem = EquityPlanCommandHandler.create createDbConnection generateId
        }
