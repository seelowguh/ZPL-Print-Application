using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//  Calls in the functions from our libraries
using SchraderElectronics.SEL_InterfaceLibrary;

namespace Print_Application
{
    public partial class frmPrintApplication : Form
    {
        public frmPrintApplication()
        {
            InitializeComponent();
            //If you need something to happen before the form loads - put it here
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //  Print function
            long _asidID = 0;
            try
            {
                //  Get the asic id from the valvebody id passed in
                _asidID = GetAsicID(txtVBID.Text);

                if (_asidID == 0)
                {
                    //  No asic ID
                    throw new Exception(string.Format("Error receiving ASICID for VBID:{0}", txtVBID.Text));
                }

                //  Builds the ZPL string
                string _zpl = BuildZPL(txtVBID.Text, _asidID);
                //  insert printer name
                string _printer = "ZPL Printer Name";

                //  Call print function
                if (!PrintLabel(_zpl, _printer))
                {
                    //  If false - something went wrong
                    throw new Exception(string.Format("Error with printing for VBID:{0}", txtVBID.Text));
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmPrintApplication_Load(object sender, EventArgs e)
        {
            //  Need something to run on load? put it here
        }

        private bool PrintLabel(string _zpl, string _printer)
        {
            clsRawPrinterHelper.SendStringToPrinter(_printer, _zpl);
            return true;
        }

        private long GetAsicID(string _vbid)
        {
            //  Initialise variables - Insert 
            string _ssql = string.Format("", _vbid);
            long _response = 0;
            SqlConnection _connection = null;
            SqlCommand _command = null;
            SqlDataReader _dataReader = null;

            try
            {
                //  Assign the connection to the central SQL Connection which pulls the connection string from the local XML configurataion (app.config)
                using (_connection = clsDataConnections.GetSQLConnection(clsDataConnections.Connection.Central))
                {
                    //Opens the connection
                    _connection.Open();
                    //  Create the command - can be reader, update, insert, etc.
                    using (_command = new SqlCommand(_ssql, _connection))
                    {
                        //  Create the reader to pull out the data
                        _dataReader = _command.ExecuteReader();
                        while (_dataReader.Read())
                        {
                            if (!_dataReader.HasRows)
                            {
                                //  When the query has no rows, throw this.
                                throw new Exception(String.Format("Query provides no rows for VBID:{0}", _vbid));
                            }
                            else
                            {
                                //  Gets the first value as an Int32 
                                _response = _dataReader.GetInt32(0);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //  throw up a level
                throw ex;
            }
            catch (Exception ex)
            {
                //  throw up a level
                throw ex;
            }
            finally
            {
                //  clean up after yourself
                _dataReader.Close();
                _dataReader.Dispose();
                _command.Dispose();
                _connection.Close();
                _connection.Dispose();
            }

            //  Return what you sent
            return _response;
        }

        private string BuildZPL(string _vbid, long _asic)
        {
            //Build up the ZPL here - insert as needed
            return string.Format("", _vbid, _asic);
        }
    }
}
