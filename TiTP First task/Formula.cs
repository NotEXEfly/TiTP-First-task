using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiTP_First_task
{
    public class Formula
    {
        private DataGridView _dataGrid;

        private BindingList<Container> DataList { get; set; }

        private double _startX;
        private double _endX;
        private double _deltaX;
        private int _num;
        private int deltaXRoundLength;

        public Formula(DataGridView DataGrid, int numVariant)
        {
            _dataGrid = DataGrid;
            _num = numVariant;
            DataList = new BindingList<Container>();
        }

        public async Task Calculate(double startX, double endX, double deltaX)
        {
            _startX = startX;
            _endX = endX;
            _deltaX = deltaX;

            DeltaXRoundInit();

            _dataGrid.Rows.Clear();

            int currentIteration = 0;

            await Task.Run(() => {
                if (_deltaX > 0)
                {
                    for (double _currentX = _startX; _currentX <= _endX; _currentX += _deltaX, currentIteration++)
                    {
                        var decision = GetDecision(_currentX);
                        DataList.Add(new Container(currentIteration, Math.Round(_currentX, deltaXRoundLength), decision.Item1, decision.Item2)); 

                        if (currentIteration >= 5000000) return;
                    }
                }
                else
                {
                    for (double _currentX = _startX; _currentX >= _endX; _currentX += _deltaX, currentIteration++)
                    {
                        var decision = GetDecision(_currentX);
                        DataList.Add(new Container(currentIteration, Math.Round(_currentX, deltaXRoundLength), decision.Item1, decision.Item2));

                        if (currentIteration >= 5000000) return;
                    }
                }
            });
 
        }

        public void DrawTable()
        {
            _dataGrid.DataSource = new BindingList<Container>(DataList);
        } 

        // ------------------- Main task decision ---------------------
        private (double, bool) GetDecision(double x)
        {
            if (x == 0)
                return (0, false);

            double x1 = f1(x);
            double x2 = f2(x);
            double result = Math.Max(x1, x2);


            if (double.IsNaN(result))
                return (double.NaN, false);
            else
                return (result, true);
        }

        private double f1(double x)
        {
            return Math.Log((1 - _num) / Math.Sin(x + _num));
        }

        // ctg = 1 / tg(x)
        private double f2(double x)
        {
            return Math.Abs(1 / Math.Tan(x) / _num);
        }

        //--------------------------------------------------------------

        /// <summary>
        /// Init deltaXRoundLength,
        /// fix floating point number in double
        /// </summary>
        private void DeltaXRoundInit()
        {
            string deltaXstr = _deltaX.ToString();

            if (deltaXstr.Contains(","))
            {
                deltaXRoundLength = deltaXstr.Split(new char[] { ',' })[1].Length;
            }
            else if (deltaXstr.Contains("1E-"))
            {
                if (deltaXstr.Length == 5)  //example 1E-05
                {
                    bool parseOk = int.TryParse(deltaXstr.Split(new char[] { '-' })[1], out int result);
                    deltaXRoundLength = parseOk ? result : 1;
                }
                else                        //example -1E-05
                {
                    bool parseOk = int.TryParse(deltaXstr.Split(new char[] { '-' })[2], out int result);
                    if (parseOk && result < 15)
                    {
                        deltaXRoundLength = result;
                    }
                    else
                    {
                        deltaXRoundLength = 1;
                    }
                }
            }
            else
            {
                deltaXRoundLength = 1;
            }
        }
    }
}
