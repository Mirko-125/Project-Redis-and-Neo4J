using Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Services;
using ServiceStack.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCors(options => 
            {
                options.AddPolicy("CORSAll", policy => 
                {
                        policy.WithOrigins(new string[]
                        {
                            "https://localhost:3000",
                            "http://localhost:3000",
                            "http://127.0.0.1:3000",
                            "https://127.0.0.1:3000",
                            "http://localhost:5500",
                            "https://localhost:5500",
                            "http://127.0.0.1:5500",
                            "https://127.0.0.1:5500",
                            "http://localhost:5041",
                            "https://localhost:5041",
                            "http://127.0.0.1:5041",
                            "https://127.0.0.1:5041",
                            "https://localhost:5236",
                            "http://localhost:5236",
                            "http://127.0.0.1:5236",
                            "https://127.0.0.1:5236"
                        }).AllowAnyHeader()
                        .AllowAnyMethod();
                });

                options.AddPolicy("CORSUser", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyOrigin().WithMethods("POST", "PUT", "GET");
                });
            });
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<AbilityService>();
builder.Services.AddScoped<AchievementService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<MarketplaceService>();
builder.Services.AddScoped<MonsterService>();
builder.Services.AddScoped<MonsterBattleService>();
builder.Services.AddScoped<PlayerFightService>();
builder.Services.AddScoped<NPCService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<ConsumableService>();
builder.Services.AddScoped<GearService>();
builder.Services.AddScoped<TradeService>();
builder.Services.AddSingleton<IRedisClientsManager>(c => 
    new PooledRedisClientManager("localhost:6379"));

builder.Services.AddSingleton<RedisCache>(serviceProvider =>
{
    var redisClientsManager = serviceProvider.GetRequiredService<IRedisClientsManager>();
    return RedisCache.Initialize(redisClientsManager);
});

// Configure the IDriver interface
var driver = GraphDatabase.Driver("neo4j://localhost:7687", AuthTokens.Basic("neo4j", "myLocalPassword125"));

// Register the IDriver instance for dependency injection
builder.Services.AddSingleton<IDriver>(driver);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CORSAll");

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.Run();