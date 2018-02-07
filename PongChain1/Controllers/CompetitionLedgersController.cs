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
    public class CompetitionLedgersController : ApiController
    {
        private PongChainEntities1 db = new PongChainEntities1();

        // GET: api/CompetitionLedgers
        public IQueryable<CompetitionLedger> GetCompetitionLedgers()
        {
            return db.CompetitionLedgers;
        }

        // GET: api/CompetitionLedgers/5
        [ResponseType(typeof(CompetitionLedger))]
        public IHttpActionResult GetCompetitionLedger(int id)
        {
            CompetitionLedger competitionLedger = db.CompetitionLedgers.Find(id);
            if (competitionLedger == null)
            {
                return NotFound();
            }

            return Ok(competitionLedger);
        }

        // PUT: api/CompetitionLedgers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCompetitionLedger(int id, CompetitionLedger competitionLedger)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != competitionLedger.Id)
            {
                return BadRequest();
            }

            db.Entry(competitionLedger).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetitionLedgerExists(id))
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

        // POST: api/CompetitionLedgers
        [ResponseType(typeof(CompetitionLedger))]
        public IHttpActionResult PostCompetitionLedger(CompetitionLedger competitionLedger)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CompetitionLedgers.Add(competitionLedger);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = competitionLedger.Id }, competitionLedger);
        }

        // DELETE: api/CompetitionLedgers/5
        [ResponseType(typeof(CompetitionLedger))]
        public IHttpActionResult DeleteCompetitionLedger(int id)
        {
            CompetitionLedger competitionLedger = db.CompetitionLedgers.Find(id);
            if (competitionLedger == null)
            {
                return NotFound();
            }

            db.CompetitionLedgers.Remove(competitionLedger);
            db.SaveChanges();

            return Ok(competitionLedger);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompetitionLedgerExists(int id)
        {
            return db.CompetitionLedgers.Count(e => e.Id == id) > 0;
        }
    }
}