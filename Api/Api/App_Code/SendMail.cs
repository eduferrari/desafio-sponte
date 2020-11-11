using System;
using Api.Models;
using System.Text;
using System.Linq;
using System.Net.Mail;

namespace WebRestApi.UsefulClasses
{
    public class SendMail
    {
        public string EmailDestino { get; set; }
        public string NomeDestino { get; set; }
        public string EmailRemetente { get; set; }
        public string AssuntoEmail { get; set; }
        public string HtmMensagem { get; set; }

        /* Usar para adicionar 'X' destinatários */
        public MailAddressCollection EmailCopias { get; set; }

        public SendMail()
        {

        }

        public static bool DispatchEmail(SendMail oDados)
        {
            bool rtnAcao = true;

            try
            {
                using (MailMessage objEmail = new MailMessage())
                {
                    objEmail.From = new MailAddress("noreply@darkocode.com.br");
                    objEmail.To.Add(new MailAddress(oDados.EmailDestino));
                    if (!string.IsNullOrEmpty(oDados.EmailRemetente)) objEmail.ReplyToList.Add(new MailAddress(oDados.EmailRemetente));

                    if (oDados.EmailCopias != null)
                    {
                        oDados.EmailCopias.ToList().ForEach(delegate (MailAddress oEmail)
                        {
                            objEmail.Bcc.Add(oEmail.Address);
                        });
                    }

                    objEmail.Priority = MailPriority.Normal;
                    objEmail.IsBodyHtml = true;
                    objEmail.Subject = oDados.AssuntoEmail;
                    objEmail.Body = oDados.HtmMensagem.ToString();
                    objEmail.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");
                    objEmail.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");

                    using (SmtpClient objSmtp = new SmtpClient("smtp.darkocode.com.br", 587))
                    {
                        objSmtp.Credentials = new System.Net.NetworkCredential("noreply@darkocode.com.br", "dK#5583cd");
                        objSmtp.EnableSsl = false;
                        objSmtp.Send(objEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rtnAcao;
        }

        public static bool ConfirmaMatricula(Matriculas oRegistro, dbsponteContext db)
        {
            try
            {
                var oAluno = db.Alunos.Where(r => r.AlunoId == oRegistro.AlunoId).FirstOrDefault();

                string htmCursos = "";
                if (!string.IsNullOrEmpty(oRegistro.Cursos))
                {
                    var cursos = oRegistro.Cursos.Split(',').Select(Int32.Parse);
                    db.Cursos.Where(r => cursos.Contains(r.CursoId)).ToList().ForEach(delegate (Cursos oCurso)
                    {
                        htmCursos += "<p>" + oCurso.Nome + " com duração de " + oCurso.Duracao + "</p>";
                    });

                }

                string htmMensagem = "<!DOCTYPE html>"
                                + "<html>"
                                + "  <head>"
                                + "    <meta name=\"viewport\" content=\"width=device-width\">"
                                + "    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">"
                                + "    <title>Ceicom</title>"
                                + "  </head>"
                                + "  <body class=\"\" style=\"background-color:#f6f6f6;font-family:sans-serif;-webkit-font-smoothing:antialiased;font-size:14px;line-height:1.4;margin:0;padding:0;-ms-text-size-adjust:100%;-webkit-text-size-adjust:100%;\">"
                                + "    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"body\" style=\"border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;background-color:#f6f6f6;width:100%;\">"
                                + "      <tr>"
                                + "        <td style=\"font-family:sans-serif;font-size:14px;vertical-align:top;\">&nbsp;</td>"
                                + "        <td class=\"container\" style=\"font-family:sans-serif;font-size:14px;vertical-align:top;display:block;max-width:580px;padding:10px;width:580px;Margin:0 auto !important;\">"
                                + "          <div class=\"content\" style=\"box-sizing:border-box;display:block;Margin:0 auto;max-width:580px;padding:10px;\">"
                                + "            <table class=\"main\" style=\"border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;background:#fff;border-radius:3px;width:100%;\">"
                                + "              <tr>"
                                + "                <td class=\"wrapper\" style=\"font-family:sans-serif;font-size:14px;vertical-align:top;box-sizing:border-box;padding:20px;\">"
                                + "                  <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;width:100%;\">"
                                + "                    <tr>"
                                + "                      <td style=\"font-family:sans-serif;font-size:14px;vertical-align:top;\">"
                                + "                        <p style=\"font-family:sans-serif;font-size:14px;font-weight:normal;margin:0;Margin-bottom:15px;\">Olá " + oAluno.Nome + ",</p>"
                                + "                        <p style=\"font-family:sans-serif;font-size:14px;font-weight:normal;margin:0;Margin-bottom:15px;\">Sua inscrição acaba ser ser confirmada <strong>Mais informações abaixo</strong>.</p>"
                                + "                        <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"btn btn-primary\" style=\"border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;box-sizing:border-box;width:100%;\">"
                                + "                          <tbody>"
                                + "                            <tr>"
                                + "                              <td align=\"left\" style=\"font-family:sans-serif;font-size:14px;vertical-align:top;padding-bottom:15px;\">"
                                + "                                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;width:100%;width:auto;\">"
                                + "                                  <tbody>"
                                + "                                    <tr>"
                                + "                                      <td style=\"font-family:sans-serif;font-size:14px;font-weight:normal;margin:0;Margin-bottom:15px;\">"
                                + "											<br /><strong>Matrícula: </strong> " + oRegistro.MatriculaId + ""
                                + "											<br /><strong>Data da matrícula: </strong> " + Convert.ToDateTime(oRegistro.Data).ToString("dd/MM/yyyy") + ""
                                + "											<br /><strong>Valor total: </strong> " + oRegistro.ValorTotal.ToString("C2") + ""
                                + "											<br /><strong>Cursos inscritos: </strong> <br />" + htmCursos.ToString() + ""
                                + "										 </td>"
                                + "                                    </tr>"
                                + "                                  </tbody>"
                                + "                                </table>"
                                + "                              </td>"
                                + "                            </tr>"
                                + "                          </tbody>"
                                + "                        </table>"
                                + "                      </td>"
                                + "                    </tr>"
                                + "                  </table>"
                                + "                </td>"
                                + "              </tr>"
                                + "            </table>"
                                + "            <div class=\"footer\" style=\"clear:both;padding-top:10px;text-align:center;width:100%;\">"
                                + "              <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:separate;mso-table-lspace:0pt;mso-table-rspace:0pt;width:100%;\">"
                                + "                <tr>"
                                + "                  <td class=\"content-block\" style=\"font-family:sans-serif;font-size:14px;vertical-align:top;color:#999999;font-size:12px;text-align:center;\">"
                                + "                    <p><span class=\"apple-link\" style=\"color:#999999;font-size:12px;text-align:center;\">*** Este é um e-mail automático e não é necessário respondê-lo ***</span></p>"
                                + "                  </td>"
                                + "                </tr>"
                                + "              </table>"
                                + "            </div>"
                                + "          </div>"
                                + "        </td>"
                                + "        <td style=\"font-family:sans-serif;font-size:14px;vertical-align:top;\">&nbsp;</td>"
                                + "      </tr>"
                                + "    </table>"
                                + "  </body>"
                                + "</html>";

                SendMail oMailConfig = new SendMail()
                {
                    NomeDestino = oAluno.Nome,
                    EmailDestino = oAluno.Nome,
                    EmailRemetente = "noreply@darkocode.com.br",
                    AssuntoEmail = "Inscrição Sponte #" + oRegistro.MatriculaId,
                    HtmMensagem = htmMensagem
                };

                return DispatchEmail(oMailConfig);
            }
            catch
            {
                return false;
            }
        }
    }
}