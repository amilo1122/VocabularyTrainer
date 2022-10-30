using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabularyTrainer.Models
{
    public class LearningView
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int LangId { get; set; }
    }
}
