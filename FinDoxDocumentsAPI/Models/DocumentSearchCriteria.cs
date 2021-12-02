using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinDoxDocumentsAPI.Models
{
    public class DocumentSearchCriteria : ICanConvertToDbModel
    {
        public string DocumentName { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DocumentCategories Category { get; set; }

        public object GetDbModel()
        {
            return new
            {
                _document_name = DocumentName,
                _description = Description,
                _category = Category
            };
        }
    }
}
