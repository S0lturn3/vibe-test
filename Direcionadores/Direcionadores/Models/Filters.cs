using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Direcionadores.Models
{
    /// <summary>
    /// Estrutura que informa quais filtros devem ser aplicados nas pesquisas
    /// </summary>
    public class CurrentFilter
    {
        /// <summary>
        /// Filtro de CLIENTE a ser aplicado
        /// </summary>
        /// <example>GRADE</example>
        public string Cliente { get; set; }

        /// <summary>
        /// Filtro de SITUAÇÃO a ser aplicado
        /// </summary>
        /// <example>GRADE</example>
        public string Situacao { get; set; }

        /// <summary>
        /// Filtro de BAIRRO a ser aplicado
        /// </summary>
        /// <example>JARDINS</example>
        public string Bairro { get; set; }

        /// <summary>
        /// Filtro de REFERÊNCIA a ser aplicado
        /// </summary>
        /// <example>ITAÚ</example>
        public string Referencia { get; set; }

        /// <summary>
        /// Filtro de RUA/CRUZAMENTO a ser aplicado
        /// </summary>
        /// <example>AV. FRANCISCO PORTO X AV. JORGE AMADO</example>
        public string RuaCruzamento { get; set; }
    }


    /// <summary>
    /// Estrutura usada na 'api/placemarks/filters' que informa quais filtros de pré-seleção estão atualmente disponíveis para uso.
    /// </summary>
    public class AvailableFilters
    {
        // ERICK: Pensei em usar um dicionário para deixar a possibilidade de filtros mais dinâmica,
        // mas optei pela não implementação pois possivelmente seria uma complexidade desnecessária

        /// <summary>
        /// Filtros de CLIENTES que estão disponíveis para serem selecionados
        /// </summary>
        public List<string> CLIENTES { get; set; }

        /// <summary>
        /// Filtros de SITUAÇÕES que estão disponíveis para serem selecionados
        /// </summary>
        public List<string> SITUACOES { get; set; }

        /// <summary>
        /// Filtros de BAIRROS que estão disponíveis para serem selecionados
        /// </summary>
        public List<string> BAIRROS { get; set; }


        public AvailableFilters()
        {
            CLIENTES = new List<string>();
            SITUACOES = new List<string>();
            BAIRROS = new List<string>();
        }
    }
}