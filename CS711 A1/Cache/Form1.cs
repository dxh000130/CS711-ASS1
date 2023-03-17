using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cache
{
    public partial class Form1 : Form
    {
        private CacheServer.CacheServer _cacheServer;
        
        public Form1()
        {
            InitializeComponent();
            _cacheServer = new CacheServer.CacheServer();
            // _cacheServer.LogCallback = Log;
            // _cacheServer.CacheStatusCallback = UpdateCacheStatus;
        }
        // private void btnStart_Click(object sender, EventArgs e)
        // {
        //     _cacheServer.Start();
        //     btnStart.Enabled = false;
        //     btnStop.Enabled = true;
        // }
        //
        // private void btnStop_Click(object sender, EventArgs e)
        // {
        //     _cacheServer.Stop();
        //     btnStart.Enabled = true;
        //     btnStop.Enabled = false;
        // }
        //
        // private void Log(string message)
        // {
        //     if (InvokeRequired)
        //     {
        //         Invoke(new Action<string>(Log), message);
        //         return;
        //     }
        //
        //     txtLog.AppendText(message + Environment.NewLine);
        // }
        //
        // private void UpdateCacheStatus(string status)
        // {
        //     if (InvokeRequired)
        //     {
        //         Invoke(new Action<string>(UpdateCacheStatus), status);
        //         return;
        //     }
        //
        //     lblCacheStatus.Text = status;
        // }
    }
}