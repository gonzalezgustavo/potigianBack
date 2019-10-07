using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotigianHH.Controllers.Model;
using PotigianHH.Database;
using PotigianHH.Model;

namespace PotigianHH.Controllers
{
    [Route("api/seguridad")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly SecurityContext securityContext;

        public SecurityController(SecurityContext securityContext)
        {
            this.securityContext = securityContext;
        }

        [HttpGet("{usr}/{code}")]
        public async Task<ActionResult<Response<User>>> LoginUser(string usr, string code)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () =>
                {
                    var user = await (from u in securityContext.Users 
                                      join s in securityContext.AccessSystems on u.AccessCode.Trim() equals s.AccessCode.Trim()
                                      where s.Name.Trim() == "Mercaderias" && u.Code.Trim() == code && u.Name.Trim() == usr.Trim()
                                      select u).FirstOrDefaultAsync();

                    if (user == null)
                    {
                        user = new User
                        {
                            Code = "-1"
                        };
                    }

                    return user;
                });
        }
    }
}