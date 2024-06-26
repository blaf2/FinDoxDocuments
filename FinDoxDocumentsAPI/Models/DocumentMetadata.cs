﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace FinDoxDocumentsAPI.Models
{
    public class DocumentMetadata
    {
        public int DocumentId { get; set; }
        public DateTime UploadTimestamp { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DocumentCategories Category { get; set; }
    }
}
