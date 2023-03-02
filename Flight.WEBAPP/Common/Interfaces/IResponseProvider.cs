namespace Flight.WEBAPP.Common.Interfaces
{
    public interface IResponseProvider
    {
        HttpResponseMessage GetListFlightResponse(out string pResponse);
        HttpResponseMessage GetAirPortDetailsResponse(string pIataCode, out string pResponse);
        HttpResponseMessage GetAirPortDetailsResponse(string pLat, string pLon, out string pResponse);
        HttpResponseMessage GetAirPlaneInfo(string pIadaCode, out string pResponse);
        HttpResponseMessage GetAirPlaneInfo(out string pResponse);
    }
}
