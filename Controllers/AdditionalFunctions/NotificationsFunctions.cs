using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ApiClick.Controllers
{
    public class NotificationsFunctions
    {
        //private readonly IHttpContextAccessor _contextAccessor;

        //public NotificationsFunctions(IHttpContextAccessor contextAccessor)
        //{
        //    this._contextAccessor = contextAccessor;
        //}

        ///// <summary>
        ///// Sends notifications
        ///// </summary>
        ///// <param name="pns">Push notification services, enum</param>
        ///// <param name="message">Text message</param>
        ///// <param name="to_tag">Receiver's tag, users shouldn't be allowed to see this D:</param>
        //public async Task<HttpResponseMessage> Post(string pns, [FromBody] string message, string to_tag)
        //{
        //    var user = _contextAccessor.HttpContext.User.Identity.Name;
        //    string[] userTag = new string[2];
        //    userTag[0] = "username:" + to_tag;
        //    userTag[1] = "from:" + user;

        //    NotificationOutcome outcome = null;
        //    HttpStatusCode ret = HttpStatusCode.InternalServerError;

        //    switch (pns.ToLower())
        //    {
        //        //case "wns":
        //        //    // Windows 8.1 / Windows Phone 8.1
        //        //    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
        //        //                "From " + user + ": " + message + "</text></binding></visual></toast>";
        //        //    outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
        //        //    break;
        //        case "apns":
        //            // iOS
        //            var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + message + "\"}}";
        //            outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
        //            break;
        //        case "fcm":
        //            // Android
        //            var notif = "{ \"data\" : {\"message\":\"" + "From " + user + ": " + message + "\"}}";
        //            outcome = await Notifications.Instance.Hub.SendFcmNativeNotificationAsync(notif, userTag);
        //            break;
        //    }

        //    if (outcome != null)
        //    {
        //        if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
        //            (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
        //        {
        //            ret = HttpStatusCode.OK;
        //        }
        //    }

        //    return new HttpResponseMessage(ret);
        //}

        public async Task<NotificationOutcome> ToSendNotificationAsync(string pns, string message, string userTag)
        {
            switch (pns.ToLower())
            {
                case "apns":
                    // iOS
                    var alert = "{\"aps\":{\"alert\":\"" + "From " + userTag + ": " + message + "\"}}";
                    return await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                case "fcm":
                    // Android
                    var notif = "{ \"data\" : {\"message\":\"" + "From " + userTag + ": " + message + "\"}}";
                    return await Notifications.Instance.Hub.SendFcmNativeNotificationAsync(notif, userTag);
                default: return null;
            }
        }
    }
}
