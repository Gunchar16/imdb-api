namespace Infrastructure.Repositories.DTOs
{

    public class ApiPersonRootObject
    {
        public Result[] results { get; set; }
    }

    public class Result
    {
        public int id { get; set; }
        public Known_For[] known_for { get; set; }
        public string known_for_department { get; set; }
        public string name { get; set; }
    }

    public class Known_For
    {
        public int[] genre_ids { get; set; }
        public string media_type { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
    }



    public class Biography
    {
        public string biography { get; set; }
        public string birthday { get; set; }
        public string name { get; set; }
        public string known_for_department { get; set; }
    }

}


