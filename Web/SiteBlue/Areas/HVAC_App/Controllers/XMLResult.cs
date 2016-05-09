using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HVACapp.Areas.HVAC_App.Controllers
{
    public class XmlResult : ActionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlResult"/> class.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize to XML.</param>
        public XmlResult(object objectToSerialize)
        {
            ObjectToSerialize = objectToSerialize;
        }

        /// <summary>
        /// Gets the object to be serialized to XML.
        /// </summary>
        public object ObjectToSerialize { get; private set; }

        /// <summary>
        /// Serialises the object that was passed into the constructor to XML and writes the corresponding XML to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ControllerContext context)
        { 
            if (ObjectToSerialize is string)
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.ContentType = "text/xml";
                context.HttpContext.Response.Output.WriteLine((string)ObjectToSerialize);
            }
            else
                if (ObjectToSerialize != null)
                {
                    context.HttpContext.Response.Clear();
                    var xs = new System.Xml.Serialization.XmlSerializer(ObjectToSerialize.GetType());
                    context.HttpContext.Response.ContentType = "text/xml";
                    xs.Serialize(context.HttpContext.Response.Output, ObjectToSerialize);
                }
           
        }
    }
}