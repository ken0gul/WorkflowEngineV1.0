using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WorkflowEngineV1._0.Models
{
    public class TaskItem
    {
        [JsonPropertyName("id")]
        [Key]
        [JsonIgnore]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }


        [JsonIgnore]
        public int? WorkflowId { get; set; } // Foreign Key to Workflow

        [JsonIgnore]
        public Workflow? Workflow { get; set; } // Navigation property to Workflow
    }
}
