using AliGarAPI.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AliGarAPI.Controllers
{
    public class RecordActionAPIController : ApiController
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
        [Route("api/recordaction/all")]
        public HttpResponseMessage GetAll()
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                List<RecordAction> list = ctx.RecordActions.ToList();

                Mapper.CreateMap<RecordAction, RecordActionModel>();

                List<RecordActionModel> ret = Mapper.Map<List<RecordAction>, List<RecordActionModel>>(list);

                return CreateResponse(HttpStatusCode.OK, ret);
            }
        }

        [HttpGet]
        [Route("api/recordaction/detail")]
        public HttpResponseMessage Detail([FromUri]decimal id)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var recordaction = ctx.RecordActions.Where(r => r.IdRecordAction == id).FirstOrDefault();

                if (recordaction == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                Mapper.CreateMap<RecordAction, RecordActionModel>();

                RecordActionModel ret = Mapper.Map<RecordAction, RecordActionModel>(recordaction);

                return CreateResponse(HttpStatusCode.OK, ret);
            }
        }

        [HttpDelete]
        [Route("api/recordaction/delete")]
        public HttpResponseMessage Delete([FromUri]decimal id)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var recordaction = ctx.RecordActions.Where(r => r.IdRecordAction == id).FirstOrDefault();

                if (recordaction == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                try
                {
                    ctx.RecordActions.Remove(recordaction);
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
        [Route("api/recordaction/add")]
        public HttpResponseMessage Add([FromBody]RecordAction newRecord)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                try
                {
                    ctx.RecordActions.Add(newRecord);
                    int affected = ctx.SaveChanges();

                    switch ((int)newRecord.IdAction)
                    {
                        case 1:
                            {
                                var editDevice = ctx.Devices.Where(p => p.IdDevice == 1).FirstOrDefault();
                                editDevice.DeviceStatus = true;
                                ctx.SaveChanges();
                                return CreateResponse(HttpStatusCode.OK, editDevice);
                                
                            }
                        case 2:
                            {
                                var editDevice = ctx.Devices.Where(p => p.IdDevice == 1).FirstOrDefault();
                                editDevice.DeviceStatus = false;
                                ctx.SaveChanges();
                                return CreateResponse(HttpStatusCode.OK, editDevice);
                            }
                        case 3:
                            {
                                var editDevice = ctx.Devices.Where(p => p.IdDevice == 2).FirstOrDefault();
                                editDevice.DeviceStatus = true;
                                ctx.SaveChanges();
                                return CreateResponse(HttpStatusCode.OK, editDevice);
                            }
                        case 4:
                            {
                                var editDevice = ctx.Devices.Where(p => p.IdDevice == 2).FirstOrDefault();
                                editDevice.DeviceStatus = false;
                                ctx.SaveChanges();
                                return CreateResponse(HttpStatusCode.OK, editDevice);
                            }
                    }

                    return CreateResponse(HttpStatusCode.OK, affected);
                }
                catch (Exception e)
                {
                    return CreateResponse(HttpStatusCode.Conflict);
                }

            }
        }

        [HttpGet]
        [Route("api/recordaction/lastest")]
        public HttpResponseMessage GetLastest()
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var list = ctx.RecordActions.ToList();

                RecordAction record = ctx.RecordActions.Where(r => r.Status == false).FirstOrDefault();
                if (record == null)
                {
                    return CreateResponse(HttpStatusCode.OK, 0);
                }

                RecordActionModel rs;

                Mapper.CreateMap<RecordAction, RecordActionModel>();

                rs = Mapper.Map<RecordAction, RecordActionModel>(record);

                record.Status = true;
                ctx.SaveChanges();
                
                return CreateResponse(HttpStatusCode.OK, rs);
            }
        }
    }
}
