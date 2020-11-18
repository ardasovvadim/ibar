using System.Data.Entity.ModelConfiguration;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace IBAR.TradeModel.Business.Services
{
    public interface IActionContextAccessor
    {
        void SetContext(HttpActionContext actionContext);
        void SetModelError(string name, string error);
        void ThrowIfModelInvalid();
        void SetModelErrorAndThrow(string name, string error);
    }

    public class ActionContextAccessor : IActionContextAccessor
    {
        private HttpActionContext _context;
        private ModelStateDictionary _modelState;

        public void ThrowIfModelInvalid()
        {
            if (!_modelState.IsValid)
            {
                throw new ModelValidationException();
            }
        }

        public void SetModelErrorAndThrow(string name, string error)
        {
            SetModelError(name, error);
            throw new ModelValidationException();
        }

        public void SetModelError(string name, string error)
        {
            if (!_modelState.Keys.Contains(name))
                _modelState.SetModelValue(name, null);
            _modelState[name].Errors.Add(error);
        }

        public void SetContext(HttpActionContext actionContext)
        {
            _context = actionContext;
            _modelState = _context.ModelState;
        }
    }
}