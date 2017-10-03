namespace SecretSantaApp.Models
{
    public class StringModel
    {
        public StringModel()
        {
        }

        public StringModel(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}