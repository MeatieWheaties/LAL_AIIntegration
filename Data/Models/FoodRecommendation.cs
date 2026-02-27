namespace LunchAndLearn_AIIntegration.Data.Models
{
    public class FoodRecommendation : Entity
    {
        public string Recommendation { get; set; }
    }

    public class FoodRecommendationComponents : Entity
    {
        public int RecommendationId { get; set; }
        public int ItemId { get; set; }
    }
}
