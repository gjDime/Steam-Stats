using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace steamStats.Models
{
    public class GameViewModel
    {
        public Game Game { get; set; }
        public bool IsFavorite { get; set; }
    }
}