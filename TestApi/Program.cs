using TestApi.Models;

internal class Program
{
    private static readonly List<Car> Cars = new()
        {
            new Car{ Name = "Volvo", Color = "Black", YearManufactured = 2020},
            new Car{ Name = "Ford", Color = "Yellow", YearManufactured = 2000},
            new Car{ Name = "Lada", Color = "Silver", YearManufactured = 1990}
        };


    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
        app.UseRouting();

        app.UseEndpoints(endpoint =>
        {
            endpoint.Map("/Cars", _ => _.Response.WriteAsJsonAsync(Cars));
            endpoint.Map("/Cars/{name}", _ => _.Response.WriteAsJsonAsync(Cars.FirstOrDefault(x => x.Name.Equals(_.GetRouteValue("name")))));
        });

        app.MapControllers();
        

        app.Run();
    }
}


