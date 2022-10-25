
namespace VocabularyTrainer.Models
{
    public class Word
    {
        public int Id { get; set; }
        public string? EngWord { get; set; }
        public string? RusWord { get; set; }
        public int TypeId { get; set; }
        public int CategoryId { get; set; }
    }
}
