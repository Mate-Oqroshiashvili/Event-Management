using Event_Management.Models.Dtos.TicketDtos;
using System.Net;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Event_Management.Repositories.CodeRepositoryFolder
{
    public class CodeRepository : ICodeRepository
    {
        private readonly IConfiguration _configuration;

        public CodeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> SendToEmail(string email, string text)
        {
            try
            {
                var userName = _configuration["EmailSettings:UserName"];
                var password = _configuration["EmailSettings:Password"];

                using (var client = new SmtpClient(_configuration["EmailSettings:SmtpServer"]))
                {
                    client.Port = int.Parse(_configuration["EmailSettings:Port"]!);
                    client.Credentials = new NetworkCredential(userName, password);
                    client.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage()
                    {
                        From = new MailAddress(_configuration["EmailSettings:From"]!),
                        Subject = "Verification Code",
                        Body = text,
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                    return "Gmail Code sent successfully";
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> SendToPhone(string phone, string text)
        {
            string accountSid = _configuration["Twilio:AccountSID"]!;
            string authToken = _configuration["Twilio:AuthToken"]!;

            TwilioClient.Init(accountSid, authToken);

            var messageResource = await MessageResource.CreateAsync(
                to: new PhoneNumber("+995" + phone),
                from: new PhoneNumber(_configuration["Twilio:FromPhoneNumber"]),
                body: $"Verification Code: {text}");

            return "SMS Code Sent successfully";
        }

        public async Task<string> SendTicketToEmail(string email, TicketDto ticket)
        {
            try
            {
                var userName = _configuration["EmailSettings:UserName"];
                var password = _configuration["EmailSettings:Password"];

                using (var client = new SmtpClient(_configuration["EmailSettings:SmtpServer"]))
                {
                    client.Port = int.Parse(_configuration["EmailSettings:Port"]!);
                    client.Credentials = new NetworkCredential(userName, password);
                    client.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage()
                    {
                        From = new MailAddress(_configuration["EmailSettings:From"]!),
                        Subject = "Your Event Ticket",
                        Body = $"Your ticket for {ticket.Event.Title} is attached. Please scan the QR code at the entrance.",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    // Update the path to point to the 'qrcodes' folder inside 'Uploads' directory
                    string qrCodePath = Path.Combine("Uploads", "qrcodes", ticket.QRCodeImageUrl.TrimStart('/'));

                    // Ensure the path is valid (if it's relative to your project directory)
                    var fullPath = Path.GetFullPath(qrCodePath);
                    mailMessage.Attachments.Add(new Attachment(fullPath));

                    await client.SendMailAsync(mailMessage);
                    return "Ticket sent successfully!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
