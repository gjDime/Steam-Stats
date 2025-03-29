using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using steamStats.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using System.Xml.Linq;

namespace steamStats.Controllers
{
    public class SteamController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<List<Game>> SearchGames(string keyword)
        {
            var url = $"https://store.steampowered.com/api/storesearch/?term={Uri.EscapeDataString(keyword)}&cc=us&category1=998"; // Use `cc=us` for the US currency (USD)
            var response = await client.GetStringAsync(url);
            var jsonResponse = JObject.Parse(response);

            var games = new List<Game>();

            foreach (var item in jsonResponse["items"])
            {
                var id = item["id"].ToString();
                var name = item["name"].ToString();
                var price = item["price"]?["final"] != null ? double.Parse(item["price"]["final"].ToString()) : 0;
                var currency = item["price"]?["currency"]?.ToString() ?? "FTP"; 
                var imageUrl = item["tiny_image"].ToString();

                if (price != 0)
                {
                    price = price / 100;
                }

                games.Add(new Game
                {
                    Id = id,
                    Name = name,
                    Currency = currency,
                    imageUrl = imageUrl,
                    price = price
                });
            }

            return games;
        }


        public async Task<ActionResult> SearchResult(string keyword)
        {

            var games = await SearchGames(keyword);
            return View(games); 
        }

        public ActionResult Search()
        {
            return View();
        }

        // GET: Steam
        public ActionResult Index()
        {
            Debug.WriteLine(db.games.Count());

            var time = Session["LastUpdate"] as DateTime?;
            ViewBag.LastUpdate = time?.ToShortTimeString() ?? "Long time ago";
            ViewBag.disableButton = time.HasValue && (DateTime.Now - time.Value).TotalMinutes < 5;



            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = UserManager.FindByName(User.Identity.Name);

            var model = db.games.ToList().Where(game => game.IsInPopular).Select(game => new GameViewModel
            {
                Game = game,

                IsFavorite = user != null && user.userGames.Any(ug => ug.Equals(game))
            }).ToList();

            return View(model);
        }

        [Authorize]
        public ActionResult userList()
        {

            var user = db.Users.First(u => u.UserName.Equals(User.Identity.Name));
            if (user.userGames == null)
                user.userGames = new List<Game>();

            //var model = db.userGamesModel.FirstOrDefault(ugm => ugm.username.Equals(User.Identity.Name));
            //if (model == null)
            //{
            //    model = new UserGamesModel();
            //    model.username = User.Identity.Name;
            //    db.userGamesModel.Add(model);
            //    db.SaveChanges();
            //}


            var time = Session["LastUpdate"] as DateTime?;
            ViewBag.LastUpdate = time?.ToShortTimeString() ?? "Never";
            ViewBag.disableButton = time.HasValue && (DateTime.Now - time.Value).TotalMinutes < 5;



            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> AddToFavourites(int id)//add to user list
        {


            var game = db.games.FirstOrDefault(g => g.Id == id.ToString());
            if (game == null)
            {
                game = await getGameInfo(id) as Game;
                db.games.Add(game);
            }

            game.InFavoriteCount++;
            //game.IsInFavorite = true;

            var user = db.Users.First(u => u.UserName.Equals(User.Identity.Name));

            if (!user.userGames.Contains(game))
            {
                user.userGames.Add(game);
            }
            await db.SaveChangesAsync();
            return Json(new { success = true });





            //var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //ApplicationUser user = UserManager.FindByName(User.Identity.Name);

            //var model = db.userGamesModel.FirstOrDefault(ugm => ugm.username.Equals(User.Identity.Name));

            //if (!model.Games.Contains(game))
            //{
            //    model.Games.Add(game);
            //    await db.SaveChangesAsync();
            //    return Json(new { success = true });
            //}



            //return Json(new { success = false, message = "Game already in favorites." });
        }

        private async Task<Game> getGameInfo(int id)
        {
            var game = new Game();
            var url = $"https://store.steampowered.com/api/appdetails?appids={id}&cc=us"; // Use `cc=us` for the US currency (USD)
            try
            {
                var response = await client.GetStringAsync(url);
                var jsonResponse = JObject.Parse(response);



                //https://store.steampowered.com/api/appdetails?appids=440 TF2
                game.Id = id.ToString();
                game.Name = jsonResponse[id.ToString()]?["data"]?["name"].ToString();
                double temp = 0;
                double.TryParse(jsonResponse[id.ToString()]?["data"]?["price_overview"]?["final"].ToString(), out temp);
                game.price = temp / 100;
                //Debug.WriteLine("PRICE: "+ temp);
                game.imageUrl = jsonResponse[id.ToString()]?["data"]?["header_image"].ToString();
                //Debug.WriteLine($"=-=-=-id= {game.Id} name = {game.Name} price = {game.price} imageurl={game.imageUrl}");

                //200-5min
                var url2 = $"https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={id}";
                var response2 = await client.GetStringAsync(url2);
                var jsonResponse2 = JObject.Parse(response2);
                game.playerCount = int.Parse(jsonResponse2["response"]?["player_count"].ToString());
                return game;
            }
            catch (HttpRequestException e)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request Error for game {id}: {e.Message}");
                return null;
            }
            catch (JsonException e)
            {
                System.Diagnostics.Debug.WriteLine($"JSON Parsing Error for game {id}: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected Error for game {id}: {e.Message}");
                return null;
            }
        }


        [HttpPost]
        public async Task<ActionResult> AddToDatabase(int id)//add to favourites 
        {
            try
            {
                
                var existingGame = db.games.FirstOrDefault(g => g.Id == id.ToString());
                if (existingGame == null)
                {
                    Game gameToAdd = await getGameInfo(id) as Game;
                    gameToAdd.IsInPopular = true;
                    db.games.Add(gameToAdd);
                    await db.SaveChangesAsync();
                    return Json(new { success = true, message = $"{gameToAdd.Name} added to list successfully" });
                }
                else
                {
                    if (!existingGame.IsInPopular)
                    {
                        existingGame.IsInPopular = true;
                        await db.SaveChangesAsync();
                        return Json(new { success = true, message = $"{existingGame.Name} added to list successfully" });
                    }

                    return Json(new { success = false, message = $"{existingGame.Name} is already added" });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                return Json(new { success = false, message = "Exception" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update()
        {
            foreach (var game in db.games)
            {
                var url2 = $"https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={game.Id}";
                var response2 = await client.GetStringAsync(url2);
                var jsonResponse2 = JObject.Parse(response2);
                game.playerCount = int.Parse(jsonResponse2["response"]?["player_count"].ToString());

                Session["LastUpdate"] = DateTime.Now;
            }
            db.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<ActionResult> UserListDelete(int id)// TODO
        {
            try
            {
                var game = db.games.First(g => g.Id == id.ToString());

                //var userId = User.Identity.Name;
                //var user = db.Users.Include(u => u.userGames).FirstOrDefault(u => u.UserName == userId);
                var user = db.Users.First(u => u.UserName.Equals(User.Identity.Name));

                game.InFavoriteCount--;

                user.userGames.Remove(game);
                //if (game.InFavoriteCount == 0 && game.IsInPopular == false)


                await db.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                Debug.WriteLine($"Exception: {ex.Message}");
                return Json(new { success = false });
            }
        }
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var game = db.games.FirstOrDefault(g => g.Id == id.ToString());
                game.IsInPopular = false;

                if (game.InFavoriteCount == 0)
                    db.games.Remove(game);

                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                Debug.WriteLine($"Exception: {ex.Message}");
                return Json(new { success = false });
            }
        }
    }
}