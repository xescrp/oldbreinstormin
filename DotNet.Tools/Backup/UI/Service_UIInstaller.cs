using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DotNet.Tools.UI
{
    public partial class Service_UIInstaller : Form
    {
        public Service_UIInstaller()
        {
            InitializeComponent();
        }

        private void ckuser_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                txtPass.Enabled = ckuser.Checked;
                txtPass.ReadOnly = !ckuser.Checked;
                txtUser.Enabled = ckuser.Checked;
                txtUser.ReadOnly = !ckuser.Checked;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Service_UIInstaller_Load(object sender, EventArgs e)
        {
            try
            {
                txtPass.Enabled = ckuser.Checked;
                txtPass.ReadOnly = !ckuser.Checked;
                txtUser.Enabled = ckuser.Checked;
                txtUser.ReadOnly = !ckuser.Checked;
                _load_start_modes();
                cbStartmode.SelectedItem = cbStartmode.Items[0];
            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPathService.Text.StartsWith(@"\\"))
                {
                    throw new
                        Exception("No se permiten rutas UNC para la instalacion del servicio. Especifique la ruta local del servicio");
                }
                if (txtServicename.Text == null || txtServicename.Text == "") 
                {
                    throw new Exception("Especifique un nombre para el servicio");
                }
                if (txtServer.Text == null || txtServer.Text == "") 
                {
                    throw new Exception("Especifique el nombre del servidor");
                }

                string mode = cbStartmode.SelectedItem.ToString();

                WIN32.Win32API.ServiceStartMode st = 
                    (WIN32.Win32API.ServiceStartMode)Enum.Parse(typeof(WIN32.Win32API.ServiceStartMode), mode);

                WIN32.Win32API.ServiceReturnCode rt = new DotNet.Tools.WIN32.Win32API.ServiceReturnCode();

                if (ckuser.Checked)
                {
                    WIN32.Win32_Service sv = new DotNet.Tools.WIN32.Win32_Service(txtServer.Text);
                    rt = sv.Install(txtServer.Text, txtServicename.Text,
                        txtServicename.Text, txtPathService.Text,
                        st, txtUser.Text, txtPass.Text, null);
                }
                else 
                {
                    WIN32.Win32_Service sv = new DotNet.Tools.WIN32.Win32_Service(txtServer.Text);
                    rt = sv.Install(txtServer.Text, txtServicename.Text,
                        txtServicename.Text, txtPathService.Text,
                        st, null, null, null);
                }

                if (rt != null) { MessageBox.Show("Resultado: " + rt.ToString()); }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _load_start_modes() 
        {
            string[] modes = Enum.GetNames(typeof(WIN32.Win32API.ServiceStartMode));
            if (modes != null && modes.Length > 0) 
            {
                foreach (string mode in modes) 
                {
                    cbStartmode.Items.Add(mode);
                }
            }
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
