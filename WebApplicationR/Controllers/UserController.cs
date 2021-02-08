using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationR.Models;
using WebApplicationR.Security.Jwt;

namespace WebApplicationR.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> usermanager;
        public UserController(IMediator mediator, UserManager<User> usermanager)
        {
            this._mediator = mediator;
            this.usermanager = usermanager;
        }
        [Route("[Controller]/ConfirmEmail")]
        public IActionResult ConfirmEmail()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<UserBindingModel>> Register(UserRequest.Request Data)
        {
           var result =  await _mediator.Send(Data);

            return View("~/Views/User/ConfirmEmail.cshtml");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<UserBindingModel>> Login(Login.Request Data)
        {
            var result = await _mediator.Send(Data);
            

            return View("~/Views/Home/index.cshtml");
        }
    }
}
