using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI
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

            if (rawData == "null")
            {
                //Set null property/ flag
                bindingContext.Model = null;
            }
            else
            {
                await _defaultBinder.BindModelAsync(bindingContext);
            }
        }
    }
}
