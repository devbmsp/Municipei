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

        public async Task<HomeModel> Login(string email, string pass, CollectionReference user)
        {
           try
            {
                var query = user.WhereEqualTo("email", email);
                var snapshot = await query.GetSnapshotAsync();
                var infoLogin = snapshot.Documents.FirstOrDefault();
                if (infoLogin != null)
                {
                    string password = infoLogin.GetValue<string>("password");
                    if (password != null && password == pass)
                    {
                        string emailUser = infoLogin.GetValue<string>("email");
                        string cpf = infoLogin.GetValue<string>("cpf");
                        string occup = infoLogin.GetValue<string>("occupation");
                        string munpr = infoLogin.GetValue<string>("munPR");
                        string phone = infoLogin.GetValue<string>("phone");
                        string name = infoLogin.GetValue<string>("name");
                        var userEnter = new HomeModel()
                        {
                            Password = password,
                            Cpf = cpf,
                            Email = emailUser,
                            MunPR = munpr,
                            Name = name,
                            Occupation = occup,
                            Phone = phone
                        };
                        return userEnter;
                    }
                }
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao realizar o login: " + ex.Message);
                return null;
            }
        }
    }
}
