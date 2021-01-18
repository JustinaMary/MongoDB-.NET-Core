namespace PetizenApi.Models
{
    public class FileClass
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string Path { get; set; }
    }

    public class DynamicFields
    {
        public string name { get; set; }

        public string alias { get; set; }

    }
}
