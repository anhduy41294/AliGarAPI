using AliGarAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using AliGarAPI.HubCentral;
using System.Web.Http.Cors;
namespace AliGarAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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

                var myHubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                myHubContext.Clients.All.notifyNew("Hello");

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

                //Find the record which has max date time
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
                    
                    /*
                    ///Check Usermode
                    var flag = ctx.UserModes.FirstOrDefault();
                    if ( flag.Mode == true)
                    {
                        //Manual Mode
                        return CreateResponse(HttpStatusCode.OK, affected);

                    }else
                    {
                        //Auto Mode
                        //Handle Action
                        var profile = ctx.Profiles.Where(p => p.Status == true).FirstOrDefault();
                        
                        //Check Temperature
                        if (newRecord.Temperature > profile.TemperatureStandard)
                        {
                            //Check Device Situation
                            if (ctx.Devices.Where(p => p.IdDevice == 1).FirstOrDefault().DeviceStatus == false)
                            {
                                //Water Device is off
                                RecordAction newAction = new RecordAction();
                                newAction.IdAction = 1;
                                newAction.Duration = profile.WaterDuration;
                                newAction.Status = false;

                                ctx.RecordActions.Add(newAction);
                                ctx.SaveChanges();

                                return CreateResponse(HttpStatusCode.OK, newAction);
                            }
                        }

                        //Check Humidity
                        if (newRecord.Humidity < profile.HumidityStandard)
                        {
                            //Check Device Situation
                            if (ctx.Devices.Where(p => p.IdDevice == 1).FirstOrDefault().DeviceStatus == false)
                            {
                                //Water Device is off
                                RecordAction newAction = new RecordAction();
                                newAction.IdAction = 1;
                                newAction.Duration = profile.WaterDuration;
                                newAction.Status = false;

                                ctx.RecordActions.Add(newAction);
                                ctx.SaveChanges();

                                return CreateResponse(HttpStatusCode.OK, newAction);
                            }
                        }

                        //Handle Cover Device....

                        return CreateResponse(HttpStatusCode.OK, 0);
                    }*/

                    var myHubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                    myHubContext.Clients.All.notifyNewSituation(newRecord.Temperature.ToString()+ "="+ newRecord.Humidity.ToString());

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
