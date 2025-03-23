using Microsoft.AspNetCore.Mvc;
using Municipei.Models;
using System.Diagnostics;
using Google.Cloud.Firestore;
using Municipei.Interface;
using Google.Rpc;
using Newtonsoft.Json;

namespace Municipei.Controllers
{
    public class HomeController : Controller
    {
        
        private FirestoreDb _bd;
        
        private readonly IHomeModel _service;


        public HomeController(FirestoreDb db, IHomeModel service)
        {
            _bd = db;
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
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
        [HttpPost]
        public async Task<IActionResult> Authorization(HomeModel model)
        {
            try
            {

                var code = await _service.SendCode(model.Email);
                if (code == "Erro")
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
            catch(Exception)
            {
                TempData["MensagemErro"] = "Não conseguimos realizar seu Cadastro";
                return View("Index");
            }
        }
    }
}
