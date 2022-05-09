using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MC_authNET.Utils.Extensions;

namespace MC_authNET.Protocol.Util
{
    class ErrorHandler
    {
        private Queue<ErrorMessage> errors = new Queue<ErrorMessage>();

        private bool Enabled;

        public ErrorHandler()
        {
            Enabled = true;
        }

        public void Add(ErrorMessage error)
        {
            errors.Enqueue(error);
        }
        
        public ErrorMessage GetMessage()
        {
            return errors.Dequeue();
        }

        public ErrorMessage Get()
        {
            return errors.Peek();
        }


        public void Clear()
        {
            errors.Clear();
        }

        public void DispayError()
        {

            if (Enabled && errors.Count > 0)
            {     
                foreach (ErrorMessage error in errors)
                {
                    ConsoleMore.WriteMessage($"{error.context} => {error.message}", MessageType.textLine, error.Color);
                }

                errors.Clear();
                //Environment.Exit(1);
                                  
            }
        }
    }
}
