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

        /// <summary>
        /// Exportar novo arquivo KML com base em um filtro.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="situacao"></param>
        /// <param name="bairro"></param>
        /// <param name="referencia"></param>
        /// <param name="ruaCruzamento"></param>
        /// <returns>Novo arquivo KML com base nos filtros aplicados.</returns>
        [HttpPost]
        [Route("export")]
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
                DirecionadoresFile direcionadoresFile = new DirecionadoresFile();
                this.ValidateFilters(direcionadoresFile, cliente, bairro, situacao, referencia, ruaCruzamento);

                return Ok(new List<string>() { "String 0" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        ///  Listar os dados filtrados.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="situacao"></param>
        /// <param name="bairro"></param>
        /// <param name="referencia"></param>
        /// <param name="ruaCruzamento"></param>
        /// <returns>Lista de elementos filtrados no formato JSON.</returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(AvailableFilters))]
        public IHttpActionResult List(
                [FromUri] string cliente,
                [FromUri] string situacao,
                [FromUri] string bairro,
                [FromUri] string referencia,
                [FromUri] string ruaCruzamento
            )
        {
            try
            {
                DirecionadoresFile direcionadoresFile = new DirecionadoresFile();
                this.ValidateFilters(direcionadoresFile, cliente, bairro, situacao, referencia, ruaCruzamento);

                return Ok(direcionadoresFile.GetPlacemarks(cliente, bairro, situacao, referencia, ruaCruzamento));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        ///  Retornar os valores únicos dos campos de préseleção lidos do arquivo KML
        /// </summary>
        /// <returns>Estrutura com todos os valores disponíveis para seleção de cada campo de filtragem</returns>
        [HttpGet]
        [Route("filters")]
        [ResponseType(typeof(AvailableFilters))]
        public IHttpActionResult Filters()
        {
            try
            {
                DirecionadoresFile direcionadoresFile = new DirecionadoresFile();
                AvailableFilters availableFiltersList =  direcionadoresFile.GetAvailableFilters();

                return Ok(availableFiltersList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion Endpoints


        #region Private Methods

        private void ValidateFilters(string referencia, string ruaCruzamento)
        {
            if (String.IsNullOrWhiteSpace(referencia) || referencia.Length < 3) throw new Exception("O filtro de REFERÊNCIA deve possuir pelo menos 3 caracteres.");
            if (String.IsNullOrWhiteSpace(ruaCruzamento) || ruaCruzamento.Length < 3) throw new Exception("O filtro de RUA/CRUZAMENTO deve possuir pelo menos 3 caracteres.");
        }


        private void ValidateFilters(DirecionadoresFile direcionadoresInstance, string cliente, string bairro, string situacao, string referencia = "___", string ruaCruzamento = "___")
        {
            if (String.IsNullOrWhiteSpace(referencia) || referencia.Length < 3) throw new Exception("O filtro de REFERÊNCIA deve possuir pelo menos 3 caracteres.");
            if (String.IsNullOrWhiteSpace(ruaCruzamento) || ruaCruzamento.Length < 3) throw new Exception("O filtro de RUA/CRUZAMENTO deve possuir pelo menos 3 caracteres.");


            AvailableFilters availableFiltersList = direcionadoresInstance.GetAvailableFilters();

            bool clienteExists = availableFiltersList.CLIENTES.Exists(c => c == cliente);
            bool bairroExists = availableFiltersList.BAIRROS.Exists(b => b == bairro);
            bool situacaoExists = availableFiltersList.SITUACOES.Exists(s => s == situacao);

            if (!clienteExists) throw new Exception("O valor informado para CLIENTE não corresponde aos valores disponíveis.");
            if (!bairroExists) throw new Exception("O valor informado para BAIRRO não corresponde aos valores disponíveis.");
            if (!situacaoExists) throw new Exception("O valor informado para SITUACAO não corresponde aos valores disponíveis.");
        }

        #endregion Private Methods

    }
}
