using System.Data;

namespace FinDoxDocumentsAPI.Repositories
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
        IDbConnection GetDocumentConnection();
    }
}
