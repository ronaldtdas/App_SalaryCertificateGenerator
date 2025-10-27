namespace App_SalaryCertificate
{
	partial class frmReportViewer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			rv = new Microsoft.Reporting.WinForms.ReportViewer();
			containerPanel = new Panel();
			containerPanel.SuspendLayout();
			SuspendLayout();
			// 
			// containerPanel
			// 
			containerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			containerPanel.AutoScroll = true;
			containerPanel.Controls.Add(rv);
			containerPanel.Location = new Point(0, 0);
			containerPanel.Name = "containerPanel";
			containerPanel.Size = new Size(1081, 1065);
			containerPanel.TabIndex = 1;
			// 
			// rv
			// 
			rv.Location = new Point(0, 0);
			rv.Name = "ReportViewer";
			rv.ServerReport.BearerToken = null;
			rv.Size = new Size(799, 1030);
			rv.TabIndex = 0;
			// 
			// frmReportViewer
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1081, 1065);
			Controls.Add(containerPanel);
			Name = "frmReportViewer";
			Text = "Salary Certificate Report";
			WindowState = FormWindowState.Maximized;
			Load += frmReportViewer_Load;
			containerPanel.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private Microsoft.Reporting.WinForms.ReportViewer rv;
		private Panel containerPanel;
	}
}