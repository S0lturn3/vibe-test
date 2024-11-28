using Direcionadores.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Direcionadores.Models
{
    /// <summary>
    /// Serve para armazenar o arquivo e seus métodos que mexem nele diretamente.
    /// </summary>
    public class DirecionadoresFile
    {

        #region Private Properties

        private static readonly Dictionary<string, CachedFile> _fileCache = new Dictionary<string, CachedFile>();

        private XDocument _currentFile;
        private XNamespace _kmlNamespace;

        #endregion Private Properties

        #region Protected Properties

        protected List<XElement> CurrentList;
        protected CurrentFilter CurrentFilter;

        #endregion Protected Properties

        #region Public Properties

        public string FilePath { get; private set; }
        

        public XDocument CurrentFile
        {
            get => this._currentFile;
            set => this._currentFile = value;
        }

        public XNamespace KmlNamespace
        {
            get => this._kmlNamespace;
            set => this._kmlNamespace = value;
        }

        #endregion Public Properties


        #region Constructor

        // Construtor que usa um arquivo KML de caminho fixo
        public DirecionadoresFile(string filePath = @"C:\Temp\DIRECIONADORES1.kml", string kmlNamespace = "http://www.opengis.net/kml/2.2")
        {
            this.FilePath = filePath;
            this.OpenFile(kmlNamespace, filePath);
        }

        // Construtor com filtro e caminho do arquivo
        public DirecionadoresFile(CurrentFilter currentFilter, string filePath = @"C:\Temp\DIRECIONADORES1.kml", string kmlNamespace = "http://www.opengis.net/kml/2.2")
        {
            this.FilePath = filePath;
            this.CurrentFilter = currentFilter;
            this.OpenFile(kmlNamespace, filePath);
        }

        // Construtor para carregar um arquivo a partir de um HttpPostedFile (upload)
        public DirecionadoresFile(HttpPostedFile file, string kmlNamespace = "http://www.opengis.net/kml/2.2")
        {
            this.FilePath = Path.Combine(Path.GetTempPath(), file.FileName);
            this.OpenFile(kmlNamespace, file.InputStream);
        }

        #endregion Constructor


        #region Private Methods

        /// <summary>
        /// Tenta carregar o arquivo a partir do cache ou do disco, com prioridade para o arquivo de upload.
        /// </summary>
        private void OpenFile(string kmlNamespace, string filePath, bool isUpload = false)
        {

            // Valida se já existe algo no cache
            if (_fileCache.Count > 0)
            {
                // Se encontrar, valida se existe algum do tipo Upload para dar prioridade
                if (_fileCache.Any(cached => cached.Value.Origin == FileOrigin.Upload))
                {
                    KeyValuePair<string, CachedFile> cachedFile = _fileCache.Where(cached => cached.Value.Origin == FileOrigin.Upload).FirstOrDefault();

                    this.CurrentFile = cachedFile.Value.Document;
                    KmlNamespace = kmlNamespace;
                    cachedFile.Value.LastAccess = DateTime.Now;     // Atualiza o tempo de acesso
                    cachedFile.Value.Origin = FileOrigin.Upload;    // Marca como upload
                }

                // Se não encontrar o tipo Upload, assume que é um Local
                else
                {
                    _fileCache[filePath] = new CachedFile
                    {
                        Document = this.CurrentFile,
                        Origin = FileOrigin.Local,
                        LastAccess = DateTime.Now
                    };

                    this.CurrentFile = this.CurrentFile = XDocument.Load(filePath);
                    KmlNamespace = kmlNamespace;
                }
            }
            else
            {
                // Caso não tenha nada no cache, assume que seja a inicialização via disco e carrega do disco e adiciona ao cache
                this.CurrentFile = XDocument.Load(filePath);
                KmlNamespace = kmlNamespace;
                _fileCache[filePath] = new CachedFile
                {
                    Document = this.CurrentFile,
                    Origin = FileOrigin.Local,
                    LastAccess = DateTime.Now
                };
            }
        }

        /// <summary>
        /// Sobrecarga para leitura do arquivo a partir de um InputStream, com prioridade para o upload.
        /// </summary>
        private void OpenFile(string kmlNamespace, Stream fileStream)
        {
            this.CurrentFile = XDocument.Load(fileStream);
            KmlNamespace = kmlNamespace;

            // Se o arquivo for upload, prioriza no cache
            _fileCache[this.FilePath] = new CachedFile
            {
                Document = this.CurrentFile,
                Origin = FileOrigin.Upload,
                LastAccess = DateTime.Now
            };
        }



        /// <summary>
        /// Obtém uma lista de valores distintos de um atributo específico no arquivo KML.
        /// </summary>
        /// <param name="attributeName">O nome do atributo para o qual os valores distintos devem ser obtidos.</param>
        /// <returns>Uma lista de valores distintos para o atributo especificado.</returns>
        /// <exception cref="FileNotFoundException">Lançado quando o arquivo KML não é encontrado.</exception>
        private List<string> GetDistinctValues(string attributeName)
        {
            return CurrentFile.Descendants(KmlNamespace + "ExtendedData")
                .Descendants(KmlNamespace + "Data")
                .Where(data => (string)data.Attribute("name") == attributeName)
                .Select(data => (string)data.Element(KmlNamespace + "value"))
                .Where(valor => !string.IsNullOrWhiteSpace(valor))
                .Distinct()
                .OrderBy(valor => valor)
                .ToList();
        }

        private bool AplicaFiltro(List<XElement> dados, string attributeName, string filtroValor)
        {
            return string.IsNullOrWhiteSpace(filtroValor) ||
                   dados.Any(data =>
                       (string)data.Attribute("name") == attributeName &&
                       data.Descendants().Any(elem => elem.Name.LocalName == "value" && elem.Value.Contains(filtroValor)));
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Busca por todos os filtros disponíveis (e sem se repetir)
        /// </summary>
        /// <returns>Estrutura <see cref="AvailableFilters"/> com as propriedades preenchidas.</returns>
        public AvailableFilters GetAvailableFilters()
        {
            AvailableFilters availableFilters = new AvailableFilters();

            try
            {
                availableFilters.CLIENTES = GetDistinctValues("CLIENTE");
                availableFilters.SITUACOES = GetDistinctValues("SITUAÇÃO");
                availableFilters.BAIRROS = GetDistinctValues("BAIRRO");

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao obter os filtros disponíveis", ex);
            }

            return availableFilters;
        }


        /// <summary>
        /// Busca todos os registros que batam com os filtros informados. Filtros que não foram informados não são considerados na busca.
        /// </summary>
        /// <param name="currentFilter">Estrutura de filtros atualmente informados.</param>
        /// <returns>Lista com elementos encontrados</returns>
        public List<XElement> GetPlacemarks(CurrentFilter currentFilter)
        {
            /*
             * 1. Este método não estava muito bem organizado a nível de código apesar de funcional, portanto o reorganizei junto com ChatGPT para que ficasse mais legível.
             * 
             * 2. O motivo da filtragem não funcionar previamente eu descobri que ao buscar pelo elemento <Data> com atributo name="determinado-valor", 
             * a pesquisa logo depois validava se o seu valor daquele elemento <Data> era o valor do filtro, sendo que o <value> é um elemento interno do <Data>, 
             * eles estavam em níveis diferentes, por isso a pesquisa não o encontrava.
            */

            List<XElement> listaFiltrada = CurrentFile.Descendants()
                .Where(elem => elem.Name.LocalName == "Placemark") // Seleciona apenas os <Placemark>
                .Where(placemark =>
                {
                    var dados = placemark.Descendants()
                                         .Where(elem => elem.Name.LocalName == "Data") // Seleciona apenas os <Data>
                                         .ToList();

                    // Verifica os filtros utilizando o método auxiliar
                    bool filtroRuas = AplicaFiltro(dados, "RUA/CRUZAMENTO", currentFilter.RuaCruzamento);
                            bool filtroCliente = AplicaFiltro(dados, "CLIENTE", currentFilter.Cliente);
                            bool filtroBairro = AplicaFiltro(dados, "BAIRRO", currentFilter.Bairro);
                            bool filtroSituacao = AplicaFiltro(dados, "SITUAÇÃO", currentFilter.Situacao);
                            bool filtroReferencia = AplicaFiltro(dados, "REFERÊNCIA", currentFilter.Referencia);

                    // Retorna true se todos os filtros aplicáveis forem atendidos
                    return filtroRuas && filtroCliente && filtroBairro && filtroSituacao && filtroReferencia;
                })
                .ToList();

            return listaFiltrada;
        }


        #endregion Public Methods

    }
}