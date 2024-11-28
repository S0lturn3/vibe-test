using System.Web.Http;
using WebActivatorEx;
using Direcionadores;
using Swashbuckle.Application;
using Direcionadores.Utils;
using System.IO;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Direcionadores
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
            .EnableSwagger(c =>
            {
                // Configuração básica do Swagger
                c.SingleApiVersion("v2", "Direcionadores")
                 .Description("Projeto ASP .NET desenvolvido como projeto para processo seletivo na empresa Vibe")
                 .Contact(cc => cc.Name("Erick Carvalho").Email("erickcarvalho.contato20@gmail.com"))
                 .License(lc => lc.Name("MIT License").Url("https://opensource.org/licenses/MIT"))
                 ;

                // Para adicionar o suporte ao upload de arquivos, use o método de extensão AddFileUpload
                c.OperationFilter<FileUploadOperationFilter>();


                c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "Documentation/SwaggerAnnotation.xml"));


                // Configurando o caminho virtual (se necessário)
                c.RootUrl(req => req.RequestUri.GetLeftPart(System.UriPartial.Authority));
            })
            .EnableSwaggerUi(c =>
            {
                // Configuração básica do Swagger UI
                c.DocumentTitle("Swagger UI - Direcionadores");
            });
        }
    }
}
