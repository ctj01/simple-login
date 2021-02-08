using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationR.Security.Jwt;

namespace WebApplicationR.Models
{
    public class UserRequest
    {
        public class Request : IRequest<UserBindingModel>
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public String UserName { get; set; }
            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().NotNull();
                RuleFor(x => x.Password).NotEmpty().NotNull();
                RuleFor(x => x.Name).NotEmpty().NotNull();
                RuleFor(x => x.UserName).NotEmpty().NotNull();
            }
        }
        public class Handler : IRequestHandler<Request, UserBindingModel>
        {
            private readonly DbContextUser _context;
            private readonly UserManager<User> userManager;
            private readonly IJwtGenerator jwtGenerator;
            private readonly RoleManager<IdentityRole> roleManager;
            private readonly IHttpContextAccessor HttpContextAccessor;
            private readonly SignInManager<User> signInManager;
            private readonly LinkGenerator url;

            public Handler(DbContextUser context, UserManager<User> userManager, IJwtGenerator jwtGenerator, RoleManager<IdentityRole> roleManager, IHttpContextAccessor HttpContextAccessor, SignInManager<User> signInManager, LinkGenerator url)
            {
                _context = context;
                this.userManager = userManager;
                this.jwtGenerator = jwtGenerator;
                this.roleManager = roleManager;
                this.HttpContextAccessor = HttpContextAccessor;
                this.signInManager = signInManager;
                this.url = url;
            }
            public string ConfirmEmail(string userid, string token)
            {
                var user = userManager.FindByIdAsync(userid).Result;
                IdentityResult result = userManager.
                            ConfirmEmailAsync(user, token).Result;
                if (result.Succeeded)
                {
                    var Message = "Email confirmed successfully!";
                    return Message;
                   
                }
                else
                {
                    var Message = "Error while confirming your email!";
                    return (Message);
                }
            }
            public async Task<UserBindingModel> Handle(Request request, CancellationToken cancellationToken)
            {
                Guid id = Guid.NewGuid();
                var user = new User
                {
                    Id = id.ToString() ,
                    Name = request.Name,
                    UserName = request.UserName,
                    Email = request.Email
                };
                var result = await userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    if (!roleManager.RoleExistsAsync("Visitor").Result)
                    {
                        var rol = new IdentityRole();
                        rol.Name = "visitor";
                        rol.Id = Guid.NewGuid().ToString();

                        var roleresult = roleManager.CreateAsync(rol).Result;
                        if (!roleresult.Succeeded)
                        {
                            throw new Exception("Error While creating rol");
                        }
                    }
                    await userManager.AddToRoleAsync(user, "Visitor");
                    string confirmationToken = userManager.
                   GenerateEmailConfirmationTokenAsync(user).Result;
                    string confirmation = url.GetUriByPage(HttpContextAccessor.HttpContext,
                        page: "/user/confirmemail",
                        handler: null,
                        values: ConfirmEmail(user.Id, confirmationToken), HttpContextAccessor.HttpContext.Request.Scheme,
                        host: HostString.FromUriComponent("smtp.gmail.com")
                            );
                    SmtpClient client = new SmtpClient();
                    client.DeliveryMethod = SmtpDeliveryMethod.
                     SpecifiedPickupDirectory;
                    client.PickupDirectoryLocation = @"C:\Test";

                    client.Send("cristianmt023@gmail.com", user.Email,
                           "Confirm your email",
                       confirmation);
                    return new UserBindingModel
                    {
                        Name = user.Name,
                        Email = user.Email,
                        Token = jwtGenerator.GenerateToken(user),
                        UserName = user.UserName
                    };
                }
                throw new Exception("Error");
            }
        }
    }
}
