using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAuthServer.Service
{
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() => // () isimsiz metot anlamına gelir.
        {
            var configuration = new MapperConfiguration(config =>
            {
                config.AddProfile<DtoMapper>();
            });
            return configuration.CreateMapper();
        });
        public static IMapper Mapper => lazy.Value;
    }
}
