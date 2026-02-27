using System.ComponentModel.DataAnnotations.Schema;

namespace LunchAndLearn_AIIntegration.Data.Models
{
    public class AIConfiguration : Entity
    {
        public string Objective { get; set; } = string.Empty;

        [NotMapped]
        public List<AIConfigurationContext> Context { get; set; } = new List<AIConfigurationContext>();

        public string RenderContext()
        {
            var _result = Objective;

            foreach(var _context in Context)
            {
                _result += $"\n{_context}";
            }

            return _result;
        }
    }

    public class AIConfigurationContext : Entity
    {
        public string Value { get; set; }
    }
}
