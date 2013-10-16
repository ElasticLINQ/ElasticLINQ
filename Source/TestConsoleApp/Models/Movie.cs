namespace TestConsoleApp.Models
{
    public class Movie
    {
        public string Title;
        public string Director;
        public int Year;
        public bool Awesome;

        public override string ToString()
        {
            return string.Format("{0} ({1}), {2}", Title, Year, Director);
        }
    }
}