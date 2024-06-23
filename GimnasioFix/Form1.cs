using GimnasioFix.Dato;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using System.Threading;
using System.Drawing.Design;
using MySqlX.XDevAPI.Common;
using System.Data.SqlClient;
using ZXing.QrCode.Internal;

namespace GimnasioFix
{

    public partial class Form1 : Form
    {
        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;
        public bool cooldown = false;

        private DataTable table;
        private DataTable table2;
        ClienteAdmin admin = new ClienteAdmin();
        DateTime saved = DateTime.Now;
        int count = 1;
        string mysqlcon = "server=127.0.0.1;user=root;database=gimnasio;password=; convert zero datetime=True";
        private void Inicializar()
        {
            table = new DataTable();

            table.Columns.Add("Nombre");
            table.Columns.Add("Apellido");
            table.Columns.Add("Telefono");
            table.Columns.Add("Numero de cuenta");
            dataGridView_Clients.DataSource = table;


        }

        private void InicializarPay()
        {
            table2 = new DataTable();

            table2.Columns.Add("Numero de cuenta");
            table2.Columns.Add("Fecha limite");
            table2.Columns.Add("Precio");
            table2.Columns.Add("Saldo restante");
            table2.Columns.Add("Tipo de subscripcion");
            dataGridView_Pagos.DataSource = table2;
        }
        /// <summary>
        /// Se llama al inicio del formulario, actualiza cualquier pago que necesite actualizarce.
        /// Desactiva subscripciones que ya no estan vigentes por saldo faltante.
        /// Lo cual hace que se trabe un poco al inicio.
        /// </summary>
        private void UpdateAllListings()
        {

        }
        /// <summary>
        /// Saca todos los datos necesarios en cuanto se abre la app
        /// Inicializa la tabla de usuarios.
        ///
        /// </summary>
        private void FirstConsultAcc()
        {
            List<Cliente> values = new List<Cliente>();
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlcon);
            try
            {
                string sql = "";
                mySqlConnection.Open();
                Console.WriteLine("Connection added");
                sql = "SELECT TOP 20 * FROM clientes WHERE Activo = 1";
                MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("11");
                    Cliente cliente = new Cliente();
                    cliente.AccNum = reader["AccNum"].ToString();
                    cliente.Name = reader["Name"].ToString();
                    cliente.LastName = reader["LastName"].ToString();
                    cliente.Tel = reader["Tel"].ToString();
                    values.Add(cliente);
                }


            }
            catch (Exception ex)
            {

            }
            finally
            {
                mySqlConnection.Close();
            }
            Inicializar();
            Console.Write(values.ToString());
            Console.Write("aaaa");
            foreach (var item in values)
            {
                DataRow row = table.NewRow();
                row["Nombre"] = item.Name;
                row["Apellido"] = item.LastName;
                row["Telefono"] = item.Tel;
                row["Numero de cuenta"] = item.AccNum.ToString();
                table.Rows.Add(row);

            }
        }
            /// <summary>
            /// Saca todos los datos necesarios en cuanto se abre la app
            /// Inicializa la tabla de pagos.
            /// </summary>
        private void FirstConsultPay()
        {

        }

        public Form1()
        {
            InitializeComponent();
            UpdateAllListings();
            Inicializar();
            InicializarPay();
            FirstConsultAcc();
            FirstConsultPay();
            ConsultarTabla();
        }


        /// <summary>
        /// Esta hace la consulta para la tabla de cuentas registradas
        /// </summary>
        private void ConsultarTabla()
        {
            List<Cliente> values = new List<Cliente>();
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlcon);
            try
            {
                string sql = "";
                mySqlConnection.Open();
                Console.WriteLine("Connection added");
                if (txtSearch.Text.Length == 0)
                {
                    sql = "SELECT * FROM clientes WHERE Activo = 1";
                }
                else
                {
                    string selected = "";
                    string finisher = "";
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            selected = "AccNum = ";
                            break;
                        case 1:
                            selected = "Name = '";
                            finisher = "'";
                            break;
                        case 2:
                            selected = "LastName = '";
                            finisher = "'";
                            break;
                        case 3:
                            selected = "Tel = ";
                            break;
                        default:
                            selected = "AccNum = ";
                            break;
                    }

                    sql = "SELECT * FROM clientes WHERE Activo = 1 AND " + selected + txtSearch.Text + finisher;
                }
                MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("11");
                    Cliente cliente = new Cliente();
                    cliente.AccNum = reader["AccNum"].ToString();
                    cliente.Name = reader["Name"].ToString();
                    cliente.LastName = reader["LastName"].ToString();
                    cliente.Tel = reader["Tel"].ToString();
                    //cliente.FechaFin = DateTime.Parse(reader["FechaFin"].ToString());
                    //cliente.FechaInicio = DateTime.Parse(reader["FechaInicio"].ToString());
                    values.Add(cliente);
                }


            }
            catch (Exception ex)
            {

            }
            finally
            {
                mySqlConnection.Close();
            }
            Inicializar();
            Console.Write(values.ToString());
            Console.Write("aaaa");
            foreach (var item in values)
            {
                DataRow row = table.NewRow();
                row["Nombre"] = item.Name;
                row["Apellido"] = item.LastName;
                row["Telefono"] = item.Tel;
                row["Numero de cuenta"] = item.AccNum.ToString();
                table.Rows.Add(row);


            }
        
        }

        private void Guardar()
        {
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlcon);
            int newid = 0;
            try
            {
                mySqlConnection.Open();
                string sql = "SELECT MAX(Id) FROM Clientes";
                MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
                var reader = cmd.ExecuteReader();
                while(reader.Read()){
                    newid = Int32.Parse(reader["Id"].ToString());
                }
            }
            catch
            {

            }
            finally
            {

            }
            DateTime rn = DateTime.Now;
            string numAcc = (rn.Year - 2000).ToString() + rn.Month.ToString() + rn.Day.ToString() + (newid+1);
            Console.WriteLine(numAcc);
            Cliente model = new Cliente()
            {
                Name = txtName.Text,
                LastName = txtLastName.Text,
                AccNum = numAcc.ToString(),
                Tel = textBox1.Text,
            };
            
            try
            {
                mySqlConnection.Open();
                /*if (model.FechaInicio.Month < 10)
                    monthini = "0" + model.FechaInicio.Month.ToString();
                else
                    monthini = model.FechaInicio.Month.ToString();
                if (model.FechaFin.Month < 10)
                    monthfin = "0" + model.FechaFin.Month.ToString();
                else
                    monthfin = model.FechaFin.Month.ToString();
                if (model.FechaInicio.Day < 10)
                    dayini = "0" + model.FechaInicio.Day.ToString();
                else
                    dayini = model.FechaInicio.Day.ToString();
                if (model.FechaFin.Day < 10)
                    dayfin = "0" + model.FechaInicio.Day.ToString();
                else
                    dayfin = model.FechaInicio.Month.ToString();

                string fechini = model.FechaInicio.Year.ToString() + "-" + monthini + "-" + dayini;
                string fechfin = model.FechaFin.Year.ToString() + "-" + monthfin + "-" + dayfin;*/
                string sql = "INSERT INTO clientes (`Id`, `Name`, `LastName`, `Activo`, `Imagen`, `Tel`, `AccNum`) VALUES (NULL,'" + model.Name + "', '"+model.LastName+"','1', NULL,'"+model.Tel.ToString()+ "', '" + model.AccNum + "');";
                MySqlCommand cmd2 = new MySqlCommand(sql, mySqlConnection);
                cmd2.ExecuteNonQuery();
                Zen.Barcode.CodeQrBarcodeDraw qrcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;
                pictureBox3.Image = qrcode.Draw(model.AccNum.ToString(), 150);
                lblResultName.Text = model.Name.ToString();
                lblResultAcc.Text = model.AccNum.ToString();
                var await = TriggerCooldownRegister(30000);
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.Message);
            }
            finally
            {
                mySqlConnection.Close();
            }

            ConsultarTabla();
        }

        private void labelDatePaid_Click(object sender, EventArgs e)
        {

        }

        private void dateTime_paid_ValueChanged(object sender, EventArgs e)
        {

        }

        private void labelName_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveClient_Click(object sender, EventArgs e)
        {
            {
                bool check = false;

                if (txtName.Text == "")
                {
                    check = true;
                }
                if (txtLastName.Text == "")
                {
                    check = true;
                }
                if (textBox1.Text == "")
                {
                    check = true;
                }

                if (!check)
                {
                    Guardar();
                    Clean();
                }
                else
                {
                    MessageBox.Show("Rellena todos los campos", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Clean()
        {
            txtLastName.Text = "";
            txtName.Text = "";
            textBox1.Text = "";
        }

        private void label2_Click(object sender, EventArgs e)
        {

            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                comboBox2.Items.Add(Device.Name);
            }

            comboBox2.SelectedIndex = 0;
            FinalFrame = new VideoCaptureDevice();
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox2.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();
            Thread.Sleep(1000);
            timer1.Start();
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs e)
        {
            pictureBox2.Image = (Bitmap)e.Frame.Clone();
        }


        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void AddVisit(string AccNum, string Name = null)
        {
            List<Cliente> values = new List<Cliente>();
            List<Payment> subs = new List<Payment>();
            MySqlConnection mySqlConnection = new MySqlConnection(mysqlcon);
            try
            {
                string sql = "SELECT * FROM clientes WHERE AccNum = " + AccNum;
                mySqlConnection.Open();

                MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Cliente cliente = new Cliente();
                    
                    cliente.Name = reader["Name"].ToString();
                    if (Name == null)
                        Name = cliente.Name;
                    values.Add(cliente);
                }
                if(values.Count > 0)
                {
                    if (AccNum == "00000000")
                    {
                        mySqlConnection.Close();
                        mySqlConnection.Open();
                        sql = "INSERT INTO entradas (Id,FechaHora,AccNum,Nombre) VALUES (NULL, current_timestamp(), '" + AccNum + "', '" + Name + "');";
                        MySqlCommand cmd2 = new MySqlCommand(sql, mySqlConnection);
                        cmd2.ExecuteNonQuery();
                        lblName.Text = "Bienvenid@ " + Name;
                        lblTime.Text = "Pase de día";
                        lblDay.Text = "Feliz entrenamiento";
                        var wait = TriggerCooldownLogin(15000);
                    }
                    else
                    {
                        sql = "SELECT * FROM pagos WHERE AccNum = " + AccNum;
                        mySqlConnection.Close();
                        mySqlConnection.Open();
                        MySqlCommand cmd2 = new MySqlCommand(sql, mySqlConnection);
                        var reader2 = cmd2.ExecuteReader();
                        while (reader2.Read())
                        {
                            Payment payed = new Payment();

                            payed.LastDate = DateTime.Parse(reader["FechaFin"].ToString());
                            subs.Add(payed);
                        }
                        if(subs.Count > 0)
                        {
                            int DaysLeft = (subs[0].LastDate - DateTime.Now).Days;
                            if (DaysLeft >= 0)
                            {
                                string Date = subs[0].LastDate.Date.ToString();
                                lblTime.Text = "Fecha final de la subscripcion: " + Date;
                                if (DaysLeft > 7)
                                {
                                    lblDay.Text = "Feliz Entrenamiento";
                                }
                                else
                                    lblDay.Text = "Quedan " + DaysLeft.ToString() + " días restantes";
                                mySqlConnection.Close();
                                mySqlConnection.Open();
                                sql = "INSERT INTO entradas (Id,FechaHora,AccNum,Nombre) VALUES (NULL, current_timestamp(), '" + AccNum + "', '" + Name + "');";
                                MySqlCommand cmd3 = new MySqlCommand(sql, mySqlConnection);
                                cmd3.ExecuteNonQuery();
                                var wait = TriggerCooldownLogin(15000);
                            }
                            else
                            {
                                MessageBox.Show("La subscripción se a acabado, favor de renovar");
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontro una subscripción asociada a esta cuenta");
                        }

                    }

                }
                else
                {
                    MessageBox.Show("No se encontro el número de cuenta");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ZXing.Windows.Compatibility.BarcodeReader reader = new ZXing.Windows.Compatibility.BarcodeReader();
            ZXing.Result result = reader.Decode((Bitmap)pictureBox2.Image);
            if (result != null && !cooldown)
            {
                try
                {
                    string decode = result.ToString().Trim();
                    if (decode != "" && IsDigitsOnly(decode))
                    {
                        textBox2.Text = decode;
                        AddVisit(decode);
                        cooldown = true;
                        var wait = TriggerCooldown(500);

                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        async Task TriggerCooldown(int milisecond)
        {
            textBox3.Text = "";
            textBox2.Text = "";
            await Task.Delay(milisecond);
            cooldown = false;
            
        }

        async Task TriggerCooldownLogin(int milisecond)
        {
            await Task.Delay(milisecond);
            cooldown = false;
            lblName.Text = "Bienvenid@ a X Gimnasio";
            lblDay.Text = "Muestra tu QR o ingresa tu número de cuenta";
            lblDay.Text = "O consigue un acceso por día";
        }

        async Task TriggerCooldownRegister(int milisecond)
        {
            await Task.Delay(milisecond);
            cooldown = false;
            lblResultName.Text = "";
            lblResultFecha.Text = "";
            lblResultAcc.Text = "";
            pictureBox3.Image = null;
        }


        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FinalFrame.Stop();
            timer1.Stop();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox2.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ConsultarTabla();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string decode = textBox2.Text.ToString().Trim();
            if (decode != "" && IsDigitsOnly(decode))
            {
                textBox2.Text = decode;
                AddVisit(decode);
                cooldown = true;
                var wait = TriggerCooldown(500);

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

                AddVisit("00000000", textBox3.Text);
                cooldown = true;
                var wait = TriggerCooldown(500);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dateTime_toPay.Value = dateTime_paid.Value.AddDays(7);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTime_toPay.Value = dateTime_paid.Value.AddDays(15);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dateTime_toPay.Value = dateTime_paid.Value.AddMonths(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dateTime_paid.Value = DateTime.Now;
        }

        private void lblResultAcc_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage_clientDetail_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
