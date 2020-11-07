using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunosController : ControllerBase
    {
        private readonly dbsponteContext db;

        public AlunosController(dbsponteContext context)
        {
            db = context;
        }

        // GET: api/Alunos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alunos>>> GetAlunos()
        {
            try
            {
                return await db.Alunos.ToListAsync();
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

        // GET: api/Alunos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Alunos>> GetAlunos(int id)
        {
            try
            {
                var alunos = await db.Alunos.FindAsync(id);

                if (alunos == null)
                {
                    return NotFound("Aluno não encotrado ou deletado!");
                }

                return alunos;
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

        // PUT: api/Alunos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlunos(int id, Alunos alunos)
        {
            if (id != alunos.AlunoId)
            {
                return NotFound("Aluno não encontrado para atualização!");
            }
            else
            {
                db.Entry(alunos).State = EntityState.Modified;

                var oAluno = await (from r in db.Alunos where r.Cpf == alunos.Cpf && r.AlunoId != alunos.AlunoId select new { r.AlunoId }).FirstOrDefaultAsync();
                if (oAluno == null)
                {
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AlunosExists(id))
                        {
                            return NotFound("Aluno não encontrado!");
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        Dispose(true);
                    }
                }
                else return BadRequest("Não será possível atualizar, os dados informados são iguais a outro curso já cadastrado!");

                return Ok("Dados do aluno atualizados com sucesso!");
            }
        }

        // POST: api/Alunos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Alunos>> PostAlunos(Alunos alunos)
        {
            try
            {
                var oCurso = await (from r in db.Alunos where r.Cpf == alunos.Cpf select new { r.AlunoId }).FirstOrDefaultAsync();
                if (oCurso == null)
                {
                    db.Alunos.Add(alunos);
                    await db.SaveChangesAsync();

                    return CreatedAtAction("GetAlunos", new { id = alunos.AlunoId }, alunos);
                }
                else return BadRequest("Já existe um aluno cadastado com esse CPF!");
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

        // DELETE: api/Alunos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Alunos>> DeleteAlunos(int id)
        {
            try
            {
                var alunos = await db.Alunos.FindAsync(id);
                if (alunos == null)
                {
                    return NotFound();
                }

                db.Alunos.Remove(alunos);
                await db.SaveChangesAsync();

                return alunos;
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

        private bool AlunosExists(int id)
        {
            return db.Alunos.Any(e => e.AlunoId == id);
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
