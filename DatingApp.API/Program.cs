using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DatingApp.Datas;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api
{
    public class Program
    {
        private static readonly DbContextOptions<DataContext> _context;
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            using (var db = new DataContext(_context))
            {
                db.Database.Migrate();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
