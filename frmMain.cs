using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace App_SalaryCertificate
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			// Load default date to today
			dateTimePicker1.Value = DateTime.Today;
		}

		// Helper function to convert numbers to words
		private string NumberToWords(decimal number)
		{
			string[] ones = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
			string[] teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
			string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
			string[] scales = { "", "Thousand", "Million", "Billion" };

			if (number == 0) return "Zero";

			long intPart = (long)number;
			long fractionalPart = (long)((number - intPart) * 100);

			if (intPart == 0 && fractionalPart == 0) return "Zero";

			StringBuilder words = new StringBuilder();
			int scaleIndex = 0;

			while (intPart > 0)
			{
				long remainder = intPart % 1000;
				if (remainder != 0)
				{
					words.Insert(0, " " + scales[scaleIndex]);
					words.Insert(0, ConvertThreeDigits((int)remainder));
				}
				intPart /= 1000;
				scaleIndex++;
			}

			string result = words.ToString().Trim();
			if (fractionalPart > 0)
			{
				result += " and " + fractionalPart.ToString("D2") + "/100";
			}

			return result;
		}

		private string ConvertThreeDigits(int number)
		{
			string[] ones = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
			string[] teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
			string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

			StringBuilder result = new StringBuilder();

			int hundreds = number / 100;
			int remainder = number % 100;

			if (hundreds > 0)
			{
				result.Append(ones[hundreds] + " Hundred");
			}

			if (remainder >= 20)
			{
				if (hundreds > 0) result.Append(" ");
				int tenDigit = remainder / 10;
				int oneDigit = remainder % 10;
				result.Append(tens[tenDigit]);
				if (oneDigit > 0)
					result.Append(" " + ones[oneDigit]);
			}
			else if (remainder >= 10)
			{
				if (hundreds > 0) result.Append(" ");
				result.Append(teens[remainder - 10]);
			}
			else if (remainder > 0)
			{
				if (hundreds > 0) result.Append(" ");
				result.Append(ones[remainder]);
			}

			return result.ToString();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
				using (OpenFileDialog openDialog = new OpenFileDialog())
				{
					openDialog.Filter = "CSV Files (*.csv)|*.csv|Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|All Files (*.*)|*.*";
					openDialog.Title = "Import Employee Data";

					if (openDialog.ShowDialog() == DialogResult.OK)
					{
						string filePath = openDialog.FileName;
						List<clsEmployee> employees = new List<clsEmployee>();

						if (filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
						{
							employees = ImportFromCSV(filePath);
						}
						else if (filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || 
								 filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
						{
							employees = ImportFromExcel(filePath);
						}

						if (employees.Count > 0)
						{
							// Populate DataGridView with employees
							PopulateEmployeeDataGrid(employees);
						}
						else
						{
							MessageBox.Show("No employee data found in the file.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Clipboard.SetText(ex.ToString());
				MessageBox.Show($"Error: {ex.Message}\n\n{ex.StackTrace}", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private List<clsEmployee> ImportFromCSV(string filePath)
		{
			List<clsEmployee> employees = new List<clsEmployee>();

			try
			{
				using (StreamReader reader = new StreamReader(filePath))
				{
					// Read header line
					string? headerLine = reader.ReadLine();
					if (headerLine == null) return employees;

					string[] headers = headerLine.Split(',');
					int employeeIdIndex = Array.IndexOf(headers, "EmployeeID");
					int salutationIndex = Array.IndexOf(headers, "Salutation");
					int firstNameIndex = Array.IndexOf(headers, "FirstName");
					int lastNameIndex = Array.IndexOf(headers, "LastName");
					int joiningDateIndex = Array.IndexOf(headers, "JoiningDate");
					int designationIndex = Array.IndexOf(headers, "Designation");
					int basicSalaryIndex = Array.IndexOf(headers, "BasicSalary");
					int houseAllowanceIndex = Array.IndexOf(headers, "HouseAllowance");
					int medicalAllowanceIndex = Array.IndexOf(headers, "MedicalAllowance");
					int travelAllowanceIndex = Array.IndexOf(headers, "TravelAllowance");
					int miscellaneousIndex = Array.IndexOf(headers, "Miscellaneous");
					int transportAllowanceIndex = Array.IndexOf(headers, "TransportAllowance");
					int performanceAllowanceIndex = Array.IndexOf(headers, "PerformanceAllowance");
					int monthlyTaxDeductionIndex = Array.IndexOf(headers, "MonthlyTaxDeduction");

					string? line;
					while ((line = reader.ReadLine()) != null)
					{
						if (string.IsNullOrWhiteSpace(line)) continue;

						string[] values = line.Split(',');
						if (values.Length == 0) continue;

						try
						{
							clsEmployee emp = new clsEmployee();

							if (employeeIdIndex >= 0 && employeeIdIndex < values.Length)
								emp.EmployeeID = int.Parse(values[employeeIdIndex].Trim());

							if (salutationIndex >= 0 && salutationIndex < values.Length)
								emp.Salutation = values[salutationIndex].Trim();

							if (firstNameIndex >= 0 && firstNameIndex < values.Length)
								emp.FirstName = values[firstNameIndex].Trim();

							if (lastNameIndex >= 0 && lastNameIndex < values.Length)
								emp.LastName = values[lastNameIndex].Trim();

							if (joiningDateIndex >= 0 && joiningDateIndex < values.Length)
								emp.JoiningDate = DateTime.ParseExact(values[joiningDateIndex].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

							if (designationIndex >= 0 && designationIndex < values.Length)
								emp.Designation = values[designationIndex].Trim();

							if (basicSalaryIndex >= 0 && basicSalaryIndex < values.Length)
								emp.BasicSalary = decimal.Parse(values[basicSalaryIndex].Trim());

							if (houseAllowanceIndex >= 0 && houseAllowanceIndex < values.Length)
								emp.HouseAllowance = decimal.Parse(values[houseAllowanceIndex].Trim());

							if (medicalAllowanceIndex >= 0 && medicalAllowanceIndex < values.Length)
								emp.MedicalAllowance = decimal.Parse(values[medicalAllowanceIndex].Trim());

							if (travelAllowanceIndex >= 0 && travelAllowanceIndex < values.Length)
								emp.TravelAllowance = decimal.Parse(values[travelAllowanceIndex].Trim());

							if (miscellaneousIndex >= 0 && miscellaneousIndex < values.Length)
								emp.Miscellaneous = decimal.Parse(values[miscellaneousIndex].Trim());

							if (transportAllowanceIndex >= 0 && transportAllowanceIndex < values.Length)
								emp.TransportAllowance = decimal.Parse(values[transportAllowanceIndex].Trim());

							if (performanceAllowanceIndex >= 0 && performanceAllowanceIndex < values.Length)
								emp.PerformanceAllowance = decimal.Parse(values[performanceAllowanceIndex].Trim());

							if (monthlyTaxDeductionIndex >= 0 && monthlyTaxDeductionIndex < values.Length)
								emp.MonthlyTaxDeduction = decimal.Parse(values[monthlyTaxDeductionIndex].Trim());

							emp.NetSalaryInWords = NumberToWords(emp.NetSalary);

							employees.Add(emp);
						}
						catch (Exception ex)
						{
							System.Diagnostics.Debug.WriteLine($"Error parsing row: {line}. Error: {ex.Message}");
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error reading CSV file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return employees;
		}

		private List<clsEmployee> ImportFromExcel(string filePath)
		{
			List<clsEmployee> employees = new List<clsEmployee>();

			try
			{
				// For Excel support, you'll need to install a NuGet package like EPPlus or ClosedXML
				// For now, showing a placeholder message
				MessageBox.Show("Excel import requires additional libraries. Please use CSV format or install an Excel library (EPPlus/ClosedXML).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
				
				// Alternative: You could also use OleDB connection for Excel files
				// This is a simplified version - for production, use EPPlus or ClosedXML
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error reading Excel file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return employees;
		}

		private void printPreviewDialog1_Load(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// Populate the DataGridView with employee data
		/// </summary>
		private void PopulateEmployeeDataGrid(List<clsEmployee> employees)
		{
			try
			{
				// Clear existing columns
				dataGridView1.Columns.Clear();
				dataGridView1.Rows.Clear();

				// Add columns
				dataGridView1.Columns.Add("EmployeeID", "Employee ID");
				dataGridView1.Columns.Add("Name", "Employee Name");
				dataGridView1.Columns.Add("Designation", "Designation");
				dataGridView1.Columns.Add("BasicSalary", "Basic Salary");

				// Add a button column for showing certificate
				DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
				buttonColumn.Name = "ShowCertificate";
				buttonColumn.HeaderText = "Action";
				buttonColumn.Text = "Show Certificate";
				buttonColumn.UseColumnTextForButtonValue = true;
				dataGridView1.Columns.Add(buttonColumn);

				// Set column widths
				dataGridView1.Columns["EmployeeID"].Width = 80;
				dataGridView1.Columns["Name"].Width = 150;
				dataGridView1.Columns["Designation"].Width = 150;
				dataGridView1.Columns["BasicSalary"].Width = 100;
				dataGridView1.Columns["ShowCertificate"].Width = 120;

				// Add rows
				foreach (var employee in employees)
				{
					string fullName = $"{employee.Salutation} {employee.FirstName} {employee.LastName}".Trim();
					dataGridView1.Rows.Add(employee.EmployeeID, fullName, employee.Designation, employee.BasicSalary);
				}

				// Store employees list as tag for later reference
				dataGridView1.Tag = employees;

				// Subscribe to cell click event
				dataGridView1.CellClick -= DataGridView1_CellClick;
				dataGridView1.CellClick += DataGridView1_CellClick;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error populating grid: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Handle click on the Show Certificate button
		/// </summary>
		private void DataGridView1_CellClick(object? sender, DataGridViewCellEventArgs e)
		{
			try
			{
				// Check if the clicked cell is in the ShowCertificate column
				if (e.ColumnIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "ShowCertificate" && e.RowIndex >= 0)
				{
					// Get the selected employee
					List<clsEmployee>? employees = dataGridView1.Tag as List<clsEmployee>;
					if (employees != null && e.RowIndex < employees.Count)
					{
						clsEmployee selectedEmployee = employees[e.RowIndex];
						DateTime certificateDate = dateTimePicker1.Value;

						// Open report viewer with the selected employee and certificate date
						frmReportViewer reportForm = new frmReportViewer(selectedEmployee, certificateDate);
						reportForm.ShowDialog();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}