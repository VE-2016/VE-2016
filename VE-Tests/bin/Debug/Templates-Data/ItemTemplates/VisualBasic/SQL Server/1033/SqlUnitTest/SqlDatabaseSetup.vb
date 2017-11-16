Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data.Common
Imports Microsoft.Data.Tools.Schema.Sql.UnitTesting
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class SqlDatabaseSetup

    <AssemblyInitialize()>
    Public Shared Sub InitializeAssembly(ByVal ctx As TestContext)
        ' Setup the test database based on setting in the
        ' configuration file
        SqlDatabaseTestClass.TestService.DeployDatabaseProject()
        SqlDatabaseTestClass.TestService.GenerateData()
    End Sub

End Class
