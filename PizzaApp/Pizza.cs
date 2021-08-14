using System.Text.Json;
using System.Text.Json.Serialization;

namespace PizzaApp
{

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

        public override string ToString() 
        {
            return JsonSerializer.Serialize<Pizza>(this, new JsonSerializerOptions 
            {
                    WriteIndented = true
            });
        }
    }
}