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

            if(name == null)
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

            var firstName1 = name1.FirstName;

            var name2 = db.Players
                .Where(p => p.Id == PlayerId2)
                .FirstOrDefault();

            var firstName2 = name2.FirstName;


            return Ok(firstName1 + ' ' + winCount1 + " - " + winCount2 + " " + firstName2);
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