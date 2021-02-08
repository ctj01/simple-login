using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationR.Models;

namespace WebApplicationR.Security.Jwt
{
    public interface IJwtGenerator
    {
        string GenerateToken(User user);
    }
}
