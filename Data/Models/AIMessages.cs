namespace LunchAndLearn_AIIntegration.Data.Models
{
    public class AI_KoboldAssessment : Entity
    {
        public int KoboldId { get; set; }
        public KoboldAssessment Assessment { get; set; } = KoboldAssessment.New;
    }

    public class Assessment : Entity
    {

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

        public string[] Items { get; set; }
    }

    public enum KoboldAssessment
    {
        New,
        Valid,
        Invalid
    }
}
