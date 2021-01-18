using GraphQL.Types;
using PetizenApi.Models;


namespace PetizenApi.Types
{
    public class FileGraphType : ObjectGraphType<FileClass>
    {
        public FileGraphType()
        {
            Field(f => f.Id);
            Field(f => f.Name);
            Field(f => f.MimeType);
            Field(f => f.Path);
        }
    }
}
