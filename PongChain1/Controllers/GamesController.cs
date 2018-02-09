using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PongChain1;

namespace PongChain1.Controllers
{
    public class GamesController : ApiController
    {
        private PongChainEntities1 db = new PongChainEntities1();

        // GET: api/Games
        public IQueryable<Game> GetGames()
        {
            return db.Games;
        }

        // GET: api/Games/5
        [ResponseType(typeof(Game))]
        public IHttpActionResult GetGame(int id)
        {
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        [HttpGet]
        [Route("api/PlayerRecord/{id}")]
        public IHttpActionResult GetPlayerName(int id)
        {
            var winCount = db.Games
                .Where(g => g.WinnerPlayersId == id)
                .Count();

            var loseCount = db.Games
                .Where(g => g.LoserPlayersId == id)
                .Count();

            var name = db.Players
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (name == null)
            {
                return NotFound();
            }

            var firstName = name.FirstName;

            var lastName = name.LastName;

            return Ok(firstName + ' ' + lastName + " " + winCount + "W, " + loseCount + "L");
        }

        [HttpGet]
        [Route("api/HeadToHeadRecord/{PlayerId1}/{PlayerId2}")]
        public IHttpActionResult GetHeadToHeadRecord(int PlayerId1, int PlayerId2)
        {
            var winCount1 = db.Games
                .Where(g => g.WinnerPlayersId == PlayerId1 && g.LoserPlayersId == PlayerId2)
                .Count();

            var winCount2 = db.Games
                .Where(g => g.WinnerPlayersId == PlayerId2 && g.LoserPlayersId == PlayerId1)
                .Count();

            var name1 = db.Players
                .Where(p => p.Id == PlayerId1)
                .FirstOrDefault();

            var firstName1 = "";
            var firstName2 = "";
            if (name1 != null)
            {
                firstName1 = name1.FirstName;
            }

            var name2 = db.Players
                .Where(p => p.Id == PlayerId2)
                .FirstOrDefault();

            if (name2 != null)
            {
                firstName2 = name2.FirstName;
            }


            return Ok(firstName1 + ' ' + winCount1 + " - " + winCount2 + " " + firstName2);
        }

        [HttpGet]
        [Route("api/GenerateOdds/{PlayerId1}/{PlayerId2}")]
        public IHttpActionResult GenerateOdds(int PlayerId1, int PlayerId2)
        {
            decimal winCount1 = db.Games
                .Where(g => g.WinnerPlayersId == PlayerId1)
                .Count();

            decimal loseCount1 = db.Games
                .Where(g => g.LoserPlayersId == PlayerId1)
                .Count();

            var name1 = db.Players
                .Where(p => p.Id == PlayerId1)
                .FirstOrDefault();

            if (name1 == null)
            {
                return NotFound();
            }

            decimal winCount2 = db.Games
                .Where(g => g.WinnerPlayersId == PlayerId2)
                .Count();

            decimal loseCount2 = db.Games
                .Where(g => g.LoserPlayersId == PlayerId2)
                .Count();

            var name2 = db.Players
                .Where(p => p.Id == PlayerId2)
                .FirstOrDefault();

            if (name2 == null)
            {
                return NotFound();
            }

            var firstName1 = name1.FirstName;
            var firstName2 = name2.FirstName;

            decimal overAllPercentage1 = winCount1 / (winCount1 + loseCount1);
            decimal overAllPercentage2 = winCount2 / (winCount2 + loseCount2);
            //decimal overAllCompare1 = overAllPercentage1 / overAllPercentage2; //How many times bigger is Player 1's win percentage than Player 2's?
            //decimal overAllCompare2 = overAllPercentage2 / overAllPercentage1; //How many times bigger is Player 2's win percentage than Player 1's?

            decimal h2hWinCount1 = db.Games
               .Where(g => g.WinnerPlayersId == PlayerId1 && g.LoserPlayersId == PlayerId2)
               .Count();

            decimal h2hWinCount2 = db.Games
                .Where(g => g.WinnerPlayersId == PlayerId2 && g.LoserPlayersId == PlayerId1)
                .Count();

            decimal h2hPercentage1 = .5M;  //fix this later....prevented divide by 0 erro
            decimal h2hPercentage2 = .5M;

            if (h2hWinCount1 + h2hWinCount2 != 0)
            {
                h2hPercentage1 = h2hWinCount1 / (h2hWinCount1 + h2hWinCount2);
                h2hPercentage2 = h2hWinCount2 / (h2hWinCount1 + h2hWinCount2);
            }
            //decimal h2hCompare1 = h2hPercentage1 / h2hPercentage2; //How many times bigger is Player 1's head-to-head percentage than Player 2's?
            //decimal h2hCompare2 = h2hPercentage2 / h2hPercentage1; //How many times bigger is Player 2's head-to-head percentage than Player 1's?

            decimal averagePercentage1 = (overAllPercentage1 + h2hPercentage1) / 2.0M;
            decimal averagePercentage2 = (overAllPercentage2 + h2hPercentage2) / 2.0M;

            decimal combinedCompare1 = averagePercentage1 / averagePercentage2;
            decimal combinedCompare2 = averagePercentage2 / averagePercentage1;

            decimal favoriteCompare = 1.0M;
            decimal underdogCompare = 1.0M;
            var favoriteName = "";
            var underdogName = "";
            if (combinedCompare1 >= combinedCompare2)
            {
                favoriteCompare = combinedCompare1;
                underdogCompare = combinedCompare2;
                favoriteName = firstName1;
                underdogName = firstName2;
            }
            else
            {
                favoriteCompare = combinedCompare2;
                underdogCompare = combinedCompare1;
                favoriteName = firstName2;
                underdogName = firstName1;
            }

            decimal oddsMultiplierFavorite = 2 / (favoriteCompare + 1);
            decimal oddsMultiplierUnderdog = favoriteCompare * oddsMultiplierFavorite;
            decimal favoritePercentageChance = 50 * oddsMultiplierFavorite;
            decimal underdogPercentageChance = 50 * oddsMultiplierUnderdog;

            decimal oddsFavorite = 100 * underdogCompare;
            decimal oddsUnderdog = 100 * favoriteCompare;

            return Ok("Favorite: " + favoriteName + " + $" + oddsFavorite.ToString("#.##") + "   ------ Underdog: " + underdogName + " + $" + oddsUnderdog.ToString("#.##"));
        }

        // PUT: api/Games/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGame(int id, Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != game.Id)
            {
                return BadRequest();
            }

            db.Entry(game).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Games
        [ResponseType(typeof(Game))]
        public IHttpActionResult PostGame(Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Games.Add(game);
            try { db.SaveChanges(); }
            catch (Exception)
            {
                return InternalServerError();
            }


            return CreatedAtRoute("DefaultApi", new { id = game.Id }, game);
        }

        // DELETE: api/Games/5
        [ResponseType(typeof(Game))]
        public IHttpActionResult DeleteGame(int id)
        {
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return NotFound();
            }

            db.Games.Remove(game);
            db.SaveChanges();

            return Ok(game);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameExists(int id)
        {
            return db.Games.Count(e => e.Id == id) > 0;
        }
    }
}