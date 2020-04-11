namespace Simiosoft.Clients.MySQL

open MySql.Data.MySqlClient

module Connection =

    let create options =
        let builder =
            MySqlConnectionStringBuilder(
                Server = options.Host,
                Port = uint32 options.Port,
                Database = options.Database,
                ApplicationName = options.Application,
                MinimumPoolSize = uint32 (fst options.ConnectionPool),
                MaximumPoolSize = uint32 (snd options.ConnectionPool))
        new MySqlConnection(builder.ToString())
