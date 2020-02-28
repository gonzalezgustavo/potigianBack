using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PreparersController> logger;

        public SecurityController(SecurityContext securityContext, ILogger<PreparersController> logger)
        {
            this.securityContext = securityContext;
            this.logger = logger;
        }

        [HttpGet("{usr}/{code}")]
        public async Task<ActionResult<Response<User>>> LoginUser(string usr, string code)
        {
            logger.LogInformation($"GET de usuario invocado por " + usr);
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () =>
                {
                    var user = await (from u in securityContext.Users 
                                      join s in securityContext.AccessSystems on u.AccessCode.Trim() equals s.AccessCode.Trim()
                                      where s.Name.Trim() == "Mercaderias" && u.Code.Trim() == usr && u.AccessCode.Trim() == code
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