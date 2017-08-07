using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Exceptions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly"), Serializable]
    public class AppException : Exception
    {
        public AppException( string message)
        : base(string.Format(CultureInfo.InvariantCulture, "Message: {0}", message))
        {
            AppMessage = message;
        }


        public AppException(int code, string message, Exception ex)
        : base(string.Format(CultureInfo.InvariantCulture, "Message: {0}", message), ex)
        {

            AppMessage = message;
        }



       // public int AppCode { get; set; }
        public string AppMessage { get; set; }
    }
}
