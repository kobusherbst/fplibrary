using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SqlServerCe;

namespace FPLibrary.DataAccessLayer
{
  public static class GenericFactoryHelper
  {
    public static DataTable AllFactories
    {
      get { return DbProviderFactories.GetFactoryClasses(); }
    }
    public static DbProviderFactory GetFactory(string providerName)
    {
      return DbProviderFactories.GetFactory(providerName);
    }
    public static DbConnection GetConnection(string providerName)
    {
      return GenericFactoryHelper.GetFactory(providerName).CreateConnection();
    }
    public static DbConnectionStringBuilder ConnectionStringBuilder(string providerName)
    {
      return GenericFactoryHelper.GetFactory(providerName).CreateConnectionStringBuilder();
    }
    public static string GetConnectionStringDialog()
    {
      ADODB.Connection adoCon = new ADODB.Connection();
      object connection = (object)adoCon;
      MSDASC.DataLinks dlg = new MSDASC.DataLinks();
      if (dlg.PromptEdit(ref connection))
      {
        return adoCon.ConnectionString;
      }
      else
      {
        return String.Empty;
      }
    }
    public static DbConnection GetAccessConnection(string dbPath, string dbPassword)
    {
      DbConnection connection = GetConnection("System.Data.OleDb");
      DbConnectionStringBuilder dbcb = ConnectionStringBuilder("System.Data.OleDb");
      dbcb.Add("Provider", "Microsoft.ACE.OLEDB.12.0");
      dbcb.Add("Data Source", dbPath);
      if (!String.IsNullOrEmpty(dbPassword))
      {
        dbcb.Add("Jet OLEDB:Database Password", dbPassword);
      }
      connection.ConnectionString = dbcb.ConnectionString;
      return connection;
    }
    public static DbConnection GetAccessOleConnection(string dbPath, string dbPassword)
    {
      DbConnection connection = GetConnection("System.Data.OleDb");
      DbConnectionStringBuilder dbcb = ConnectionStringBuilder("System.Data.OleDb");
      dbcb.Add("Provider", "Microsoft.Jet.OLEDB.4.0");
      dbcb.Add("Data Source", dbPath);
      if (!String.IsNullOrEmpty(dbPassword))
      {
        dbcb.Add("Jet OLEDB:Database Password", dbPassword);
      }
      connection.ConnectionString = dbcb.ConnectionString;
      return connection;
    }
    public static DbConnection GetSQLServerConnection(string server, string database)
    {
      SqlConnectionStringBuilder dbcb = new SqlConnectionStringBuilder { DataSource = server, InitialCatalog = database, IntegratedSecurity = true };
      SqlConnection connection = new SqlConnection(dbcb.ConnectionString);
      return connection;
    }
    public static DbConnection GetSQLServerConnection(string server, string database, string user, string password)
    {
      SqlConnectionStringBuilder dbcb = new SqlConnectionStringBuilder { DataSource = server, InitialCatalog = database, UserID = user, Password = password };
      SqlConnection connection = new SqlConnection(dbcb.ConnectionString);
      return connection;
    }
    public static DbConnection GetSQLCeConnection(string dbPath, string password)
    {
      SqlCeConnection connection = new SqlCeConnection(String.Format("Data Source = {0}; Password = '{1}'",dbPath,password));
      return connection;
    }
  }
}
