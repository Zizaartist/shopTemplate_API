using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    [ApiController]
    public class NotificationsRegistrationController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private NotificationHubClient hub;
        ClickContext _context = new ClickContext();
        Functions funcs = new Functions();

        public NotificationsRegistrationController(IHttpContextAccessor contextAccessor)
        {
            hub = Notifications.Instance.Hub; 
            _contextAccessor = contextAccessor;
        }

        // POST api/register
        // This creates a registration id
        [Route("api/[controller]")]
        [Authorize(Roles = "User, Admin, SuperAdmin")]
        [HttpPost]
        //Handle содержит в себе токен, который пользователь получил от firebase
        public async Task<string> Post(string handle = null)
        {
            string newRegistrationId = null;

            // make sure there are no existing registrations for this push handle (used for iOS and Android)
            if (handle != null)
            {
                var registrations = await hub.GetRegistrationsByChannelAsync(handle, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null)
                newRegistrationId = await hub.CreateRegistrationIdAsync();

            return newRegistrationId;
        }

        // PUT api/register/5
        // This creates or updates a registration (with provided channelURI) at the specified id
        [Route("api/[controller]")]
        [Authorize(Roles = "User, Admin, SuperAdmin")]
        [HttpPut]
        public async Task<HttpResponseMessage> Put(string id, DeviceRegistration deviceUpdate)
        {
            RegistrationDescription registration = null;
            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "wns":
                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "apns":
                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "fcm":
                    registration = new FcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    throw new HttpRequestException(HttpStatusCode.BadRequest.ToString());
            }

            registration.RegistrationId = id;

            // add check if user is allowed to add these tags
            registration.Tags = new HashSet<string>(deviceUpdate.Tags);
            registration.Tags.Add("username:" + id);
            var userCl = funcs.identityToUser(User.Identity, _context);
            userCl.DeviceType = deviceUpdate.Platform;
            userCl.NotificationRegistration = "username:" + id;
            await _context.SaveChangesAsync();
            try
            {
                await hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                ReturnGoneIfHubResponseIsGone(e);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE api/register/5
        //Чуток изменил функционал, удаляется только запись в бд
        [Route("api/[controller]")]
        [Authorize(Roles = "User, Admin, SuperAdmin")]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete()
        {
            //await hub.DeleteRegistrationAsync(id);
            //return new HttpResponseMessage(HttpStatusCode.OK);
            var user = funcs.identityToUser(User.Identity, _context);
            user.NotificationRegistration = null;
            await _context.SaveChangesAsync();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse)webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
            }
        }
    }
}
