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
        [Route("api/recordsituation/getchart")]
        public HttpResponseMessage GetChart()
        {
            using (QLAliGarEntities ctx = new QLAliGarEntities())
            {
                var list = ctx.RecordSituations.OrderByDescending(p => p.IdRecordSituation).Take(10).ToList();

                
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
                decimal m = list.Max(r => r.IdRecordSituation);
                RecordSituation recordsituation = ctx.RecordSituations.Where(r => r.IdRecordSituation == m).FirstOrDefault();

                var myHubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                myHubContext.Clients.All.notifyGetDone(1);

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
                    DateTime now = DateTime.Now;
                    newRecord.RecordTime = now;
                    ctx.RecordSituations.Add(newRecord);
                    int affected = ctx.SaveChanges();

                    int ss = 0;

                    var myHubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                    myHubContext.Clients.All.notifySituation(newRecord.Temperature.ToString() + "=" + newRecord.Humidity.ToString());
                    myHubContext.Clients.All.notifyNewDeviceStatus("2=0");

                    
                    ///Check Usermode
                    var flag = ctx.UserModes.FirstOrDefault();
                    if ( flag.Mode == false)
                    {
                        //Manual Mode
                        return CreateResponse(HttpStatusCode.OK, 0);

                    }else
                    {
                        //Auto Mode
                        //Handle Action
                        var profile = ctx.Profiles.Where(p => p.Status == true).FirstOrDefault();

                        var deviceWater = ctx.Devices.Where(p => p.IdDevice == 1).FirstOrDefault();
                        var deviceCover = ctx.Devices.Where(p => p.IdDevice == 2).FirstOrDefault();

                        //Check Temperature
                        if (newRecord.Temperature > profile.TemperatureStandard - 2)
                        {
                            //Check Device Situation
                            if (deviceCover.DeviceStatus == false)
                            {
                                //Cover Device is off
                                RecordAction newAction = new RecordAction();
                                newAction.IdAction = 3;
                                newAction.Duration = 0;
                                newAction.Status = false;

                                ctx.RecordActions.Add(newAction);
                                deviceCover.DeviceStatus = false;
                                ctx.SaveChanges();

                                //Notify
                                myHubContext.Clients.All.notifyNewDeviceStatus("2=1");

                                return CreateResponse(HttpStatusCode.OK, 0);
                            }
                        }
                        else
                        {
                            //Check Device Situation
                            if (deviceCover.DeviceStatus == true)
                            {
                                //Cover Device is on
                                RecordAction newAction = new RecordAction();
                                newAction.IdAction = 4;
                                newAction.Duration = 0;
                                newAction.Status = false;

                                ctx.RecordActions.Add(newAction);
                                deviceCover.DeviceStatus = false;
                                ctx.SaveChanges();

                                //Notify
                                myHubContext.Clients.All.notifyNewDeviceStatus("2=0");

                                return CreateResponse(HttpStatusCode.OK, 0);
                            }
                        }

                        //Check Humidity
                        if (newRecord.Humidity < profile.HumidityStandard)
                        {
                            //Check Device Situation
                            if (deviceWater.DeviceStatus == false)
                            {
                                //Water Device is off
                                RecordAction newAction = new RecordAction();
                                newAction.IdAction = 1;
                                newAction.Duration = profile.WaterDuration;
                                newAction.Status = false;

                                ctx.RecordActions.Add(newAction);
                                deviceWater.DeviceStatus = false;
                                ctx.SaveChanges();

                                //Notify
                                myHubContext.Clients.All.notifyNewDeviceStatus("1=1");
                                return CreateResponse(HttpStatusCode.OK, 0);
                            }
                        }

                        return CreateResponse(HttpStatusCode.OK, 0);
                    }
                }
                catch (Exception e)
                {
                    return CreateResponse(HttpStatusCode.Conflict);
                }

            }
        }
    }
}
