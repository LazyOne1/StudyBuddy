using System.Data;

namespace StudyBuddy.DbModule.IDbHelpers
    {
    public interface IDbWrapper
        {
        bool DoesDbExist();
        bool DoesTableExist(string tableName);
        object ExecuteScalar(IDbCommand command);
        bool ExecuteNonQuery(IDbCommand command);
        IDataReader ExecuteReader(IDbCommand command);
        void Dispose();
        }
    }