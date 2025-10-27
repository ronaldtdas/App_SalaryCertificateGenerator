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
			btnSave = new Button();
			btnPrint = new Button();
			rv = new Microsoft.Reporting.WinForms.ReportViewer();
			SuspendLayout();
			// 
			// btnSave
			// 
			btnSave.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSave.Location = new Point(680, 744);
			btnSave.Name = "btnSave";
			btnSave.Size = new Size(107, 49);
			btnSave.TabIndex = 1;
			btnSave.Text = "Save PDF";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Visible = false;
			btnSave.Click += btnSave_Click;
			// 
			// btnPrint
			// 
			btnPrint.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnPrint.Location = new Point(551, 744);
			btnPrint.Name = "btnPrint";
			btnPrint.Size = new Size(107, 49);
			btnPrint.TabIndex = 2;
			btnPrint.Text = "Print";
			btnPrint.UseVisualStyleBackColor = true;
			btnPrint.Visible = false;
			btnPrint.Click += btnPrint_Click;
			// 
			// rv
			// 
			rv.Location = new Point(12, 12);
			rv.Name = "ReportViewer";
			rv.ServerReport.BearerToken = null;
			rv.Size = new Size(784, 1000);
			rv.TabIndex = 0;
			// 
			// frmReportViewer
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(799, 1030);
			Controls.Add(rv);
			Controls.Add(btnSave);
			Controls.Add(btnPrint);
			Name = "frmReportViewer";
			Text = "Salary Certificate Report";
			Load += frmReportViewer_Load;
			ResumeLayout(false);
		}

		#endregion

		private Button btnSave;
		private Button btnPrint;
		private Microsoft.Reporting.WinForms.ReportViewer rv;
	}
}