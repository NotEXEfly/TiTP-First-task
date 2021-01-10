using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TiTP_First_task
{
    public partial class Form1 : Form
    {
        private readonly int Num = 54;
        private Formula _formula;

        public Form1()
        {
            InitializeComponent();

            MaximizeBox = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;

            textBox1.MaxLength = 15;
            textBox2.MaxLength = 15;
            textBox3.MaxLength = 15;

            this.MouseDown += new MouseEventHandler((o, e) =>
            {
                Capture = false;
                Message m = Message.Create(base.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
                WndProc(ref m);
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _formula = new Formula(dataGridView1, Num);
            label4.Text += Num;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool startXCheck = Validate(textBox1.Text, out double startX);
            bool endXCheck = Validate(textBox2.Text, out double endX);
            bool deltaXCheck = Validate(textBox3.Text, out double deltaX);

            dataGridView1.Rows.Clear();

            if (startXCheck && endXCheck && deltaXCheck)
            {
                if (!CheckInputValues(startX, endX, deltaX))
                {
                    UiEnable(true);
                    return;
                } 

                UiEnable(false);   
                Calculate(startX, endX, deltaX);
            }
            else
            {
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                MessageBox.Show("Переданы неверные значения!");
            }
        }

        private async void Calculate(double startX, double endX, double deltaX)
        {
            label5.Text = "Calculating...";

            await _formula.Calculate(startX, endX, deltaX);

            label5.Text = "Print raws...";
            this.Refresh();

            _formula.DrawTable();

            label5.Text = "";
            UiEnable(true);
        }

        /// <summary>
        /// Change double.TryParse bad return to NaN
        /// </summary>
        private bool Validate(string str, out double result)
        {
            if (double.TryParse(str, out double res))
            {
                result = res;
                return true;
            }
            else 
            {
                result = double.NaN;
                return false;
            }
        }

        private bool CheckInputValues(double startX, double endX, double deltaX)
        {
            if (deltaX == 0)
            {
                MessageBox.Show("deltaX не может содержать значение 0.");
                return false;
            }
            else if (deltaX > 0 && (startX > endX) || deltaX < 0 && (startX < endX))
            {
                MessageBox.Show("Значение startX не может превышать значение endX при положительном deltaX;\nЗначение endX не может превышать значение startX при отрицательном deltaX.");
                return false;
            }
            
            return true; 
        }

        /// <summary>
        /// Switch user ui enable
        /// </summary>
        private void UiEnable(bool value)
        {
            textBox1.Enabled = value;
            textBox2.Enabled = value;
            textBox3.Enabled = value;
            button1.Enabled = value;
            pictureBox1.Visible = !value;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;
            if (dataGridView1.Columns[e.ColumnIndex].Name == "status")
            {
                if ((bool)e.Value)
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                    e.Value = "Ok";
                }
                else
                {
                    e.CellStyle.BackColor = Color.LightPink;
                    e.Value = "Error";
                }
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "result")
            {
                if (e.Value is double.NaN || (double)e.Value == 0)
                {
                    e.Value = "-";
                }
            }
        }
        /// <summary>
        /// Check textBox input on correct symbols: digits, BackSpace, comma, minus
        /// </summary>
        private void textBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && !new int[] { 8, 45, 44 }.Contains(e.KeyChar) ) 
            {
                e.Handled = true;
            }
        }

    }
}
