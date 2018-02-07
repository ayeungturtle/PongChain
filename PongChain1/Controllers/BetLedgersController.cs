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
    public class BetLedgersController : ApiController
    {
        private PongChainEntities1 db = new PongChainEntities1();

        // GET: api/BetLedgers
        public IQueryable<BetLedger> GetBetLedgers()
        {
            return db.BetLedgers;
        }

        // GET: api/BetLedgers/5
        [ResponseType(typeof(BetLedger))]
        public IHttpActionResult GetBetLedger(int id)
        {
            BetLedger betLedger = db.BetLedgers.Find(id);
            if (betLedger == null)
            {
                return NotFound();
            }

            return Ok(betLedger);
        }

        // PUT: api/BetLedgers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBetLedger(int id, BetLedger betLedger)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != betLedger.Id)
            {
                return BadRequest();
            }

            db.Entry(betLedger).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BetLedgerExists(id))
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

        // POST: api/BetLedgers
        [ResponseType(typeof(BetLedger))]
        public IHttpActionResult PostBetLedger(BetLedger betLedger)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BetLedgers.Add(betLedger);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = betLedger.Id }, betLedger);
        }

        // DELETE: api/BetLedgers/5
        [ResponseType(typeof(BetLedger))]
        public IHttpActionResult DeleteBetLedger(int id)
        {
            BetLedger betLedger = db.BetLedgers.Find(id);
            if (betLedger == null)
            {
                return NotFound();
            }

            db.BetLedgers.Remove(betLedger);
            db.SaveChanges();

            return Ok(betLedger);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BetLedgerExists(int id)
        {
            return db.BetLedgers.Count(e => e.Id == id) > 0;
        }
    }
}