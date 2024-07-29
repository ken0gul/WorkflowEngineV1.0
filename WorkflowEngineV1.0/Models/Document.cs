using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WorkflowEngineV1._0.Models
{
    public class Document
    {

        [Key]
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public DateTime UpdatedDate
        {
            get; set;
        }

        public bool isPublished { get; set; }


        public int WorkflowId { get; set; }  // Add this property
        [JsonIgnore]
        public Workflow? Workflow { get; set; }
    }
}
