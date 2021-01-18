using GraphQL.Language.AST;
using PetizenApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetizenApi.Repositories
{
    public class CommonRepository : ICommonRepository
    {
        public CommonRepository()
        {

        }

        public List<string> GetFieldsName(IDictionary<string, Field> fields)
        {
            try
            {
                var fieldNames = new List<string>();
                fieldNames = fields.Where(t => t.Value.SelectionSet.Selections.Count == 0).Select(t => t.Key).ToList();

                foreach (var item in fields.Where(t => t.Value.SelectionSet.Selections.Count > 0))
                {
                    var fieldValue = item.Value.SelectionSet.Selections.OfType<Field>().Select(j => j.Name);

                    foreach (var value in fieldValue)
                    {
                        var fieldMerge = item.Key + "." + value;
                        fieldNames.Add(fieldMerge);

                    }
                }
                return fieldNames;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

    }
}
