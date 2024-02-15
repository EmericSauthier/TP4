using ApiFilm.Models.DataManager;
using ApiFilm.Models.EntityFramework;
using ApiFilm.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace ApiFilm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();

            // Add services to the container.
            builder.Services.AddDbContext<FilmRatingsDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("FilmRatingsDBContext")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddScoped<IDataRepository<Utilisateur>, UtilisateurManager>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}