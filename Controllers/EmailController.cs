using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Threading.Tasks;

namespace Test.RabbitMQ.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        public EmailController()
        {

        }

        [HttpPost]
        public async Task<ActionResult<bool>> Send()
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Admin",
            "egpadmin@ppa.gov.et");
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress("User",
            "legessehabtu64@gmail.com");
            message.To.Add(to);

            message.Subject = "This is email subject";

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "<h1>Hello World!</h1>";
            bodyBuilder.TextBody = "Hello World!";

            //bodyBuilder.Attachments.Add(env.WebRootPath + "\\file.png");
            //message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            await client.ConnectAsync("indigo.cloudns.io", 465, true);
            await client.AuthenticateAsync("indigo.cloudns.io", "{Pass123!!}");

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            client.Dispose();

            return Ok(true);
        }
    }
}
