using AutoMapper;
using System.Reflection;

namespace ECommerceAPI.Tests.Common
{
    public class TestBase
    {
        protected IMapper Mapper { get; }

        public TestBase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Automatically loads all mapping profiles from ECommerceAPI
                cfg.AddMaps(Assembly.Load("ECommerceAPI"));
            });

            Mapper = config.CreateMapper();
        }
    }
}