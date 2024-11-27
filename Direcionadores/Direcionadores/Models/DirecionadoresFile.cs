using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Direcionadores.Models
{
    public class DirecionadoresFile
    {

        #region Private Properties

        private XDocument _currentFile;
        private XNamespace _kmlNamespace;

        private List<XElement> _currentList;

        #endregion Private Properties

        #region Public Properties

        public List<XElement> currentList
        {
            get => this._currentList;
            set => this._currentList = value;
        }
        
        public XDocument currentFile
        {
            get => this._currentFile;
            set => this._currentFile = value;
        }
        public XNamespace kmlNamespace
        {
            get => this._kmlNamespace;
            set => this._kmlNamespace = value;
        }

        public string filePath;

        #endregion Public Properties



        #region Constructor

        public DirecionadoresFile()
        {
            this.OpenFile("http://www.opengis.net/kml/2.2", @"C:\Temp\DIRECIONADORES1.kml");
        }

        #endregion Constructor


        #region Private Methods

        private void OpenFile(string xNamespace, string filePath)
        {
            kmlNamespace = xNamespace;
            currentFile = XDocument.Load(filePath);
            currentList = currentFile.Descendants().Where(elem => elem.Name.LocalName == "Placemark").ToList();
        }

        #endregion Private Methods

        #region Public Methods

        public AvailableFilters GetAvailableFilters()
        {
            AvailableFilters availableFilters = new AvailableFilters();

            List<string> listaClientesCompleta = new List<string>();
            List<string> listaSituacoesCompleta = new List<string>();
            List<string> listaBairrosCompleta = new List<string>();

            try
            {
                // Busca dos valores completos primeiro
                listaClientesCompleta = currentFile.Descendants(kmlNamespace + "ExtendedData")
                    .Descendants(kmlNamespace + "Data")
                    .Where(data => (string)data.Attribute("name") == "CLIENTE")
                    .Select(data => (string)data.Element(kmlNamespace + "value"))
                    .ToList();

                listaSituacoesCompleta = currentFile.Descendants(kmlNamespace + "ExtendedData")
                    .Descendants(kmlNamespace + "Data")
                    .Where(data => (string)data.Attribute("name") == "SITUAÇÃO")
                    .Select(data => (string)data.Element(kmlNamespace + "value"))
                    .ToList();
                
                listaBairrosCompleta = currentFile.Descendants(kmlNamespace + "ExtendedData")
                    .Descendants(kmlNamespace + "Data")
                    .Where(data => (string)data.Attribute("name") == "BAIRRO")
                    .Select(data => (string)data.Element(kmlNamespace + "value"))
                    .ToList();


                // Filtragem e limpeza dos valores completos
                availableFilters.CLIENTES = listaClientesCompleta
                    .Where(valor => !string.IsNullOrWhiteSpace(valor))
                    .Distinct()
                    .OrderBy(valor => valor)
                    .ToList();

                availableFilters.SITUACOES = listaSituacoesCompleta
                    .Where(valor => !string.IsNullOrWhiteSpace(valor))
                    .Distinct()
                    .OrderBy(valor => valor)
                    .ToList();

                availableFilters.BAIRROS = listaBairrosCompleta
                    .Where(valor => !string.IsNullOrWhiteSpace(valor))
                    .Distinct()
                    .OrderBy(valor => valor)
                    .ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return availableFilters;
        }


        public List<XElement> GetPlacemarks(string cliente, string bairro, string situacao, string referencia, string ruaCruzamento)
        {
            // Ainda não retorna os registros filtrados, vou identificar o que está impedindo no meu horário de intervalo
            List<XElement> listaFiltrada = currentFile.Descendants()
                .Where(placemark => placemark.Name.LocalName == "Placemark")
                .Where(placemark =>
                    // Filtrando por RUA/CRUZAMENTO
                    placemark.Descendants()
                        .Where(data => (string)data.Attribute("name") == "RUA/CRUZAMENTO")
                        .Any(data => (string)data.Element("value") == ruaCruzamento.Trim()) &&

                    // Filtrando por CLIENTE
                    placemark.Descendants()
                        .Where(data => (string)data.Attribute("name") == "CLIENTE")
                        .Any(data => (string)data.Element("value") == cliente.Trim()) &&

                    // Filtrando por BAIRRO
                    placemark.Descendants()
                        .Where(data => (string)data.Attribute("name") == "BAIRRO")
                        .Any(data => (string)data.Element("value") == bairro.Trim()) &&

                    // Filtrando por SITUAÇÃO
                    placemark.Descendants()
                        .Where(data => (string)data.Attribute("name") == "SITUAÇÃO")
                        .Any(data => (string)data.Element("value") == situacao.Trim()) &&

                    // Filtrando por REFERENCIA
                    placemark.Descendants()
                        .Where(data => (string)data.Attribute("name") == "REFERENCIA")
                        .Any(data => (string)data.Element("value") == referencia.Trim())
                )
                .ToList();

            return listaFiltrada;
        }

        #endregion Public Methods

    }
}