﻿using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models.Dtos.TicketDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Event_Management.Repositories.CodeRepositoryFolder
{
    public class CodeRepository : ICodeRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly IConfiguration _configuration; // Configuration for accessing app settings
        private readonly IMemoryCache _cache; // Memory cache for storing temporary data

        public CodeRepository(DataContext dataContext, IConfiguration configuration, IMemoryCache cache)
        {
            _context = dataContext;
            _configuration = configuration;
            _cache = cache;
        }

        /// <summary>
        /// Sends an email with the specified text to the given email address.
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

        /// <summary>
        /// Sends an SMS with the specified text to the given phone number using Twilio.
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

        /// <summary>
        /// Sends a ticket to the specified email address with the QR code attached.
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

                    // Strip out the base URL part and ensure relative path is correct
                    string relativeQRCodePath = ticket.QRCodeImageUrl.Replace("https://localhost:7056/", "").TrimStart('/');

                    // Ensure there is no duplicate "Uploads/QRCodes" part
                    if (relativeQRCodePath.StartsWith("Uploads/QRCodes", StringComparison.OrdinalIgnoreCase))
                    {
                        relativeQRCodePath = relativeQRCodePath.Substring("Uploads/QRCodes".Length).TrimStart('/');
                    }

                    // Construct the local file path
                    string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "qrcodes");
                    string qrCodePath = Path.Combine(basePath, relativeQRCodePath);

                    // Check if the file exists before adding it as an attachment
                    if (File.Exists(qrCodePath))
                    {
                        mailMessage.Attachments.Add(new Attachment(qrCodePath));
                    }
                    else
                    {
                        throw new FileNotFoundException($"QR Code image not found: {qrCodePath}");
                    }

                    // Send the email
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

        /// <summary>
        /// Sends an email notification to the specified email address with the given subject and body.
        private async Task SendEmailToNotifyAsync(string email, string subject, string body)
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
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);
                    await client.SendMailAsync(mailMessage);
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

        /// <summary>
        /// Sends an event cancellation notification to the specified email address.
        public async Task SendEventCancellationNotification(string email, string eventTitle)
        {
            string subject = "Event Cancellation Notice";
            string body = $"Dear attendee,<br><br>We regret to inform you that the event <b>{eventTitle}</b> has been canceled. We apologize for any inconvenience caused.<br><br>Best regards,<br>Event Management Team";

            await SendEmailToNotifyAsync(email, subject, body);
        }

        /// <summary>
        /// Sends an event reschedule notification to the specified email address.
        public async Task SendEventRescheduleNotification(string email, string eventTitle, DateTime newDate)
        {
            string subject = "Event Reschedule Notice";
            string body = $"Dear attendee,<br><br>The event <b>{eventTitle}</b> has been rescheduled to <b>{newDate:dddd, MMMM dd, yyyy at hh:mm tt}</b>. Please update your calendar accordingly.<br><br>Best regards,<br>Event Management Team";

            await SendEmailToNotifyAsync(email, subject, body);
        }

        /// <summary>
        /// Sends verification codes to the organizer's email and phone number.
        public async Task<string> SendCodes(int orgnizerId)
        {
            try
            {
                var organizer = await _context.Organizers
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == orgnizerId)
                    ?? throw new NotFoundException($"Organizer with the Id {orgnizerId} not found!");

                var rng = new Random();
                var emailCode = rng.Next(100000, 999999).ToString();
                var smsCode = rng.Next(100000, 999999).ToString();

                organizer.User.EmailVerificationCode = emailCode;
                organizer.User.SmsVerificationCode = smsCode;
                organizer.User.CodeExpiration = DateTime.UtcNow.AddMinutes(10);

                await _context.SaveChangesAsync();

                await SendToEmail(organizer.Email, $"Your email verification code is {emailCode}");
                await SendToPhone(organizer.PhoneNumber, $"Your SMS verification code is {smsCode}");

                return "Verification codes sent successfully.";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Sends verification codes to the specified email and phone number.
        public async Task<bool> SendCodes(string email, string phoneNumber)
        {
            var errorMessages = new List<string>();

            // Email validation
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                errorMessages.Add("A valid email address is required.-");
            }

            // Phone number validation
            if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^\d{9}$"))
            {
                errorMessages.Add("Phone number must be exactly 9 digits.");
            }

            // If there are validation errors, throw them together
            if (errorMessages.Count != 0)
            {
                throw new BadRequestException(string.Join(" ", errorMessages));
            }

            var emailCode = new Random().Next(100000, 999999).ToString();
            var smsCode = new Random().Next(100000, 999999).ToString();

            _cache.Set(email, $"{emailCode},{smsCode}", TimeSpan.FromMinutes(10)); // Auto-expires ✅

            await SendToEmail(email, $"Your email verification code is {emailCode}");
            await SendToPhone(phoneNumber, $"Your SMS verification code is {smsCode}");

            return true;
        }

        /// <summary>
        /// Retrieves the verification codes from the cache for the specified email address.
        public string GetCodes(string email)
        {
            return _cache.TryGetValue(email, out string? codes) ? codes! : string.Empty;
        }

        /// <summary>
        /// Validates the email address format.
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
