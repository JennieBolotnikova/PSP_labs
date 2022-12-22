using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ThreadData
    {
        public ThreadData(string? Address, int Port, string? MatrixForSolve) 
        {
            this.Address = Address;
            this.Port = Port;
            this.MatrixForSolve = MatrixForSolve;
        }
        public string? Address { get; set; }
        public int Port { get; set; }
        public string? MatrixForSolve { get; set; }
    }
}
