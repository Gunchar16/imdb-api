namespace Infrastructure.Repositories.DTOs
{

    public class ApiGenreRootObject
    {
        public ApiGenre[] genres { get; set; }
    }

    public class ApiGenre
    {
        public int id { get; set; }
        public string name { get; set; }
    }

}
