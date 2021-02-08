using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationR.Security.Jwt;

namespace WebApplicationR.Models
{
    public class Login
    {
        public class Request:IRequest<UserBindingModel>
        {
            public string email  { get; set; }
            public string Password { get; set; }

        }
        public class loginValidator: AbstractValidator<Request>
        {
            public loginValidator()
            {
                RuleFor(x => x.email).NotEmpty().NotNull();
                RuleFor(x => x.Password).NotEmpty().NotNull();
            }
        }
        public class Handler : IRequestHandler<Request, UserBindingModel>
        {
            
            private readonly UserManager<User> userManager;
            private readonly SignInManager<User> signInManager;
            private readonly IJwtGenerator jwtGenerator;
            public Handler(UserManager<User> userManager, IJwtGenerator jwtGenerator, SignInManager<User> signInManager)
            {
                
                this.userManager = userManager;
                this.jwtGenerator = jwtGenerator;  
                this.signInManager = signInManager;
            
            }
            public async Task<UserBindingModel> Handle(Request request, CancellationToken cancellationToken)
            {
                var usuario = await userManager.FindByEmailAsync(request.email);
                if (usuario == null)
                {
                    throw new Exception("Email not register yet");
                }
                var result = await signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);
                if (result.Succeeded)
                {
                    return (new UserBindingModel
                    {
                        Name = usuario.Name,
                        Email = usuario.Email,
                        UserName = usuario.UserName,
                        Token = jwtGenerator.GenerateToken(usuario),
                    });
                }
                throw new Exception("password incorrect");
            }
        }
    }
}
