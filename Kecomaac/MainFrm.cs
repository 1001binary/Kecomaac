using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static Kecomaac.Win32Cursor;

namespace Kecomaac
{
    public partial class MainFrm : Form
    {
        private static bool _isExecuting = false;
        private delegate void StopDelegate();

        public MainFrm()
        {
            InitializeComponent();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result?.ToString() == "Stopped")
            {
                StopExecuting();
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            StartExecuting(e);
        }

        private void StartExecuting(DoWorkEventArgs e)
        {
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;

            int minX = 0, maxX = width;
            int minY = 0, maxY = height;
            Random rand = new Random();

            _isExecuting = true;
            btnStart.Text = "Executing...";
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            NativeKernelMethods.SetThreadExecutionState(NativeKernelMethods.EXECUTION_STATE.ES_CONTINUOUS |
                                        NativeKernelMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                                        NativeKernelMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                                        NativeKernelMethods.EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
            while (_isExecuting)
            {
                POINT p = new POINT(rand.Next(minX, maxX), rand.Next(minY, maxY));

                Win32Cursor.ClientToScreen(this.Handle, ref p);
                Win32Cursor.SetCursorPos(p.x, p.y);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            e.Result = "Stopped";
        }

        private void StopExecuting()
        {
            _isExecuting = false;
            btnStart.Text = "Start";
            btnStop.Text = "Stop";
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            worker.RunWorkerAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _isExecuting = false;
            btnStop.Invoke(new StopDelegate(() =>
            {
                btnStop.Enabled = false;
                btnStop.Text = "Stopping...";
            }));
        }

        private void linkLabelDrawIO_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://draw.io");
        }
    }
}
