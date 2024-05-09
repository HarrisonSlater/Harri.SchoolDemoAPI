using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.Models.Attributes
{
    public class NullableQueryStringDecimalModelBinder : IModelBinder
    {
        private DecimalModelBinder _defaultBinder;
        public NullableQueryStringDecimalModelBinder(ILoggerFactory loggerFactory)
        {
            _defaultBinder = new DecimalModelBinder(NumberStyles.Any, loggerFactory);
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string rawData = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            if (string.IsNullOrWhiteSpace(rawData))
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
            else if (rawData.Equals("null", System.StringComparison.OrdinalIgnoreCase))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
            }
            else
            {
                await _defaultBinder.BindModelAsync(bindingContext);
            }
        }
    }
}
