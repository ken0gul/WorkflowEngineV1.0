using System.Text.Json.Serialization;

namespace WorkflowEngineV1._0.Models
{
    public class Workflow
    {
        [JsonIgnore]

        public int Id { get; set; }


        [JsonPropertyName("workflowName")]
        public string WorkflowName { get; set; }


        [JsonPropertyName("tasks")]
        public List<TaskItem> Tasks { get; set; }

        [JsonPropertyName("connections")]
        public List<Connection> Connections { get; set; }

        public TaskState State { get; set; }

        [JsonIgnore]

        public Guid? DocumentId { get; set; }
        [JsonIgnore]
        public Document? Document { get; set; }
    }
}
