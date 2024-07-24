using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WorkflowEngineV1._0.Models
{
    public class Connection
    {

        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        [JsonPropertyName("startTaskId")]
        public string StartTaskId { get; set; }

        [JsonPropertyName("endTaskId")]
        public string EndTaskId { get; set; }

        [JsonPropertyName("xLoc")]
        public float XLoc { get; set; }

        [JsonPropertyName("yLoc")]
        public float YLoc { get; set; }

        [JsonIgnore]
        public int WorkflowId { get; set; } // Foreign Key to Workflow
        [JsonIgnore]

        public Workflow? Workflow { get; set; } // Navigation property to Workflow

   
    }
}
