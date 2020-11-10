using System;
using Api.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatriculasController : ControllerBase
    {
        private readonly dbsponteContext _context;

        public MatriculasController(dbsponteContext context)
        {
            _context = context;
        }

        // GET: api/Matriculas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Matriculas>>> GetMatriculas()
        {
            return await _context.Matriculas.ToListAsync();
        }

        // GET: api/Matriculas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Matriculas>> GetMatriculas(int id)
        {
            var matriculas = await _context.Matriculas.FindAsync(id);

            if (matriculas == null)
            {
                return NotFound();
            }

            return matriculas;
        }

        // PUT: api/Matriculas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatriculas(int id, Matriculas matriculas)
        {
            if (id != matriculas.MatriculaId)
            {
                return BadRequest();
            }

            _context.Entry(matriculas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatriculasExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Matriculas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Matriculas>> PostMatriculas(Matriculas matriculas)
        {
            _context.Matriculas.Add(matriculas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatriculas", new { id = matriculas.MatriculaId }, matriculas);
        }

        // DELETE: api/Matriculas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Matriculas>> DeleteMatriculas(int id)
        {
            var matriculas = await _context.Matriculas.FindAsync(id);
            if (matriculas == null)
            {
                return NotFound();
            }

            _context.Matriculas.Remove(matriculas);
            await _context.SaveChangesAsync();

            return matriculas;
        }

        private bool MatriculasExists(int id)
        {
            return _context.Matriculas.Any(e => e.MatriculaId == id);
        }
    }
}
