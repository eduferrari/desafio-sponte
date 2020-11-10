using System;
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
    }
}