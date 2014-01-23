namespace TestConsoleApp.Models
{
    public class Movie
    {
        public string Title { get; set; }
        public string Director { get; set; }
        public int Year { get; set; }
        public string[] Genres { get; set; }
        public decimal? Rating { get; set; }
    }
}