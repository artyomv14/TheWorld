using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using The_World.Models;
using The_World.Services;
using The_World.ViewModels;

namespace The_World.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IWorldRepository _repo;
        private ILogger<AppController> _logger;

        public AppController(IMailService mailService, IWorldRepository repo, ILogger<AppController> logger)
        {
            _mailService = mailService;
            _repo = repo;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Trips()
        {
            try
            {
                var data = _repo.GetAllTrips();
                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get trips in Index page: {ex.Message}");
                return Redirect("/error");
            }
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (model.Email.Contains("example.com"))
            {
                ModelState.AddModelError("", "We do not support fictional addresses");
            }
            if (ModelState.IsValid)
            {
                _mailService.SendMail(model);
                ViewBag.UserMessage = "Message Sent";
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}

