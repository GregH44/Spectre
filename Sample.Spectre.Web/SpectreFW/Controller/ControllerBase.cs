using Microsoft.AspNetCore.Mvc;
using SpectreFW.Attributes;

namespace SpectreFW.Controller
{
    [ServiceFilter(typeof(CorrelationIdAttribute))]
    [ServiceFilter(typeof(ExceptionHandlerAttribute))]
    public class ControllerBase : Microsoft.AspNetCore.Mvc.Controller
    {
        public string CorrelationId { get; set; }
    }
}
