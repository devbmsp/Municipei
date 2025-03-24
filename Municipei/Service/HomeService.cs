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
        private readonly GoogleDriveService _driveService;

        public HomeService(GoogleDriveService driveService)
        {
            _driveService = driveService;
        }

        public async Task<string> SendCode(string email, CollectionReference user)
        {
            try
            {
                var query = user.WhereEqualTo("email", email);
                var snapshot = await query.GetSnapshotAsync();
                if (snapshot.Count > 0)
                {
                    return null;
                }
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

                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, fromPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return code;
            }
            catch (Exception)
            {
                return "Erro";
            }
        }

        public async Task<HomeModel> RegisterClient(HomeModel model, CollectionReference user)
        {
            try
            {
                model.Date_Start = DateTime.Now.ToString("dd/MM/yyyy");
                var newUser = new HomeModel()
                {
                    Date_Start = model.Date_Start,
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
            catch (Exception)
            {
                return null;
            }
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
                        int id = infoLogin.GetValue<int>("id");
                        string emailUser = infoLogin.GetValue<string>("email");
                        string cpf = infoLogin.GetValue<string>("cpf");
                        string occup = infoLogin.GetValue<string>("occupation");
                        string munpr = infoLogin.GetValue<string>("munPR");
                        string phone = infoLogin.GetValue<string>("phone");
                        string name = infoLogin.GetValue<string>("name");
                        var userEnter = new HomeModel()
                        {
                            Id = id,
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

        public async Task<List<HomeModel>> GetUsers(CollectionReference user)
        {
            var snapshot = await user.GetSnapshotAsync();
            var listaDeUsuarios = new List<HomeModel>();
            foreach (var doc in snapshot.Documents)
            {
                var usuario = doc.ConvertTo<HomeModel>();
                listaDeUsuarios.Add(usuario);
            }
            return listaDeUsuarios;
        }
        public async Task<bool> SendPdf(CollectionReference user, string email, string municipio)
        {
            try
            {
                var query = user.WhereEqualTo("email", email);
                var snapshot = await query.GetSnapshotAsync();
                if (snapshot.Count == 0)
                {
                    return false;
                }
                string pdfName = municipio + ".pdf";

                var fileId = await _driveService.ObterFileIdPorNomeAsync(pdfName);
                if (string.IsNullOrEmpty(fileId))
                {
                    return false;
                }

                var pdfStream = await _driveService.DownloadFileAsync(fileId);

                string fromEmail = "brunomatheuspires1@gmail.com";
                string fromPassword = "ayqj idau ihge krnw ";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Municipei", fromEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = $"Documento PDF - {municipio}";

                var builder = new BodyBuilder();
                builder.HtmlBody = $"<p>Olá, Segue seu documento referente a {municipio} em anexo.</p>";
                builder.Attachments.Add($"{municipio}.pdf", pdfStream, new ContentType("application", "pdf"));
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromEmail, fromPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar PDF do Drive: {ex.Message}");
                return false;
            }
        }

    }
}
