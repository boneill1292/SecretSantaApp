using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
    public class StringModel
    {
        public StringModel() { }
        public StringModel(string text)
        {
            Text = text;
        }

        public string Text { get; set; }

    }
}
