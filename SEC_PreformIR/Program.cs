using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SEC.PreformTempMonitor.Hubs;
using SEC_PreformIR.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
                // Maintain property names during serialization. See:
                // https://github.com/aspnet/Announcements/issues/194
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());
// Add Kendo UI services to the services container"
builder.Services.AddDbContext<EntityContext>();


builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(config =>
{
    //some swagger configuration code.

    //use fully qualified object names
    config.CustomSchemaIds(x => x.FullName);
    config.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwagger();

app.UseRouting();
app.UseAuthorization();

app.UseSwaggerUI(c =>
{
    var swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "My API");

});

app.MapHub<TemperatureHub>("/temperatureHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();