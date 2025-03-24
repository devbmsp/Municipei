using Microsoft.AspNetCore.Mvc;
using Municipei.Models;
using System.Diagnostics;
using Google.Cloud.Firestore;
using Municipei.Interface;
using Google.Rpc;
using Newtonsoft.Json;
using MunModel;
using FirebaseAdmin.Auth;

namespace Municipei.Controllers
{
    public class HomeController : Controller
    {
        private readonly string admin = "admin@admin.com";
        private readonly string passwordAdmin = "admin123";
        private readonly string occupationadm = "admin";

        private FirestoreDb _bd;
        private readonly IHomeModel _service;
        public HomeController(FirestoreDb db, IHomeModel service)
        {
            _bd = db;
            _service = service;
        }
        public IActionResult Index()
        {
            var json = System.IO.File.ReadAllText("wwwroot/Data/JsonModel.Json");
            var municipios = JsonConvert.DeserializeObject<List<MunicipiosList>>(json)!;

            var model = new HomeModel
            {
                Municipios = municipios
            };
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult CodeValidate()
        {
            return View();
        }
        public IActionResult DashBoard()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            try
            {
                CollectionReference userBD = _bd.Collection("user");
                HomeModel user = await _service.Login(email, password, userBD);
                if (user == null)
                {
                    TempData["MensagemErro"] = "E-mail ou senha inválidos";
                    return View("Login");
                }
                else if (user.Email == admin && user.Occupation == occupationadm && user.Password == passwordAdmin)
                {
                    TempData["MensagemSucesso"] = "Bem-vindo Admin!";
                    return View("Admin", user);
                }
                else
                {
                    TempData["MensagemSucesso"] = $"Login realizado com sucesso: ";
                    return View("DashBoard", user);
                }
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Não conseguimos realizar seu Login";
                return View("Login");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Authorization(HomeModel model)
        {
            try
            {
                var code = await _service.SendCode(model.Email);
                if (code == "Erro" || code == null)
                {
                    TempData["MensagemErro"] = "Não conseguimos realizar sua autenticação";
                    return View("Index");
                }
                else
                {
                    TempData["AuthCode"] = code;
                    TempData["HomeModel"] = JsonConvert.SerializeObject(model);
                    return View("CodeValidate");
                }
            }
            catch
            {
                TempData["MensagemErro"] = "Não conseguimos realizar sua autenticação";
                return View("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register(HomeModel model, string answer)
        {
            try
            {
                var code = TempData["AuthCode"] as string;
                if (answer == code)
                {
                    CollectionReference user = _bd.Collection("user");
                    await _service.RegisterClient(model, user);
                    TempData["MensagemSucesso"] = "Conseguimos realizar Sua Authenticação, Perfil Criado!";
                    return View("Login");
                }
                else
                {
                    TempData["MensagemErro"] = "Não conseguimos realizar Sua Authenticação";
                    return View("Index");
                }

            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Não conseguimos realizar seu Cadastro";
                return View("Index");
            }
        }
    }
}
