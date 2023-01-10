module Equity.PostgresDao

open System.Data
open Dapper
open Npgsql

module DapperFSharp = 
  let createSqlConnection (connectionString: string): unit -> Async<IDbConnection> =
    fun () -> async {
      let connection = new NpgsqlConnection(connectionString)
      if connection.State <> ConnectionState.Open
      then do! connection.OpenAsync() |> Async.AwaitTask
      return connection :> IDbConnection
    }