namespace FinDoxDocumentsAPI.Models
{
    public class DocumentContent : ICanConvertToDbModel
    {
        public int Id { get; set; }
        public int MetadataId { get; set; }
        public byte[] Content { get; set; }

        public object GetDbModel()
        {
            return new
            {
                metadata_id = MetadataId,
                content = Content
            };
        }
    }
}
