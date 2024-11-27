using System.Collections.Generic;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Direcionadores.Utils
{

    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (apiDescription.ActionDescriptor.ActionName.ToLower().Contains("upload"))
            {
                operation.parameters = new List<Parameter>
                {
                    new Parameter
                    {
                        name = "file",
                        @in = "formData",
                        description = "Upload de arquivo",
                        required = true,
                        type = "file"
                    }
                };

                operation.consumes.Add("multipart/form-data");
            }
        }
    }


}