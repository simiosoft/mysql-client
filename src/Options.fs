namespace Simiosoft.Clients.MySQL

type MySQLOptions =
    { Host: string
      Port: int
      Database: string
      Application: string
      ConnectionPool: int * int }
