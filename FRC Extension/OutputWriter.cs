﻿using System;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace RobotDotNet.FRC_Extension
{
    /// <summary>
    /// This class allows us to create an output windows that the RoboRIO connector can interface with.
    /// </summary>
    internal class OutputWriter : IProgress<int>
    {
        private Window m_window = null;
        private IVsOutputWindowPane m_outputPane = null;
        private IVsStatusbar m_statusBar = null;

        private bool m_initialized = false;

        private static OutputWriter s_instance;

        public static OutputWriter Instance
        {
            get { return s_instance ?? (s_instance = new OutputWriter()); }
        }

        private OutputWriter()
        {

        }

        private void Initialize()
        {
            if (m_initialized)
                return;
            DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            m_window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);

            if (m_window == null)
                return;

            IVsOutputWindow outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            if (outputWindow == null)
                return;
            Guid paneGuid = VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            outputWindow.CreatePane(ref paneGuid, "FRC", 1, 0);
            outputWindow.GetPane(ref paneGuid, out m_outputPane);

            if (m_outputPane == null)
                return;

            m_statusBar = ServiceProvider.GlobalProvider.GetService(typeof(SVsStatusbar)) as IVsStatusbar;

            if (m_statusBar == null)
                return;

            m_initialized = true;



        }

        public void Write(string value)
        {
            if (!m_initialized)
                Initialize();
            if (!m_initialized) return;
            m_window.Visible = true;
            m_outputPane.OutputStringThreadSafe(value);
            m_outputPane.Activate();
        }

        public void Write(int value)
        {
            Write(value.ToString());
        }

        public void Write(double value)
        {
            Write(value.ToString());
        }

        public void WriteLine(string value)
        {
            if (!m_initialized)
                Initialize();
            if (!m_initialized) return;
            m_window.Visible = true;
            m_outputPane.OutputStringThreadSafe(value + "\n");
            m_outputPane.Activate();
        }

        public void WriteLine(int value)
        {
            WriteLine(value.ToString());
        }

        public void WriteLine(double value)
        {
            WriteLine(value.ToString());
        }

        private uint cookie;
        private string m_progressLabel;

        public string ProgressBarLabel {
            get { return m_progressLabel; }
            set
            {
                m_progressLabel = value;
                if (!m_initialized)
                    Initialize();
                if (!m_initialized) return;
                m_statusBar.SetText(value);
            }
        }

        public void Report(int value)
        {
            if (!m_initialized)
                Initialize();
            if (!m_initialized) return;

            m_statusBar.Progress(ref cookie, 1, ProgressBarLabel, (uint)value, 100);
        }
    }
}
