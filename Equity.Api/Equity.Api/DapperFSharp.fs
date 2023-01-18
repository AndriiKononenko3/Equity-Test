module Equity.PostgresDao

open System.Data
open System.Threading.Tasks
open Npgsql

module DapperFSharp =
    let createSqlConnection (connectionString: string) : unit -> Task<IDbConnection> =
        fun () ->
            task {
                let connection = new NpgsqlConnection(connectionString)

                if connection.State = ConnectionState.Open then
                    return connection :> IDbConnection
                else
                    let! _ = connection.OpenAsync()
                    return connection :> IDbConnection
            }