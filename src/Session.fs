namespace Simiosoft.Clients.MySQL

open System.Data
open System.Threading.Tasks
open FSharp.Control.Tasks

module Session =

    let create options fn ct =
        use connection = Connection.create options
        task {
            do! connection.OpenAsync()
            return! fn({ Connection = connection; Transaction = None; CancellationToken = ct })
        }

    let createTransactional<'T> options (fn: Client -> Task<'T>) ct =
        use connection = Connection.create options
        task {
            do! connection.OpenAsync()
            use! transaction = connection.BeginTransactionAsync()
            try
                let! result = fn({ Connection = connection; Transaction = Some (transaction :> IDbTransaction); CancellationToken = ct })
                do! transaction.CommitAsync();
                return result
            with
            | _ as e ->
                do! transaction.RollbackAsync()
                raise e
        }
