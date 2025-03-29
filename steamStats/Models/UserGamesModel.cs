using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace steamStats.Models
{
    public class UserGamesModel
    {
        [Key] 
        public int Id { get; set; }  

        public string username { get; set; }  

        public List<Game> Games { get; set; } = new List<Game>();
    }
}