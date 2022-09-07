using Imdb.Infrastructure;
using Imdb.Infrastructure.Entities;
using Infrastructure.Repositories.DTOs;
using Infrastructure.Repositories.Entities;
using RestSharp;
using Newtonsoft.Json;
using System.Net;
using Shared;

namespace Infrastructure.DBInitializer
{
    public class Initializer
    {
        //Will be using later for wwwroot.
        private readonly IWebHostEnvironment _appEnvironment;
        public Initializer(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }
        //Seed populates the database with pre-written films. Will be changed later.
        public static async void Seed(IApplicationBuilder applicationBuilder)
        {            
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();

            var client = new RestClient("https://api.themoviedb.org/3/");

            

            if (!context.Role.Any())
            {
                context.Role.AddRange(
                    new Role
                    {
                        Name = "User",
                        Tier = 0

                    },
                    new Role
                    {
                        Name = "Admin",
                        Tier = 1
                    }
                    );

            }
            await context.SaveChangesAsync();
            var adminid = context.Set<Role>().Where(x => x.Name == "Admin").FirstOrDefault();
            var userid = context.Set<Role>().Where(x => x.Name == "User").FirstOrDefault();
            if (!context.User.Any())
            {
                context.User.AddRange(new User
                {
                    FirstName = "Benjamin",
                    LastName = "Reichwald",
                    UserName = "bladee",
                    Age = 28,
                    Email = "g.shengelaia@outlook.com",
                    Password = "DqoD6e9IaZm6aFmFltl97YMwBGe74PAU07fMYtxtoYc=", // ecco2k
                    Salt = "4huK41sHioo0FZr3oaGvsw==",
                    RoleId = adminid.Id
                },
                new User
                {
                    FirstName = "Giorgi",
                    LastName = "Ioseliani",
                    UserName = "ios",
                    Age = 20,
                    Email = "gshengelaia1104@sdsu.edu",
                    Password = "DqoD6e9IaZm6aFmFltl97YMwBGe74PAU07fMYtxtoYc=", // ecco2k
                    Salt = "4huK41sHioo0FZr3oaGvsw==",
                    RoleId = userid.Id
                });
            }
            if(!context.FilmCategory.Any())
            {
                var request = new RestRequest("genre/movie/list").AddParameter("api_key", "7f4b023273caea7781a4109c1599a45c");
                var response = await client.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new AppException("Could not connect to TMDB");
                var obj = JsonConvert.DeserializeObject<ApiGenreRootObject>(response.Content);
                foreach(var item in obj.genres)
                {
                    context.FilmCategory.Add(new Genre
                    {
                        TmdbId = item.id,
                        Name = item.name
                    });
                }
                await context.SaveChangesAsync();
            }


            if (!context.Films.Any())
            {
                var request = new RestRequest("movie/popular").AddParameter("api_key", "7f4b023273caea7781a4109c1599a45c");
                for (int i = 1; i < 11; i++)
                {
                    var response = await client.ExecuteAsync(request.AddParameter("page", i.ToString()));
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new AppException("Could not connect to TMDB");
                    var obj = JsonConvert.DeserializeObject<ApiMovieRootObject>(response.Content);
                    foreach (var item in obj.results)
                    {
                        var tmpFilm = new Film
                        {
                            Name = item.original_title,
                            Description = item.overview,
                            Rating = 0.0,
                            ReleaseYear = int.Parse(item.release_date == string.Empty ? "0" : item.release_date.Substring(0, 4)),
                            ImageURL = $"https://www.themoviedb.org/t/p/w600_and_h900_bestv2{item.poster_path}"
                        };
                        context.Films.Add(tmpFilm);
                        for(int j = 0; j < item.genre_ids.Length; j++)
                        {
                            context.Film_FilmCategory.Add(new Film_Genre
                            {
                                Film = tmpFilm,
                                Genre = context.FilmCategory.FirstOrDefault(x => x.TmdbId == item.genre_ids[j])
                            }) ;
                        }
                    }
                }

                await context.SaveChangesAsync();
            }

            if (!context.Person.Any())
            {
                var request = new RestRequest("person/popular").AddParameter("api_key", "7f4b023273caea7781a4109c1599a45c");
                for(int i = 1; i < 11; i++)
                {
                    var response = await client.ExecuteAsync(request.AddParameter("page", i.ToString()));
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new AppException("Could not connect to TMDB");
                    var obj = JsonConvert.DeserializeObject<ApiPersonRootObject>(response.Content);
                    foreach(var item in obj.results)
                    {
                        var detailsRequest = new RestRequest($"person/{item.id}")
                            .AddParameter("api_key", "7f4b023273caea7781a4109c1599a45c");
                        var detailsResponse = await client.ExecuteAsync(detailsRequest);
                        if (detailsResponse.StatusCode != HttpStatusCode.OK)
                            throw new AppException("Could not connect to TMDB");
                        var details = JsonConvert.DeserializeObject<Biography>(detailsResponse.Content);
                        var tmpPerson = new Person
                        {
                            Name = details.name,
                            Bio = details.biography,
                            Age = details.birthday == null ? 0 : new DateTime(DateTime.Now.Subtract(DateTime.Parse(details.birthday)).Ticks).Year - 1
                        };
                        context.Person.Add(tmpPerson);
                        if (!context.PersonCategory.Where(x => x.Name == details.known_for_department).Any())
                        {
                            context.PersonCategory.Add(new Imdb.Infrastructure.Entities.Type
                            {
                                Name = details.known_for_department
                            });
                            await context.SaveChangesAsync();
                        }
                        context.Person_PersonCategory.Add(new Person_Type
                        {
                            Person = tmpPerson,
                            TypeId = context.PersonCategory.Where(x => x.Name == details.known_for_department).Single().Id
                        });
                        foreach (var knownFor in item.known_for)
                        {
                            if (knownFor.media_type != "movie")
                                continue;
                            if (!context.Films.Where(x => x.Name == knownFor.original_title).Any())
                            {
                                var tmpFilm = new Film
                                {
                                    Name = knownFor.original_title,
                                    Description = knownFor.overview,
                                    Rating = 0.0,
                                    ReleaseYear = int.Parse(knownFor.release_date == string.Empty ? "0" : knownFor.release_date.Substring(0, 4)),
                                    ImageURL = $"https://www.themoviedb.org/t/p/w600_and_h900_bestv2{knownFor.poster_path}"
                                };
                                context.Films.Add(tmpFilm);
                                for (int j = 0; j < knownFor.genre_ids.Length; j++)
                                {
                                    context.Film_FilmCategory.Add(new Film_Genre
                                    {
                                        Film = tmpFilm,
                                        Genre = context.FilmCategory.FirstOrDefault(x => x.TmdbId == knownFor.genre_ids[j])
                                    }) ;
                                }
                                await context.SaveChangesAsync();
                            }
                            var filmId = context.Films.Where(x => x.Name == knownFor.original_title).Any();
                            context.Person_Film.Add(new Person_Film
                            {
                                Person = tmpPerson,
                                Film = context.Films.FirstOrDefault(x => x.Name == knownFor.original_title)
                            });
                        }
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
