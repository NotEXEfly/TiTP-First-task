using System;
using System.Linq;
using System.Text;

namespace TiTP_First_task
{
    public struct Container
    {
        public int RowNumber { get; set; }
        public double CurrentX { get; set; }
        public double Result { get; set; }
        public bool Status { get; set; }

        public Container(int rowNumber, double i, double result, bool status)
        {
            RowNumber = rowNumber;
            CurrentX = i;
            Result = result;
            Status = status;
        }
    }
}
