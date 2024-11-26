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

        private List<Placemark> _currentList;
        private Filters _currentFilters;

        #endregion Private Properties

        #region Public Properties

        public List<Placemark> currentList
        {
            get => this._currentList;
            set => this._currentList = value;
        }

        public Filters currentFilters
        {
            get => this._currentFilters;
            set => this._currentFilters = value;
        }

        #endregion Public Properties



        #region Constructor

        public DirecionadoresFile()
        {
            this.OpenFile("http://www.opengis.net/kml/2.2", @"C:\Temp\DIRECIONADORES1.kml");
        }

        #endregion Constructor


        #region Private Methods

        private void OpenFile(string kmlNamespace, string filePath)
        {
            XDocument file = XDocument.Load(filePath);
            List<XElement> placemarks = file.Descendants().Where(elem => elem.Name.LocalName == "Placemark").ToList();

            var placemarksList = placemarks.Descendants(kmlNamespace+"Placemark")
                .Select(placemark => new Placemark
                {
                    Name = placemark.Element(kmlNamespace+"name").Value,
                    Description = placemark.Element(kmlNamespace+"description").Value
                }).ToList();


            this.currentList = placemarksList;
        }

        #endregion Private Methods

    }
}