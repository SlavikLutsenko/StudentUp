using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace StudentUp.Models
{
    class Mail
    {
        /// <summary>
        /// Отправка письма на почтовый ящик
        /// </summary>
        /// <param name="smtpServer">Имя SMTP-сервера</param>
        /// <param name="from">Адрес отправителя</param>
        /// <param name="password">пароль к почтовому ящику отправителя</param>
        /// <param name="mailto">Адрес получателя</param>
        /// <param name="caption">Тема письма</param>
        /// <param name="message">Сообщение</param>
        /// <param name="attachFile">Присоединенный файл</param>
        public static void SendMail(string smtpServer, string from, string password, string mailto, string caption, string message, string attachFile = null)
        {
			if (!Validation.IsEmail(mailto)) throw new ValidationDataException("no email");
	        try
	        {
		        MailMessage mail = new MailMessage {From = new MailAddress(@from)};
		        mail.To.Add(new MailAddress(mailto));
		        mail.Subject = caption;
		        mail.Body = message;
		        if (!string.IsNullOrEmpty(attachFile))
			        mail.Attachments.Add(new Attachment(attachFile));
		        SmtpClient client = new SmtpClient
		        {
			        Host = smtpServer,
			        Port = 587,
			        EnableSsl = true,
			        Credentials = new NetworkCredential(@from.Split('@')[0], password),
			        DeliveryMethod = SmtpDeliveryMethod.Network
		        };
		        client.Send(mail);
		        mail.Dispose();
	        }
	        catch (Exception e)
	        {
		        throw new Exception("Mail.Send: " + e.Message);
	        }
        }
    }
}
