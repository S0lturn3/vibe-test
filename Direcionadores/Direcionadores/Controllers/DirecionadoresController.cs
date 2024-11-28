using Direcionadores.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Linq;

namespace Direcionadores.Controllers
{
    [RoutePrefix("api/placemarks")]
    public class DirecionadoresController : ApiController
    {

        #region Endpoints

        /// <summary>
        /// [INCOMPLETO] Exporta um novo arquivo KML com base nos filtros fornecidos.
        /// </summary>
        /// <param name="currentFilter">Estrutura que informa quais filtros devem ser aplicados nas pesquisas.</param>
        /// <returns>Novo arquivo KML com base nos filtros aplicados</returns>
        [HttpPost]
        [Route("export")]
        [SwaggerResponse(HttpStatusCode.OK, "Arquivo filtrado e gerado com sucesso.", typeof(System.Web.Mvc.FileContentResult))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro ao gerar o arquivo.", typeof(string))]
        public IHttpActionResult Export(
                [FromBody] CurrentFilter currentFilter
            )
        {
            try
            {
                DirecionadoresFile direcionadoresFile = new DirecionadoresFile(currentFilter);
                AvailableFilters availableFiltersList = direcionadoresFile.GetAvailableFilters();

                this.ValidateFilters(currentFilter, availableFiltersList);


                // Obtenha os dados a serem exportados (por exemplo, os Direcionadores)
                var direcionadores = direcionadoresFile.GetPlacemarks(currentFilter); // Supondo que você tenha um método para obter os dados

                // Crie um novo arquivo KML
                var kmlContent = CreateKml(direcionadores);

                // Salve ou envie o arquivo KML
                var filePath = SaveKmlToFile(kmlContent);

                // Retorne o caminho ou o próprio arquivo KML para o usuário (dependendo do seu caso)
                var fileName = "direcionadores.kml";
                var fileBytes = File.ReadAllBytes(filePath);

                return Ok(new System.Web.Mvc.FileContentResult(fileBytes, "application/vnd.google-earth.kml+xml")
                {
                    FileDownloadName = fileName
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar os registros para exportação: {ex.Message}");
            }
        }


        /// <summary>
        ///  Listar os dados filtrados.
        /// </summary>
        /// <param name="cliente">Filtro customizado referente ao campo CLIENTE. Se não for informado não será validado e não será considerado na pesquisa.</param>
        /// <param name="situacao">Filtro customizado referente ao campo SITUAÇÃO. Se não for informado não será validado e não será considerado na pesquisa.</param>
        /// <param name="bairro">Filtro customizado referente ao campo BAIRRO. Se não for informado não será validado e não será considerado na pesquisa.</param>
        /// <param name="referencia">Filtro customizado referente ao campo REFERÊNCIA. Se não for informado não será validado e não será considerado na pesquisa.</param>
        /// <param name="ruaCruzamento">Filtro customizado referente ao campo RUA/CRUZAMENTO. Se não for informado não será validado e não será considerado na pesquisa.</param>
        /// <returns>Lista de elementos filtrados no formato JSON.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, "Placemarks buscados e filtrados com sucesso.", typeof(List<XElement>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro ao buscar os registros desejados.", typeof(string))]
        public IHttpActionResult List(
                [FromUri] string cliente = "",
                [FromUri] string situacao = "",
                [FromUri] string bairro = "",
                [FromUri] string referencia = "",
                [FromUri] string ruaCruzamento = ""
            )
        {
            CurrentFilter currentFilter = new CurrentFilter
            {
                Cliente = cliente,
                Situacao = situacao,
                Bairro = bairro,
                Referencia = referencia,
                RuaCruzamento = ruaCruzamento
            };

            try
            {
                DirecionadoresFile direcionadoresFile = new DirecionadoresFile(currentFilter);
                AvailableFilters availableFiltersList = direcionadoresFile.GetAvailableFilters();

                this.ValidateFilters(currentFilter, availableFiltersList);

                return Ok(direcionadoresFile.GetPlacemarks(currentFilter));
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
        [SwaggerResponse(HttpStatusCode.OK, "Filtros encontrados com sucesso.", typeof(AvailableFilters))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro ao buscar os filtros disponíveis.", typeof(string))]
        public IHttpActionResult Filters()
        {
            try
            {
                DirecionadoresFile direcionadoresFile = new DirecionadoresFile();
                AvailableFilters availableFiltersList = direcionadoresFile.GetAvailableFilters();

                return Ok(availableFiltersList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Endpoint para carregar e processar um arquivo KML.
        /// </summary>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPost]
        [Route("upload")]
        [SwaggerResponse(HttpStatusCode.OK, "Arquivo carregado com sucesso.", typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro ao carregar o arquivo.", typeof(string))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Erro ao processar o arquivo KML.", typeof(string))]
        public IHttpActionResult Upload()
        {
            try
            {
                HttpPostedFile file = GetFileFromRequest();

                DirecionadoresFile direcionadoresFile = new DirecionadoresFile(file);
                return Ok("KML carregado com sucesso! As próximas chamadas passarão a utilizar o arquivo de upload.");
            }
            catch(FileNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion Endpoints


        #region Private Methods


        // As duas classes abaixo eu havia concluído-as, mas não estava satisfeito com a organização.
        // Então após alguns ajustes e estudos eu pedi para o GPT organizar melhor com base no que queria deixar e o que queria abtrair

        /// <summary>
        /// Método para validar filtros de texto parciais
        /// </summary>
        /// <param name="currentFilter">Filtros atuais informados para a API</param>
        /// <param name="availableFiltersList">Lista de valores disponíveis para seleção de CLIENTE, SITUAÇÃO e BAIRRO</param>
        private void ValidateFilters(CurrentFilter currentFilter, AvailableFilters availableFiltersList)
        {
            // Checagem de comprimento mínimo
            if (!String.IsNullOrWhiteSpace(currentFilter.Referencia) && currentFilter.Referencia.Length < 3)
                throw new Exception("O filtro de REFERÊNCIA deve possuir pelo menos 3 caracteres.");
            
            if (!String.IsNullOrWhiteSpace(currentFilter.RuaCruzamento) && currentFilter.RuaCruzamento.Length < 3)
                throw new Exception("O filtro de RUA/CRUZAMENTO deve possuir pelo menos 3 caracteres.");

            // Verificação de existência nos filtros
            ValidateOptionsFilterExistence(currentFilter, availableFiltersList);
        }

        /// <summary>
        /// Método para validar filtros de seleção na lista, geralmente será automaticamente chamada dentro da <see cref="ValidateFilters(CurrentFilter, AvailableFilters)"/>
        /// </summary>
        /// <param name="currentFilter">Filtros atuais informados para a API</param>
        /// <param name="availableFiltersList">Lista de valores disponíveis para seleção de CLIENTE, SITUAÇÃO e BAIRRO</param>
        private void ValidateOptionsFilterExistence(CurrentFilter currentFilter, AvailableFilters availableFiltersList)
        {
            if (!String.IsNullOrWhiteSpace(currentFilter.Cliente) && !availableFiltersList.CLIENTES.Contains(currentFilter.Cliente))
                throw new Exception("O valor informado para CLIENTE não corresponde aos valores disponíveis.");

            if (!String.IsNullOrWhiteSpace(currentFilter.Bairro) && !availableFiltersList.BAIRROS.Contains(currentFilter.Bairro))
                throw new Exception("O valor informado para BAIRRO não corresponde aos valores disponíveis.");
            
            if (!String.IsNullOrWhiteSpace(currentFilter.Situacao) && !availableFiltersList.SITUACOES.Contains(currentFilter.Situacao))
                throw new Exception("O valor informado para SITUACAO não corresponde aos valores disponíveis.");
        }


        /// <summary>
        /// Obtém o arquivo enviado na requisição e valida se ele é válido.
        /// </summary>
        /// <returns>HttpPostedFile válido.</returns>
        /// <exception cref="BadRequestException">Se nenhum arquivo for enviado ou se o arquivo não for um KML válido.</exception>
        private HttpPostedFile GetFileFromRequest()
        {
            // Obtém a requisição HTTP
            var httpRequest = HttpContext.Current.Request;

            // Verifica se foi enviado algum arquivo
            if (httpRequest.Files.Count == 0)
            {
                throw new FileNotFoundException("Nenhum arquivo foi enviado.");
            }

            // Obtém o arquivo
            HttpPostedFile file = httpRequest.Files[0];

            // Valida o tipo de arquivo (apenas KML é aceito)
            if (file.ContentType != "application/octet-stream" && !file.FileName.EndsWith(".kml"))
            {
                throw new Exception("O arquivo enviado não é um KML.");
            }

            return file;
        }




        private string CreateKml(List<XElement> direcionadores)
        {
            var kmlDoc = new XDocument(
                new XElement("kml",
                    new XAttribute("xmlns", "http://www.opengis.net/kml/2.2"),
                    new XElement("Document",
                        direcionadores.Select(direcionador => new XElement("Placemark",
                            new XElement("name", direcionador.Name),
                            new XElement("Point",
                                new XElement("coordinates", "-37.0471339,-10.9309629,0")
                            )
                        ))
                    )
                )
            );

            // Converter o XDocument para string para salvar no arquivo
            return kmlDoc.ToString();
        }

        private string SaveKmlToFile(string kmlContent)
        {
            var directory = @"C:\Temp\"; // Ou qualquer outro diretório adequado
            var fileName = "direcionadores.kml";
            var filePath = Path.Combine(directory, fileName);

            // Salve o arquivo KML no sistema de arquivos
            File.WriteAllText(filePath, kmlContent);

            return filePath;
        }


        #endregion Private Methods

    }
}
