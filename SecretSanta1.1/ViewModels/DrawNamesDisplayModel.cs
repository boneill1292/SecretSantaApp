using System.Collections.Generic;
using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class DrawNamesDisplayModel
    {
        public Group Group { get; set; }
        public bool Saved { get; set; }
        public string TestStr { get; set; }

        public List<DrawNamesEditModel> DrawNamesList { get; set; }
    }
}