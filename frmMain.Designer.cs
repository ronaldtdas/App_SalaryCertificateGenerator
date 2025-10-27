namespace App_SalaryCertificate
{
	partial class frmMain
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
			dateTimePicker1 = new DateTimePicker();
			btnSave = new Button();
			dataGridView1 = new DataGridView();
			SuspendLayout();
			// 
			// dateTimePicker1
			// 
			dateTimePicker1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dateTimePicker1.Location = new Point(12, 12);
			dateTimePicker1.Name = "dateTimePicker1";
			dateTimePicker1.Size = new Size(157, 29);
			dateTimePicker1.TabIndex = 0;
			// 
			// btnSave
			// 
			btnSave.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnSave.Location = new Point(689, 12);
			btnSave.Name = "btnSave";
			btnSave.Size = new Size(107, 29);
			btnSave.TabIndex = 1;
			btnSave.Text = "Import CSV";
			btnSave.UseVisualStyleBackColor = true;
			btnSave.Click += btnSave_Click;
			// 
			// dataGridView1
			// 
			dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.Location = new Point(12, 50);
			dataGridView1.Name = "dataGridView1";
			dataGridView1.Size = new Size(784, 385);
			dataGridView1.TabIndex = 2;
			// 
			// frmMain
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(808, 450);
			Controls.Add(dataGridView1);
			Controls.Add(btnSave);
			Controls.Add(dateTimePicker1);
			Name = "frmMain";
			Text = "Salary Certificate Generator - Import Employees";
			Load += frmMain_Load;
			ResumeLayout(false);
		}

		#endregion

		private DateTimePicker dateTimePicker1;
		private Button btnSave;
		private DataGridView dataGridView1;
	}
}