using GraphQL.Language.AST;
using System.Collections.Generic;

namespace PetizenApi.Interfaces
{
    public interface ICommonRepository
    {
        List<string> GetFieldsName(IDictionary<string, Field> fields);
    }
}
