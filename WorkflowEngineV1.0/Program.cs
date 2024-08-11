using Microsoft.EntityFrameworkCore;
using WorkflowEngineV1._0.Data;
using WorkflowEngineV1._0.Data.Repositories.Interfaces;
using WorkflowEngineV1._0.Data.Repositories;
using WorkflowEngineV1._0.Engine;
using WorkflowEngineV1._0.Handlers;
using WorkflowEngineV1._0.Services;

var builder = WebApplication.CreateBuilder(args);

// Logger
// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


// Add services to the container.
builder.Services.AddControllersWithViews();
// DB Config
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Initialize handlers and chain them
var startHandler = new StartTaskHandler();
var createDocHandler = new CreateDocTaskHandler();
var sendEmailHandler = new SendEmailTaskHandler();
var finishHandler = new FinishTaskHandler();

startHandler.SetNext(createDocHandler);
createDocHandler.SetNext(sendEmailHandler);
sendEmailHandler.SetNext(finishHandler);

builder.Services.AddSingleton(startHandler);
builder.Services.AddSingleton(createDocHandler);
builder.Services.AddSingleton(sendEmailHandler);
builder.Services.AddSingleton(finishHandler);

builder.Services.AddTransient<WorkflowEngine>(provider =>
{
    var startHandler = provider.GetRequiredService<StartTaskHandler>();
    var createDocHandler = provider.GetRequiredService<CreateDocTaskHandler>();
    var sendEmailHandler = provider.GetRequiredService<SendEmailTaskHandler>();
    var finishHandler = provider.GetRequiredService<FinishTaskHandler>();
    var context = provider.GetRequiredService<ApplicationDbContext>();
    var engine = new WorkflowEngine(context);
    engine.SetFirstHandler(startHandler);
    return engine;
});

// Add WorkflowServices to DI Container

builder.Services.AddTransient<WorkflowService>();
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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // Ensure this line is included



// Log a startup message
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started");


app.Run();
