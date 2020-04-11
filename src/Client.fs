namespace Simiosoft.Clients.MySQL

open System.Data
open System.Threading
open FSharp.Control.Tasks
open MySql.Data.MySqlClient
open Dapper

type Client =
    { Connection: MySqlConnection 
      Transaction: IDbTransaction option
      CancellationToken: CancellationToken }
    with
    member private this.Trigger sql parameters fn =
        let cmdDef = 
            match this.Transaction with
            | Some t -> CommandDefinition(sql, parameters, t, cancellationToken = this.CancellationToken)
            | None -> CommandDefinition(sql, parameters, cancellationToken = this.CancellationToken)
        task { return! fn this.Connection cmdDef }
    member this.Execute sql parameters =
        this.Trigger sql parameters (fun conn -> conn.ExecuteAsync)
    member this.ExecuteScalar<'T> sql parameters =
        this.Trigger sql parameters (fun conn -> conn.ExecuteScalarAsync<'T>)
    member this.Query<'T> sql parameters =
        this.Trigger sql parameters (fun conn -> conn.QueryAsync<'T>)
    member this.QueryFirstOrDefault<'T> sql parameters =
        this.Trigger sql parameters (fun conn -> conn.QueryFirstOrDefaultAsync<'T>)
