using AliGarAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AliGarAPI.Controllers
{
    public class DeviceAPIController : ApiController
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
        [Route("api/device/all")]
        public HttpResponseMessage GetAll()
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var list = ctx.Devices.ToList();

                return CreateResponse(HttpStatusCode.OK, list);
            }
        }

        [HttpGet]
        [Route("api/device/detail")]
        public HttpResponseMessage Detail([FromUri]decimal id)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var device = ctx.Devices.Where(r => r.IdDevice == id).FirstOrDefault();

                if (device == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                return CreateResponse(HttpStatusCode.OK, device);
            }
        }

        [HttpDelete]
        [Route("api/device/delete")]
        public HttpResponseMessage Delete([FromUri]decimal id)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var device = ctx.Devices.Where(r => r.IdDevice == id).FirstOrDefault();

                if (device == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                try
                {
                    ctx.Devices.Remove(device);
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
        [Route("api/device/add")]
        public HttpResponseMessage Add([FromBody]Device newDevice)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                try
                {
                    ctx.Devices.Add(newDevice);
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
        [Route("api/device/update")]
        public HttpResponseMessage Update([FromBody]Device updatedDevice)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var device = ctx.Devices.Where(sv => sv.IdDevice == updatedDevice.IdDevice).FirstOrDefault();

                if (device == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }
                try
                {
                    device.DeviceName = updatedDevice.DeviceName;
                    device.DeviceStatus = updatedDevice.DeviceStatus;

                    int affected = ctx.SaveChanges();

                    return CreateResponse(HttpStatusCode.OK, affected);
                }
                catch (Exception e)
                {
                    return CreateResponse(HttpStatusCode.Conflict);
                }
            }
        }
    }   
}
