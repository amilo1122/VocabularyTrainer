using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabularyTrainer.Models
{
    public class NewWordView
    {
        public int FromLangId { get; set; }
        public string FromLang { get; set; }
        public string RusWord { get; set; }
        public WordTypes WordType { get; set; }
    }
}
