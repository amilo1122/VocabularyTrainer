using System.Text.Json;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.Menus
{
    public class LearningMenu
    {
        List<Menu> learningMenu = new List<Menu> {
            new Menu { Name = "Choose category", Callback = "chooseCategory" },
            new Menu { Name = "Upload progress", Callback = "uploadProgress" },
        };

        public string? GetLearningMenu()
        {
            return JsonSerializer.Serialize(learningMenu);
        }
    }
}
