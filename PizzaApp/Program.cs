// See https://aka.ms/new-console-template for more information
using Spectre.Console;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text;

namespace PizzaApp
{
    public class Program
    {
        static readonly string _baseUrl = "http://localhost:5000/api";

        public static IDictionary<string, double> Sizes { get; set; } = new Dictionary<string, double>();
        public static IDictionary<string, double> Dough { get; set; } = new Dictionary<string, double>();
        public static IDictionary<string, double> Toppings { get; set; } = new Dictionary<string, double>();
        public static IDictionary<string, double> Sauces { get; set; } = new Dictionary<string, double>();

        public static async Task Main(string[] args)
        {
            var client = new HttpClient();

            AnsiConsole.Render(new FigletText("The Pizza Place")
                    .LeftAligned()
                    .Color(Color.Red));
            
            var menu = await client.GetStringAsync(_baseUrl + "/menu");
            JsonObject parsedMenu = JsonNode.Parse(menu).AsObject();
            DisplayMenu(parsedMenu);

            string name = AnsiConsole.Ask<string>("What's your [green]name[/]?");
            Order order = new Order(Guid.NewGuid(), name);
            bool keepActive = true;

            while (keepActive) 
            {
                order.PlaceOrder();
                if (!AnsiConsole.Confirm("Would you like another pizza?"))
                {
                    keepActive = false;
                }
            }
            
            if (AnsiConsole.Confirm("Place this order?")) 
            {
                var jsonOrder = JsonSerializer.Serialize<Order>(order);
                StringContent newOrder = new StringContent(jsonOrder, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_baseUrl + "/order", newOrder);

                if (response.IsSuccessStatusCode) 
                {
                    order.GetOrderPrice();
                    AnsiConsole.Markup("[blue]Thank you for visiting our store![/]");
                }
                else 
                {
                    AnsiConsole.Markup("[crimson]We could not place your order :([/]");
                }
            }
        }

        public static void DisplayMenu(JsonObject menu)
        {
            foreach (var prop in menu) 
            {
                var table = new Table().RoundedBorder().BorderColor(Color.BlueViolet);
                table.AddColumn(char.ToUpper(prop.Key[0]) + prop.Key.Substring(1));
                if (prop.Key == "size") 
                {
                    table.AddColumn("Dough Multiplier");
                    foreach (var pair in prop.Value.AsObject()) 
                    {
                        table.AddRow(pair.Key, $"{pair.Value}");
                        Sizes.Add(pair.Key, (double)pair.Value);
                    }
                } 
                else 
                {
                    table.AddColumn("Price");
                    foreach (var pair in prop.Value.AsObject())
                    {
                        table.AddRow(pair.Key, $"{pair.Value}");
                        switch (prop.Key) {
                            case "dough": Dough.Add(pair.Key, (double)pair.Value); break;
                            case "topping": Toppings.Add(pair.Key, (double)pair.Value); break;
                            case "base": Sauces.Add(pair.Key, (double)pair.Value); break;
                        }
                    }
                }
                AnsiConsole.Render(table.RoundedBorder());
            }
        }
    }
}

