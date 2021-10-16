using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;
using System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/api/menu", async () =>
{
    return await ReadMenu("pizzaconfigurations.json");

    static async Task<JsonObject> ReadMenu(string fileName)
    {
        using (var jsonFileReader = File.OpenText("pizzaconfigurations.json"))
        {
            var file = await jsonFileReader.ReadToEndAsync();
            return JsonNode.Parse(file)!.AsObject();
        }
    }
});

app.MapPost("/api/order", async ([FromBody] Order order) =>
{
    var orders = await GetOrders();
    if (orders.Length == 0) 
    {
        orders = new Order[] {order};
    }
    else
    {
        var allOrders = orders.ToList();
        allOrders.Add(order);
        orders = allOrders.ToArray();
    }

    using (var outputStream = File.OpenWrite("orders.json"))
    {
        JsonSerializer.Serialize<Order[]>(
            new Utf8JsonWriter(outputStream, new JsonWriterOptions
            {
                SkipValidation = true,
                Indented = true
            }),
            orders
        );
    }

    static async Task<Order[]> GetOrders()
    {
        using (var jsonFileReader = File.OpenText("orders.json"))
        {
            string content = await jsonFileReader.ReadToEndAsync();

            return JsonSerializer.Deserialize<Order[]>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
        }
    } 
});

app.Run();

public class Order 
{
    [JsonPropertyName("pizzas")]
    public List<Pizza> Pizzas { get; set; } = new List<Pizza>();
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; }

    public Order(Guid id, string customerName)
    {
        Id = id;
        CustomerName = customerName;
    }
}

public class Pizza
{
    [JsonPropertyName("size")]
    public string Size { get; set; }
    [JsonPropertyName("dough")]
    public string Dough { get; set; }
    [JsonPropertyName("topping")]
    public string[] Toppings { get; set; }
    [JsonPropertyName("base")]
    public string BaseSauce { get; set; }
    [JsonPropertyName("price")]
    public double Price { get; set; } = 0;

    public Pizza(string size, string dough, string[] toppings, string baseSauce) 
    {
        Size = size;
        Dough = dough;
        Toppings = toppings;
        BaseSauce = baseSauce;
    }
}