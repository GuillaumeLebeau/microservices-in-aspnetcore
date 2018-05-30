using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using YamlDotNet.Serialization;

namespace LoyaltyProgram.Formatters
{
    public class YamlInputFormatter : TextInputFormatter
    {
        private readonly Deserializer _deserializer;
        
        public YamlInputFormatter(Deserializer deserializer)
        {
            _deserializer = deserializer;

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x-yaml").CopyAsReadOnly());
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/yaml").CopyAsReadOnly());
        }
        
        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            var request = context.HttpContext.Request;
            using (var streamReader = context.ReaderFactory(request.Body, encoding))
            {
                var type = context.ModelType;

                try
                {
                    var model = _deserializer.Deserialize(streamReader, type);
                    return InputFormatterResult.SuccessAsync(model);
                }
                catch (Exception)
                {
                    return InputFormatterResult.FailureAsync();
                }
            }
        }
    }
}
