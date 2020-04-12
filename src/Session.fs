namespace Simiosoft.Clients.MySQL

open System.Data
open System.Threading.Tasks
open FSharp.Control.Tasks
open Simiosoft.Prelude

module Session =

    let create<'T> options (fn: Client -> Task<'T>) ct =
        use connection = Connection.create options
        task {
            do! connection.OpenAsync()
            try
                let! result = fn({ Connection = connection; Transaction = None; CancellationToken = ct }) 
                return succeed result
            with
            | _ as e ->
                return fail e
        }

    let createTransactional<'T> options (fn: Client -> Task<'T>) ct =
        use connection = Connection.create options
        task {
            do! connection.OpenAsync()
            use! transaction = connection.BeginTransactionAsync()
            try
                let! result = fn({ Connection = connection; Transaction = Some (transaction :> IDbTransaction); CancellationToken = ct })
                do! transaction.CommitAsync()
                return succeed result
            with
            | _ as e ->
                do! transaction.RollbackAsync()
                return fail e
        }
