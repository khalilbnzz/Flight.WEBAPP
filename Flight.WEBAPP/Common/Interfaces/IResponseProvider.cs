namespace Flight.WEBAPP.Common.Interfaces
{
    public interface IResponseProvider
    {
        HttpResponseMessage GetAPIResponse(out string pResponse);
    }
}
