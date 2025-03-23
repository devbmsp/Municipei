using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Municipei.Interface;
using Municipei.Models;
using Google.Cloud.Firestore;

namespace Municipei.Service
{
    public class HomeService : IHomeModel
    {
        public async Task<string> SendCode(string email)
        {
            string fromEmail = "brunomatheuspires1@gmail.com";
            string fromPassword = "ayqj idau ihge krnw ";
            string code = "";
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                code += random.Next(0, 10).ToString();
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Municipei", fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Código Municipei";
            message.Body = new TextPart("html")
            {
                Text = $"Olá, seu código é: {code}"
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, fromPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return code;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar o código para o email: " + ex.Message);
                return "Erro";
            }
        }

        public async Task<HomeModel> Login(HomeModel model, CollectionReference user)
        {
            throw new NotImplementedException();
        }

        public async Task<HomeModel> RegisterClient(HomeModel model, CollectionReference user)
        {
            var newUser = new HomeModel()
            {
                Password = model.Password,
                Cpf = model.Cpf,
                Email = model.Email,
                MunPR = model.MunPR,
                Name = model.Name,
                Occupation = model.Occupation,
                Phone = model.Phone
            };
            await user.AddAsync(newUser);
            return newUser;
        }
    }
}
