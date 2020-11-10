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
using Microsoft.VisualBasic;

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
        public async Task<IActionResult> PutAlunos([FromForm] Alunos alunos, IEnumerable<IFormFile> files)
        {
            if (alunos.AlunoId > 0)
            {
                if (files.Count() > 0)
                {
                    foreach (var arquivo in files)
                    {
                        if (arquivo != null && arquivo.Length > 0)
                        {
                            string path = _host.WebRootPath + "/media/avatar";

                            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                            //Valida tamanha do arquivo: Padrão = 1 MB
                            int MaxContentLength = 1024 * 1024 * 1;
                            if (arquivo.ContentType.Length > MaxContentLength) throw new Exception("Por favor, envie um arquivo até 1 mb.");

                            //Valida extensões de arquivos: Padrão = .jpg, .gif, .png
                            var ext = arquivo.FileName.Substring(arquivo.FileName.LastIndexOf('.'));
                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            if (!AllowedFileExtensions.Contains(ext.ToLower())) throw new Exception("Por favor, envie uma imagem do tipo .jpg,.gif,.png.");

                            string fileName = UsefulValidations.MakeUniqueFilename(path, arquivo.FileName);

                            using (FileStream filestream = System.IO.File.Create(path + fileName))
                            {
                                await arquivo.CopyToAsync(filestream);
                                filestream.Flush();
                                alunos.Foto = "/media/avatar/" + fileName;
                            }
                        }
                    }
                }

                db.Entry(alunos).State = EntityState.Modified;

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
                        return StatusCode(200);
                    }
                }
                finally
                {
                    Dispose(true);
                }

                return StatusCode(200);
            }
            else return NotFound("Aluno não encontrado para atualização!");
        }

        // POST: api/Alunos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Alunos>> PostAlunos([FromForm] Alunos alunos, IEnumerable<IFormFile> files)
        {
            try
            {
                var oCurso = await (from r in db.Alunos where r.Cpf == alunos.Cpf select new { r.AlunoId }).FirstOrDefaultAsync();
                if (oCurso == null)
                {
                    if (files.Count() > 0)
                    {
                        foreach (var arquivo in files)
                        {
                            if (arquivo != null && arquivo.Length > 0)
                            {
                                string path = _host.WebRootPath + "/media/avatar";

                                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                                //Valida tamanha do arquivo: Padrão = 1 MB
                                int MaxContentLength = 1024 * 1024 * 1;
                                if (arquivo.ContentType.Length > MaxContentLength) throw new Exception("Por favor, envie um arquivo até 1 mb.");

                                //Valida extensões de arquivos: Padrão = .jpg, .gif, .png
                                var ext = arquivo.FileName.Substring(arquivo.FileName.LastIndexOf('.'));
                                IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                                if (!AllowedFileExtensions.Contains(ext.ToLower())) throw new Exception("Por favor, envie uma imagem do tipo .jpg,.gif,.png.");

                                string fileName = UsefulValidations.MakeUniqueFilename(path, arquivo.FileName);

                                using (FileStream filestream = System.IO.File.Create(path + fileName))
                                {
                                    await arquivo.CopyToAsync(filestream);
                                    filestream.Flush();
                                    alunos.Foto = "/media/avatar" + fileName;
                                }
                            }
                            else alunos.Foto = "";
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

                if (!string.IsNullOrEmpty(alunos.Foto))
                {
                    var fileDelete = new FileInfo(alunos.Foto);
                    if (fileDelete.Exists) fileDelete.Delete();
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
