using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorMovieGrid
{
    public class InvalidActorArgumentException : System.Exception
    {

        private string _message;
        private Exception _exception;

        public override string Message { get { return this._message; } }
        public  Exception Exception { get { return this._exception; } }

        public InvalidActorArgumentException(string message)
        {
            this._message = message;
        }

        public InvalidActorArgumentException()
        {
           
        }
        public InvalidActorArgumentException(string message, Exception exception)
        {
            this._message = message;
            this._exception = exception;
            
        }
    }
}
