using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation_Master_3000 {
    public class SimulationException : Exception {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SimulationException() {
        }

        public SimulationException(string message) : base(message)
        {
        }

        public SimulationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SimulationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
