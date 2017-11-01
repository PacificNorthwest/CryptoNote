using System;

namespace CryptoNote.Model
{
    /// <summary>
    /// Note data model
    /// </summary>
    [Serializable]
    public class Note
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; } = 0;

        public Note(string text)
        {
            Text = text;
            Date = DateTime.Now;
        }
    }
}