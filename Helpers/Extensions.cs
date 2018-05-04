using Microsoft.AspNetCore.Http;

namespace myDotnetApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationErrors(this HttpResponse response, string message){
            response.Headers.Add("Appliction-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}