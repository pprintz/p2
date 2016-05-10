using System;
using System.Runtime.Serialization;

namespace Evacuation_Master_3000.UI.ControlPanelUI
{
    public class GeneralInternalException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public GeneralInternalException()
        {
        }

        public GeneralInternalException(string message) : base(message)
        {
        }

        public GeneralInternalException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GeneralInternalException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}