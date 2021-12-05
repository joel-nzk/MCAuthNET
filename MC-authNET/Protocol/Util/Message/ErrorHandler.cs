using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                    Console.ForegroundColor = error.Color;
                    Console.WriteLine($"{error.context} => {error.message}");
                }

                errors.Clear();
                Console.ForegroundColor = ConsoleColor.Gray;
                Environment.Exit(1);
                                  
            }
        }
    }
}
