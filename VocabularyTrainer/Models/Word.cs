
namespace VocabularyTrainer.Models
{
    public class Word
    {
        public int Id { get; set; }
        public string? FromWord { get; set; }
        public int FromLangId { get; set; }
        public string? ToWord { get; set; }
        public int ToLangId { get; set; }
        public int WordTypeId { get; set; }
        public int CategoryId { get; set; }
    }
}
