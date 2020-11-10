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
        private readonly dbsponteContext db;

        public MatriculasController(dbsponteContext context)
        {
            db = context;
        }

        // GET: api/Matriculas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetMatriculas()
        {
            try
            {
                return await (from m in db.Matriculas join a in db.Alunos on m.AlunoId equals a.AlunoId select new { m.MatriculaId, AlunoNome = a.Nome, m.AlunoId, m.Cursos, m.ValorTotal, m.Data }).ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                Dispose(true);
            }
        }

        // GET: api/Matriculas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Matriculas>> GetMatriculas(int id)
        {
            try
            {
                var matriculas = await db.Matriculas.FindAsync(id);

                if (matriculas == null)
                {
                    return NotFound("Matrícula não identificado ou cancelada!");
                }

                return matriculas;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                Dispose(true);

            }
        }

        // PUT: api/Matriculas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<ActionResult<Matriculas>> PutMatriculas(Matriculas matriculas)
        {
            if (matriculas.MatriculaId > 0)
            {
                if (!string.IsNullOrEmpty(matriculas.Cursos))
                {
                    var cursos = matriculas.Cursos.Split(',').Select(Int32.Parse);
                    if (cursos.Count() > 0)
                    {
                        foreach (var cursoId in cursos)
                        {
                            double valorCurso = double.Parse(db.Cursos.Find(cursoId).Custo);
                            matriculas.ValorTotal += valorCurso;
                        }
                    }
                }

                db.Entry(matriculas).State = EntityState.Modified;

                try
                {
                    if (matriculas.AlunoId > 0 && matriculas.ValorTotal > 0 && !string.IsNullOrEmpty(matriculas.Data))
                    {
                        await db.SaveChangesAsync();
                    }
                    else return BadRequest("Preencha todos os campos para prosseguir!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatriculasExists(matriculas.MatriculaId))
                    {
                        return NotFound("Matrícula não encontrado!");
                    }
                    else
                    {
                        return StatusCode(200);
                    }
                }
                finally
                {
                    Dispose(true);
                }

                return StatusCode(200);
            }
            else return BadRequest("Curso não encontrado para atualização!");
        }

        // POST: api/Matriculas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Matriculas>> PostMatriculas(Matriculas matriculas)
        {
            try
            {
                if (!string.IsNullOrEmpty(matriculas.Cursos))
                {
                    var cursos = matriculas.Cursos.Split(',').Select(Int32.Parse);
                    if (cursos.Count() > 0)
                    {
                        foreach (var cursoId in cursos)
                        {
                            double valorCurso = double.Parse(db.Cursos.Find(cursoId).Custo);
                            matriculas.ValorTotal += valorCurso;
                        }
                    }
                }

                if (matriculas.AlunoId > 0 && matriculas.ValorTotal > 0 && !string.IsNullOrEmpty(matriculas.Data))
                {
                    db.Matriculas.Add(matriculas);
                    await db.SaveChangesAsync();

                    return CreatedAtAction("GetMatriculas", new { id = matriculas.MatriculaId }, matriculas);
                }
                else return BadRequest("Preencha todos os campos para prosseguir!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                Dispose(true);
            }
        }

        // DELETE: api/Matriculas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Matriculas>> DeleteMatriculas(int id)
        {
            try
            {
                var matriculas = await db.Matriculas.FindAsync(id);
                if (matriculas == null)
                {
                    return NotFound();
                }

                db.Matriculas.Remove(matriculas);
                await db.SaveChangesAsync();

                return matriculas;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                Dispose(true);
            }
        }

        private bool MatriculasExists(int id)
        {
            return db.Matriculas.Any(e => e.MatriculaId == id);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}
