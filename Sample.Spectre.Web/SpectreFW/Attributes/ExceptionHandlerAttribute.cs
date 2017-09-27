﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SpectreFW.Configuration;
using SpectreFW.Constants;
using SpectreFW.Controller;
using System.Linq;
using System.Text;

namespace SpectreFW.Attributes
{
    internal class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger logger;

        public ExceptionHandlerAttribute(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("ExceptionHandlerFilter");
        }

        public override void OnException(ExceptionContext context)
        {
            TraceException(context);
#warning todo
            //InterceptErrorForApi(context);
        }

        private void TraceException(ExceptionContext context)
        {
            if (context != null)
            {
                StringBuilder message = new StringBuilder();
                string correlationId = "Inconnu";

                if (context.HttpContext != null
                    && context.HttpContext.Items != null && context.HttpContext.Items.Count() > 0
                    && context.HttpContext.Items.ContainsKey(GlobalConstants.CorrelationIdName))
                {
                    correlationId = context.HttpContext.Items[GlobalConstants.CorrelationIdName].ToString();
                }

                string urlDonneesContexte = GetUrlAndDataContext(context);

                if (context.Exception.InnerException == null)
                {
                    message.AppendLine($"Trace : {context.Exception.Message} ({GlobalConstants.CorrelationIdName} : {correlationId} || Données : {urlDonneesContexte})");
                    message.AppendLine($"  => {context.Exception.StackTrace}");
                }
                else
                {
                    // Le message correspondant aux exceptions du ServiceLayer, DataAccessLayer et GenericRepository contient le CorrelationId
                    message.AppendLine($"Trace : {context.Exception.Message} (({GlobalConstants.CorrelationIdName} : {correlationId} || Données : {urlDonneesContexte})");
                    message.AppendLine($"  => {context.Exception.StackTrace}");
                    message.AppendLine($"Inner Exception : {context.Exception.InnerException.Message} ({correlationId})");
                    message.AppendLine($"  => {context.Exception.InnerException.StackTrace}");
                }

                logger.LogCritical(message.ToString());
            }
            else
                logger.LogCritical("Le contexte de l'exception vaut null !!!");
        }

        private string GetUrlAndDataContext(ExceptionContext context)
        {
            var buildDatas = new StringBuilder("{");

            buildDatas.Append("Datas : ");

            if (context.RouteData.Values.Count > 0)
            {
                buildDatas.Append("{ ");

                foreach (var kvp in context.RouteData.Values)
                {
                    buildDatas.AppendFormat("{0}={1} ", kvp.Key, kvp.Value);
                }

                buildDatas.Append("}");
            }
            else
                buildDatas.Append("Aucune");

            buildDatas.Append("}");

            return buildDatas.ToString();
        }

        private void InterceptErrorForApi(ExceptionContext context)
        {
            if (context.ActionDescriptor.RouteValues.Any(route => route.Key == "controller" && route.Value == "Generic"))
            {
                int statusCodeErrorApi;

                if (!int.TryParse(ConfigurationManager.GetValue("Framework:StatusCodeErrorAPI"), out statusCodeErrorApi))
                {
                    statusCodeErrorApi = 500;
                }

                var errorMessage = context.Exception.Message;

                if (context.Exception.InnerException != null)
                    errorMessage = context.Exception.InnerException.Message;

                context.Result = new ControllerBase().StatusCode(statusCodeErrorApi, errorMessage);
            }
        }
    }
}
