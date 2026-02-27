using System.ComponentModel.DataAnnotations.Schema;

namespace LunchAndLearn_AIIntegration.Data.Models
{
    public class AI_KoboldAssessment : Entity
    {
        public int KoboldId { get; set; }
        public KoboldAssessment Assessment { get; set; } = KoboldAssessment.New;
    }

    public class Assessment : Entity
    {
        public int KoboldId { get; set; }
        public string Response { get; set; } = string.Empty;
        public int ConfidenceLevel { get; set; }

        [NotMapped]
        public List<Item> Items { get; set; } = new List<Item>();
    }

    public class Item : Entity
    {
        public int AssessmentId { get; set; }
        public string Name { get; set; }
    }

    public struct Input
    {
        public string Context { get; set; }

        public string KoboldName { get; set; }
        public string KoboldMessage { get; set; }
    }

    public struct Output
    {
        public string Response { get; set; }
        public int ConfidenceLevel { get; set; }
        public string[] Items { get; set; }

        public const string SCHEMA = """
        {
          "type": "object",
          "additionalProperties": false,
          "properties": {
            "response": { "type": "string" },
            "items": {
              "type": "array",
              "items": { "type": "string" }
            },
            "confidenceLevel":{ "type": "integer"}
          },
          "required": ["response", "items", "confidenceLevel"]
        }
        """;
    }


    public enum KoboldAssessment
    {
        New,
        Valid,
        Invalid
    }
}
