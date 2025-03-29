using steamStats.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace steamStats.Models
{
    public class Game
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int playerCount { get; set; }
        
        public double price { get; set; }
        public string Currency { get; set; }
        public string imageUrl { get; set; }

        public virtual ICollection<ApplicationUser> UserGames { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Game game &&
                   (Id == game.Id || Name == game.Name);
        }

        public bool IsInPopular { get; set; }
        public int InFavoriteCount { get; set; } = 0;
        //public Dictionary<string,bool> InUserList { get; set; }= new Dictionary<string,bool>();

        public Game()
        {

        }
        public Game(Game game)
        {
            Id = game.Id;
            Name = game.Name;
            this.playerCount = game.playerCount;
            this.price = game.price;
            Currency = game.Currency;
            this.imageUrl = game.imageUrl;
            UserGames = game.UserGames;
        }


        //public DateTime lastUpdated { get; set; }

    }
}