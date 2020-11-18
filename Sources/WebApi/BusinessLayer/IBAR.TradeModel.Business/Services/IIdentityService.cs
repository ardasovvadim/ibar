using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace IBAR.TradeModel.Business.Services
{
    public interface IIdentityService
    {
        void SetIdentity(IIdentity identity);
        long GetIdentityId();
    }

    public class IdentityService : IIdentityService
    {
        private IIdentity _identity = null;
        
        public void SetIdentity(IIdentity identity)
        {
            if (_identity == null)
            {
                _identity = identity;
            }
        }

        public long GetIdentityId()
        {
            return long.Parse((_identity as ClaimsIdentity).Claims
                .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}