using System;
using System.IO;
using Api.Models;
using Api.App_Code;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunosController : ControllerBase
    {
        private readonly dbsponteContext db;
        private readonly IWebHostEnvironment _host;

        public AlunosController(dbsponteContext context, IWebHostEnvironment host)
        {
            db = context;
            this._host = host;
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
        [HttpPut]
        public async Task<IActionResult> PutAlunos([FromBody] IFormFile arquivo, Alunos alunos)
        {
            if (alunos.AlunoId > 0)
            {
                if (arquivo.ContentType.Length > 0)
                {
                    string path = _host.WebRootPath + "/arquivos/avatar";

                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    //Valida tamanha do arquivo: Padrão = 1 MB
                    int MaxContentLength = 1024 * 1024 * 1;
                    if (arquivo.ContentType.Length > MaxContentLength) throw new Exception("Por favor, envie um arquivo até 1 mb.");

                    //Valida extensões de arquivos: Padrão = .jpg, .gif, .png
                    var ext = arquivo.FileName.Substring(arquivo.FileName.LastIndexOf('.'));
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                    if (!AllowedFileExtensions.Contains(ext.ToLower())) throw new Exception("Por favor, envie uma imagem do tipo .jpg,.gif,.png.");

                    using (FileStream filestream = System.IO.File.Create(path + UsefulValidations.MakeUniqueFilename(path, arquivo.FileName)))
                    {
                        await arquivo.CopyToAsync(filestream);
                        filestream.Flush();
                        alunos.Foto = filestream.Name;
                    }
                }

                db.Entry(alunos).State = EntityState.Modified;

                var oAluno = await (from r in db.Alunos where r.Cpf == alunos.Cpf && r.AlunoId != alunos.AlunoId select new { r.AlunoId }).FirstOrDefaultAsync();
                if (oAluno == null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(alunos.Nome) && !string.IsNullOrEmpty(alunos.Email) && !string.IsNullOrEmpty(alunos.DataNascimento))
                        {
                            await db.SaveChangesAsync();
                        }
                        else return BadRequest("Preencha todos os campos para prosseguir!");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AlunosExists(alunos.AlunoId))
                        {
                            return NotFound("Aluno não encontrado!");
                        }
                        else
                        {
                            Ok("Dados do aluno atualizados com sucesso!");
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
            else return NotFound("Aluno não encontrado para atualização!");
        }

        // POST: api/Alunos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Alunos>> PostAlunos([FromBody] IFormFile arquivo, Alunos alunos)
        {
            try
            {
                var oCurso = await (from r in db.Alunos where r.Cpf == alunos.Cpf select new { r.AlunoId }).FirstOrDefaultAsync();
                if (oCurso == null)
                {
                    if (arquivo != null && arquivo.ContentType.Length > 0)
                    {
                        string path = _host.WebRootPath + "/arquivos/avatar";

                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                        //Valida tamanha do arquivo: Padrão = 1 MB
                        int MaxContentLength = 1024 * 1024 * 1;
                        if (arquivo.ContentType.Length > MaxContentLength) throw new Exception("Por favor, envie um arquivo até 1 mb.");

                        //Valida extensões de arquivos: Padrão = .jpg, .gif, .png
                        var ext = arquivo.FileName.Substring(arquivo.FileName.LastIndexOf('.'));
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        if (!AllowedFileExtensions.Contains(ext.ToLower())) throw new Exception("Por favor, envie uma imagem do tipo .jpg,.gif,.png.");

                        using (FileStream filestream = System.IO.File.Create(path + UsefulValidations.MakeUniqueFilename(path, arquivo.FileName)))
                        {
                            await arquivo.CopyToAsync(filestream);
                            filestream.Flush();
                            alunos.Foto = filestream.Name;
                        }
                    }
                    else alunos.Foto = "";

                    if (!string.IsNullOrEmpty(alunos.Nome) && !string.IsNullOrEmpty(alunos.Cpf) && !string.IsNullOrEmpty(alunos.Email) && !string.IsNullOrEmpty(alunos.DataNascimento))
                    {
                        db.Alunos.Add(alunos);
                        await db.SaveChangesAsync();

                        return CreatedAtAction("GetAlunos", new { id = alunos.AlunoId }, alunos);
                    }
                    else return BadRequest("Preencha todos os campos para prosseguir!");
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
