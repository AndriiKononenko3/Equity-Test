namespace Equity.Api.CompositionRoot

open FSharp.Core
open Settings
open System.Threading.Tasks
open Equity.Api.QueryHandlers
open Equity.PostgresDao

module Trunk = 
    type Trunk =
        {
            QueryEquityPlanBy: Queries.EquityPlanById.Query -> Task<Result<Queries.EquityPlanById.Result, string>>
        }
        
    let compose (settings: Settings) =
        let createDbConnection = DapperFSharp.createSqlConnection settings.SqlConnectionString
        {
            QueryEquityPlanBy = EquityPlanQuery.getById createDbConnection
        }
