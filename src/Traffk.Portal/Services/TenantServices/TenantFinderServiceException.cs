using RevolutionaryStuff.Core;

namespace TraffkPortal.Services.TenantServices
{
    public enum TenantServiceExceptionCodes
    {
        TenantNotFound,
        TenantNotActive,
        TableauTenantNotFound
    }

    public class TenantFinderServiceException : CodedException<TenantServiceExceptionCodes>
    {
        public TenantFinderServiceException(TenantServiceExceptionCodes code, string message = null)
            : base(code, message)
        { }
    }
}
