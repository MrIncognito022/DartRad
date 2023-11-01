using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DartRad.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Returns the Logged In User's ID
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static int GetUserId(this Controller controller)
        {
            return int.Parse(controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
