using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Direcionadores.Controllers
{
    [RoutePrefix("api/placemarks")]
    public class DirecionadoresController : ApiController
    {

        [Route("export")]
        [HttpGet]
        public List<string> export()
        {
            try
            {
                return new List<string>() { "String 0", "String 1", "String 2", "String 3", "String 4" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("")]
        [HttpGet]
        public List<string> list()
        {
            try
            {
                return new List<string>() { "String 0", "String 3", "String 4" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("filters")]
        [HttpGet]
        public List<string> filters()
        {
            try
            {
                return new List<string>() { "String 0", "String 1", "String 2" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        //// GET: api/Direcionadores
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Direcionadores/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Direcionadores
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Direcionadores/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Direcionadores/5
        //public void Delete(int id)
        //{
        //}
    }
}
