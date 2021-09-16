using Spectre.Console;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PizzaApp
{
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

        public override string ToString() => JsonSerializer.Serialize<Order>(this);

        public void PlaceOrder() 
        {
            var size = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What pizza size would you like?")
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(Program.Sizes.Keys));

            var dough = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What pizza dough would you like?")
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(Program.Dough.Keys));

            var sauce = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Pick your favourite sauce!")
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .AddChoices(Program.Sauces.Keys));

            var toppings = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("What toppings would you like for your pizza?")
                    .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a topping, " + 
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(Program.Toppings.Keys));

           if (AnsiConsole.Confirm("Confirm pizza order")) 
           {
               Pizza p = new Pizza(size, dough, toppings.ToArray(), sauce);
               p.Price = CalculatePrice(p);
               Pizzas.Add(p);
           }
        }

        public void GetOrderPrice()
        {
            double total = 0;
            foreach (var pizza in this.Pizzas)
            {
                total += pizza.Price;
            }
            AnsiConsole.Markup($"Total price: [green]{total}[/]\n\n");
        }

        public double CalculatePrice(Pizza pizza) 
        {
            double sum = 0;
            foreach (var topping in pizza.Toppings)
            {
                sum += Program.Toppings[topping];
            }

            sum += Program.Sizes[pizza.Size] * Program.Dough[pizza.Dough] + Program.Sauces[pizza.BaseSauce];
            return sum;
        }
    }
}