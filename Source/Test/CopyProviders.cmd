erase %2%1IQToolkit.Data.*.dll
CALL %2CopyProvider.cmd Access %1 %2
CALL %2CopyProvider.cmd SqlClient %1 %2
CALL %2CopyProvider.cmd SqlServerCe %1 %2
CALL %2CopyProvider.cmd MySqlClient %1 %2
CALL %2CopyProvider.cmd SQLite %1 %2