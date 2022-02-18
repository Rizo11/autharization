using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using auth.Entities;
using blog2.ViewModels;
using System.Text.Json;

namespace auth.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userM;
    private readonly SignInManager<User> _signInManager;

    public HomeController(
        ILogger<HomeController> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager
        )
    {
        _logger = logger;
        _userM = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public IActionResult Authorization()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult Register(string returnUrl)
    {
        
        return View(new RegisterViewModel(){ ReturnUrl = returnUrl ?? string.Empty});
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var user = new User()
        {
            FullName = model.Fullname,
            Email = model.Email,
            UserName = model.Username
        };
        var result = await _userM.CreateAsync(user, model.Password);
        if(result.Succeeded)
        {
            _userM.AddToRoleAsync(user, "writer");
            return LocalRedirect($"/account/login?returnUrl={model.ReturnUrl}");
        }
        return BadRequest(JsonSerializer.Serialize(result.Errors));
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        return View(new LoginViewModel() {ReturnUrl = returnUrl});
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await _userM.FindByEmailAsync(model.Email);
        if(user != null)
        {
            await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            //suggested to read about isPersistant
            return LocalRedirect($"{model.ReturnUrl ?? "/"}");
        }

        return BadRequest("Wrong credentials");
    }
}
