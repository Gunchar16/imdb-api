using AutoMapper;
using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.Repository;

namespace Api.Factories
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RegistrationDTO, User>();
        }
    }
}
