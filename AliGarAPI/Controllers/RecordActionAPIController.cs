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

                return CreateResponse(HttpStatusCode.OK, list);
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

                    //Notify new food for every user online
                    //var myHubContext = GlobalHost.ConnectionManager.GetHubContext<CentralHub>();
                    //myHubContext.Clients.All.notifyNewBiQuyet(newBiquyet.TENBIQUYET, newBiquyet.IDBiQuyet);

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
