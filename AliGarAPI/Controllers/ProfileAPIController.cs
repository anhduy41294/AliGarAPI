using AliGarAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AliGarAPI.Controllers
{
    public class ProfileAPIController : ApiController
    {
        #region Helper
        public HttpResponseMessage CreateResponse<T>(HttpStatusCode statusCode, T data)
        {
            return Request.CreateResponse(statusCode, data);
        }

        public HttpResponseMessage CreateResponse(HttpStatusCode statusCode)
        {
            return Request.CreateResponse(statusCode);
        }

        #endregion

        [HttpGet]
        [Route("api/profile/all")]
        public HttpResponseMessage GetAll()
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var list = ctx.Profiles.ToList();

                return CreateResponse(HttpStatusCode.OK, list);
            }
        }

        [HttpGet]
        [Route("api/profile/detail")]
        public HttpResponseMessage Detail([FromUri]decimal id)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var profile = ctx.Profiles.Where(r => r.IdProfile == id).FirstOrDefault();

                if (profile == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                return CreateResponse(HttpStatusCode.OK, profile);
            }
        }

        [HttpDelete]
        [Route("api/profile/delete")]
        public HttpResponseMessage Delete([FromUri]decimal id)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var profile = ctx.Profiles.Where(r => r.IdProfile == id).FirstOrDefault();

                if (profile == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                try
                {
                    ctx.Profiles.Remove(profile);
                    int affected = ctx.SaveChanges();
                    return CreateResponse(HttpStatusCode.OK, affected);
                }
                catch (Exception e)
                {
                    return CreateResponse(HttpStatusCode.Conflict);
                }
            }
        }
        [HttpPost]
        [Route("api/profile/add")]
        public HttpResponseMessage Add([FromBody]Profile newProfile)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                try
                {
                    ctx.Profiles.Add(newProfile);
                    int affected = ctx.SaveChanges();
                    return CreateResponse(HttpStatusCode.OK, affected);
                }
                catch (Exception e)
                {
                    return CreateResponse(HttpStatusCode.Conflict);
                }

            }
        }

        [HttpPost]
        [Route("api/profile/update")]
        public HttpResponseMessage Update([FromBody]Profile updatedProfile)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var profile = ctx.Profiles.Where(sv => sv.IdProfile == updatedProfile.IdProfile).FirstOrDefault();

                if (profile == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }
                try
                {
                    profile.HumidityStandard = updatedProfile.HumidityStandard;
                    profile.LightStandard = updatedProfile.LightStandard;
                    profile.ProfileName = updatedProfile.ProfileName;
                    profile.TemperatureStandard = updatedProfile.TemperatureStandard;
                    profile.WaterDuration = updatedProfile.WaterDuration;
                    profile.Status = updatedProfile.Status;
                    int affected = ctx.SaveChanges();

                    return CreateResponse(HttpStatusCode.OK, affected);
                }
                catch (Exception e)
                {
                    return CreateResponse(HttpStatusCode.Conflict);
                }
            }
        }

        [HttpPost]
        [Route("api/profile/change")]
        public HttpResponseMessage Update([FromUri]int id)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var profile = ctx.Profiles.Where(sv => sv.IdProfile == id).FirstOrDefault();
                var list = ctx.Profiles.ToList();
                foreach (Profile p in list)
                {
                    p.Status = false;
                }

                if (profile == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }
                try
                {
                    profile.Status = true;
                    int affected = ctx.SaveChanges();

                    return CreateResponse(HttpStatusCode.OK, affected);
                }
                catch (Exception e)
                {
                    return CreateResponse(HttpStatusCode.Conflict);
                }
            }
        }

        [HttpPost]
        [Route("api/usermode/update")]
        public HttpResponseMessage UpdateMode([FromBody]UserMode updatedUser)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var mode = ctx.UserModes.FirstOrDefault();

                if (mode == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }
                try
                {
                    mode.Mode = updatedUser.Mode;
                    int affected = ctx.SaveChanges();

                    return CreateResponse(HttpStatusCode.OK, affected);
                }
                catch (Exception e)
                {
                    return CreateResponse(HttpStatusCode.Conflict);
                }
            }
        }

        [HttpGet]
        [Route("api/usermode/get")]
        public HttpResponseMessage GetMode()
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var mode = ctx.UserModes.FirstOrDefault();

                if (mode == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                return CreateResponse(HttpStatusCode.OK, mode);
            }
        }
    }
}
