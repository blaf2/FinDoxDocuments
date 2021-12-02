using System.Collections.Generic;
using System.Data;
using static Dapper.SqlMapper;

namespace FinDoxDocumentsAPI.Repositories
{
    public class SnakeCaseDynamicParameters : List<object>, IDynamicParameters
    {
        public void AddParameters(IDbCommand command, Identity identity)
        {
            foreach (var parameter in this)
                command.Parameters.Add(parameter);
        }
    }
}
