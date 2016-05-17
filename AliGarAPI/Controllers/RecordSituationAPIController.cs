using AliGarAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AliGarAPI.Controllers
{
    public class RecordSituationAPIController : ApiController
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
        [Route("api/recordsituation/all")]
        public HttpResponseMessage GetAll()
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var list = ctx.RecordSituations.ToList();

                return CreateResponse(HttpStatusCode.OK, list);
            }
        }

        [HttpGet]
        [Route("api/recordsituation/lastest")]
        public HttpResponseMessage GetLastest()
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var list = ctx.RecordSituations.ToList();
                DateTime m = list.Max(r => r.RecordTime);
                RecordSituation recordsituation = ctx.RecordSituations.Where(r => r.RecordTime == m).FirstOrDefault();

                return CreateResponse(HttpStatusCode.OK, recordsituation);
            }
        }

        [HttpGet]
        [Route("api/recordsituation/detail")]
        public HttpResponseMessage Detail([FromUri]decimal id)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var recordsituation = ctx.RecordSituations.Where(r => r.IdRecordSituation == id).FirstOrDefault();

                if (recordsituation == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                return CreateResponse(HttpStatusCode.OK, recordsituation);
            }
        }

        [HttpDelete]
        [Route("api/recordsituation/delete")]
        public HttpResponseMessage Delete([FromUri]decimal id)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var recordsituation = ctx.RecordSituations.Where(r => r.IdRecordSituation == id).FirstOrDefault();

                if (recordsituation == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                try
                {
                    ctx.RecordSituations.Remove(recordsituation);
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
        [Route("api/recordsituation/add")]
        public HttpResponseMessage Add([FromBody]RecordSituation newRecord)
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                try
                {
                    ctx.RecordSituations.Add(newRecord);
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
