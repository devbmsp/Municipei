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
        private readonly ISessao _sessao;
        private FirestoreDb _bd;
        private readonly IHomeModel _service;
        public HomeController(FirestoreDb db, IHomeModel service, ISessao sessao)
        {
            _sessao = sessao;
            _bd = db;
            _service = service;
        }
        public IActionResult Index()
        {
            var json = System.IO.File.ReadAllText("wwwroot/Data/JsonModel.Json");
            var municipios = JsonConvert.DeserializeObject<List<MunicipiosList>>(json)!;
            var vm = new RegisterViewModel
            {
                Municipios = municipios
            };

            return View(vm);
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult CodeValidate()
        {
            return View();
        }
        public IActionResult Admin()
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null || usuarioLogado.Occupation != "admin")
            {
                return View("Admin", null);
            }

            return View("Admin", usuarioLogado);
        }
        public IActionResult Perfil()
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null )
            {
                return View("Perfil", null);
            }

            return View("Perfil", usuarioLogado);
        }

        public IActionResult Sair()
        {
            _sessao.RemoverSessaoUsuario();
            return RedirectToAction("Login");
        }
        [HttpPost]
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
                    try
                    {
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        _sessao.CriarSessaoDoUsuario(user);
                    }
                    catch (Exception ex)
                    {
                        TempData["MensagemErro"] = "Não foi possivel estabalecer a conexão !";
                        return View("Login");
                    }
                    TempData["MensagemSucesso"] = "Bem-vindo Admin!";
                    return View("Admin", user);
                }
                else
                {
                    TempData["MensagemSucesso"] = $"Login realizado com sucesso: ";
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    _sessao.CriarSessaoDoUsuario(user);
                    return View("Perfil", user);
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
                CollectionReference user = _bd.Collection("user");

                var code = await _service.SendCode(model.Email, user);
                if (code == null)
                {
                    TempData["MensagemErro"] = "Conta já existente com esse email";
                    return View("Index");
                }
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
                    var response = await _service.RegisterClient(model, user);
                    if (response != null)
                    {
                        TempData["MensagemSucesso"] = "Conseguimos realizar Sua Authenticação, Perfil Criado!";
                        return View("Login");
                    }
                    TempData["MensagemErro"] = "Não conseguimos realizar Sua Authenticação";
                    return View("Index");
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
