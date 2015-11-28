using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using StudyBuddy.DbModule.IDbHelpers;

namespace StudyBuddy.DbModule.DbHelpers
    {
    public class DbWrapper :  IDisposable, IDbWrapper
        {
        private bool _disposed;
        private readonly string _dbName;
        private IDbConnection Connection { get; set; }

        public DbWrapper(string dbName)
            {
            _dbName = dbName;
            _disposed = false;
            }

        public bool DoesDbExist()
            {
            return File.Exists(_dbName + ".sqlite");
            }

        public bool DoesTableExist(string tableName)
            {
            using (var sqLiteConnection = new SQLiteConnection ("DataSource=" + _dbName + ".sqlite;Version=3;"))
                {
                using (var sqLiteCommand = sqLiteConnection.CreateCommand())
                    {
                    sqLiteConnection.Open();
                    sqLiteCommand.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
                    var table = sqLiteCommand.ExecuteScalar();
                    return table != null && table.ToString() == tableName;
                    }
                }
            }

        public object ExecuteScalar(IDbCommand command)
            {
            using (IDbConnection sqLiteConnection = new SQLiteConnection("DataSource=" + _dbName + ".sqlite;Version=3;"))
                {
                command.Connection = sqLiteConnection;
                sqLiteConnection.Open();
                return command.ExecuteScalar();
                }
            }

        public bool ExecuteNonQuery(IDbCommand command)
            {
            using (var sqLiteConnection = new SQLiteConnection("DataSource=" + _dbName + ".sqlite;Version=3;"))
                {
                command.Connection = sqLiteConnection;
                sqLiteConnection.Open();
                command.ExecuteNonQuery();
                return true;
                }
            }

        public IDataReader ExecuteReader(IDbCommand command)
            {
            if (Connection == null)
                {
                Connection = new SQLiteConnection("DataSource=" + _dbName + ".sqlite;Version=3;");
                }
            Connection.Open ();
            command.Connection = Connection;
            return command.ExecuteReader();
            }

        public void Dispose()
            {
            Dispose (true);
            GC.SuppressFinalize (this);
            }

        protected virtual void Dispose(bool disposing)
            {
            if (_disposed)
                {
                return;
                }
            if (disposing)
                {
                if (Connection != null)
                    {
                    Connection.Dispose();
                    Connection = null;
                    }
                }
            _disposed = true;
            }
        }
    }