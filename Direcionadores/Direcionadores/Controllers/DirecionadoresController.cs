using Direcionadores.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Direcionadores.Controllers
{
    [RoutePrefix("api/placemarks")]
    public class DirecionadoresController : ApiController
    {

        #region Endpoints

        [Route("export")]
        [HttpPost]
        [ResponseType(typeof(List<string>))]
        public IHttpActionResult Export(
                [FromUri] string cliente,
                [FromUri] string situacao,
                [FromUri] string bairro,
                [FromUri] string referencia,
                [FromUri] string ruaCruzamento
            )
        {
            try
            {
                this.ValidateFilters("EXPORT", cliente, situacao, bairro, referencia, ruaCruzamento);

                DirecionadoresFile direcionadoresFile = new DirecionadoresFile();
            }
            catch (HttpException ex)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok(new List<string>() { "String 0" });
        }


        [Route("")]
        [HttpGet]
        public IHttpActionResult List(
                //[FromUri] string cliente,
                //[FromUri] string situacao,
                //[FromUri] string bairro,
                //[FromUri] string referencia,
                //[FromUri] string ruaCruzamento
            )
        {
            try
            {
                //this.ValidateFilters("LIST", cliente, situacao, bairro, referencia, ruaCruzamento);

                DirecionadoresFile direcionadoresFile = new DirecionadoresFile();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new List<string>() { "String 0", "String 3", "String 4" });
        }


        [Route("filters")]
        [HttpGet]
        public IHttpActionResult Filters()
        {
            try
            {
                DirecionadoresFile direcionadoresFile = new DirecionadoresFile();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok(new List<string>() { "String 0", "String 1", "String 2" });
        }



        [Route("upload")]
        [HttpPost]
        public IHttpActionResult Import(HttpPostedFile kmlFile)
        {
            try
            {
                DirecionadoresFile direcionadoresFile = new DirecionadoresFile();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok();
        }

        #endregion Endpoints


        #region Private Methods

        private void ValidateFilters(string metodo, string cliente, string situacao, string bairro, string referencia, string ruaCruzamento)
        {
            switch (metodo)
            {
                case "EXPORT":

                    break;

                case "LIST":

                    break;

                case "FILTERS":

                    break;
            }
        }

        #endregion Private Methods

    }
}
