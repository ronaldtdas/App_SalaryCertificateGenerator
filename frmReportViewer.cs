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

namespace App_SalaryCertificate
{
    public partial class frmReportViewer: Form
    {
        private clsEmployee? _employeeData;
        private List<clsEmployee>? _employeeDataList;
        private DateTime? _certificateDate;

        public frmReportViewer()
        {
            InitializeComponent();
        }

        public frmReportViewer(clsEmployee employeeData) : this()
        {
            _employeeData = employeeData;
        }

        public frmReportViewer(clsEmployee employeeData, DateTime certificateDate) : this()
        {
            _employeeData = employeeData;
            _certificateDate = certificateDate;
        }

        public frmReportViewer(List<clsEmployee> employeeDataList) : this()
        {
            _employeeDataList = employeeDataList;
        }

        private void frmReportViewer_Load(object sender, EventArgs e)
        {
            if (_employeeDataList != null && _employeeDataList.Count > 0)
            {
                ShowReports(_employeeDataList);
            }
            else if (_employeeData != null)
            {
                ShowReport(_employeeData);
            }
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

        /// <summary>
        /// Display multiple employee reports, each as a complete separate report page
        /// </summary>
        public void ShowReports(List<clsEmployee> employeeDataList)
        {
            try
            {
                if (employeeDataList == null || employeeDataList.Count == 0)
                {
                    MessageBox.Show("No employee data provided", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Only use the first employee from the list
                _employeeData = employeeDataList[0];
                DisplayEmployeeReport(_employeeData, 1, 1);
            }
            catch (Exception ex)
            {
                Clipboard.SetText(ex.ToString());
                MessageBox.Show($"Error generating reports: {ex.Message}\n\n{ex.StackTrace}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Display a single employee's report
        /// </summary>
        private void DisplayEmployeeReport(clsEmployee empData, int currentIndex, int totalCount)
        {
            try
            {
                if (empData == null)
                {
                    MessageBox.Show("Employee data is not provided", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"DisplayEmployeeReport: {empData.FirstName} ({currentIndex}/{totalCount})");

                // Calculate net salary in words
                empData.NetSalaryInWords = NumberToWords(empData.NetSalary);

                ReportViewer reportViewer = rv;
                if (reportViewer == null)
                {
                    MessageBox.Show("Report Viewer control is not initialized", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Set the report to local processing mode
                reportViewer.ProcessingMode = ProcessingMode.Local;

                // Load the RDLC report - use absolute path
                string reportPath = Path.Combine(Application.StartupPath, "SalaryCertificate_new.rdlc");
                System.Diagnostics.Debug.WriteLine($"Report Path: {reportPath}");
                System.Diagnostics.Debug.WriteLine($"Report File Exists: {File.Exists(reportPath)}");

                if (!File.Exists(reportPath))
                {
                    MessageBox.Show($"Report file not found: {reportPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                reportViewer.LocalReport.ReportPath = reportPath;

                // Enable external images
                reportViewer.LocalReport.EnableExternalImages = true;
                string logoFile = System.IO.Path.Combine(Application.StartupPath, "assets", "logo.png");
                if (System.IO.File.Exists(logoFile))
                {
                    var logoUri = new Uri(logoFile).AbsoluteUri;
                    reportViewer.LocalReport.SetParameters(new ReportParameter[] {
                        new ReportParameter("LogoPath", logoUri)
                    });
                }

                // Create a DataTable with ONLY this employee
                DataTable dt = CreateEmployeeDataTable();

                DataRow row = dt.NewRow();
                row["EmployeeID"] = empData.EmployeeID;
                row["Salutation"] = empData.Salutation ?? "";
                row["FirstName"] = empData.FirstName ?? "";
                row["LastName"] = empData.LastName ?? "";
                row["JoiningDate"] = empData.JoiningDate;
                row["Designation"] = empData.Designation ?? "";
                row["BasicSalary"] = empData.BasicSalary;
                row["HouseAllowance"] = empData.HouseAllowance;
                row["MedicalAllowance"] = empData.MedicalAllowance;
                row["TravelAllowance"] = empData.TravelAllowance;
                row["Miscellaneous"] = empData.Miscellaneous;
                row["TotalGrossSalary"] = empData.TotalGrossSalary;
                row["TransportAllowance"] = empData.TransportAllowance;
                row["PerformanceAllowance"] = empData.PerformanceAllowance;
                row["TotalSalary"] = empData.TotalSalary;
                row["MonthlyTaxDeduction"] = empData.MonthlyTaxDeduction;
                row["NetSalary"] = empData.NetSalary;
                row["NetSalaryInWords"] = empData.NetSalaryInWords ?? "";
                dt.Rows.Add(row);

                System.Diagnostics.Debug.WriteLine("DataTable created with single employee");

                ReportDataSource rds = new ReportDataSource("EmployeeData", dt);

                // Clear existing data sources and add the new one
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(rds);
                System.Diagnostics.Debug.WriteLine("About to refresh report");
                
                // Refresh the report
                reportViewer.RefreshReport();
                
                // Update form title to show current page
                this.Text = $"Salary Certificate Report - Page {currentIndex} of {totalCount}";
                
                System.Diagnostics.Debug.WriteLine("Report refreshed successfully");
            }
            catch (Exception ex)
            {
                Clipboard.SetText(ex.ToString());
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
                MessageBox.Show($"Error displaying report: {ex.Message}\n\n{ex.StackTrace}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Display single employee report
        /// </summary>
        public void ShowReport(clsEmployee empData)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ShowReport started");

                if (empData == null)
                {
                    MessageBox.Show("Employee data is not provided", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Calculate net salary in words
                empData.NetSalaryInWords = NumberToWords(empData.NetSalary);

                System.Diagnostics.Debug.WriteLine($"Employee Data: {empData.FirstName}, NetSalary: {empData.NetSalary}");

                // Create a DataTable to hold employee data
                DataTable dt = CreateEmployeeDataTable();

                // Add employee data to DataTable
                DataRow row = dt.NewRow();
                row["EmployeeID"] = empData.EmployeeID;
                row["Salutation"] = empData.Salutation ?? "";
                row["FirstName"] = empData.FirstName ?? "";
                row["LastName"] = empData.LastName ?? "";
                row["JoiningDate"] = empData.JoiningDate;
                row["Designation"] = empData.Designation ?? "";
                row["BasicSalary"] = empData.BasicSalary;
                row["HouseAllowance"] = empData.HouseAllowance;
                row["MedicalAllowance"] = empData.MedicalAllowance;
                row["TravelAllowance"] = empData.TravelAllowance;
                row["Miscellaneous"] = empData.Miscellaneous;
                row["TotalGrossSalary"] = empData.TotalGrossSalary;
                row["TransportAllowance"] = empData.TransportAllowance;
                row["PerformanceAllowance"] = empData.PerformanceAllowance;
                row["TotalSalary"] = empData.TotalSalary;
                row["MonthlyTaxDeduction"] = empData.MonthlyTaxDeduction;
                row["NetSalary"] = empData.NetSalary;
                row["NetSalaryInWords"] = empData.NetSalaryInWords ?? "";
                dt.Rows.Add(row);

                System.Diagnostics.Debug.WriteLine("DataTable created with data");

                LoadReportWithData(dt);
            }
            catch (Exception ex)
            {
                Clipboard.SetText(ex.ToString());
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
                MessageBox.Show($"Error generating report: {ex.Message}\n\n{ex.StackTrace}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable CreateEmployeeDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EmployeeID", typeof(int));
            dt.Columns.Add("Salutation", typeof(string));
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("JoiningDate", typeof(DateTime));
            dt.Columns.Add("Designation", typeof(string));
            dt.Columns.Add("BasicSalary", typeof(decimal));
            dt.Columns.Add("HouseAllowance", typeof(decimal));
            dt.Columns.Add("MedicalAllowance", typeof(decimal));
            dt.Columns.Add("TravelAllowance", typeof(decimal));
            dt.Columns.Add("Miscellaneous", typeof(decimal));
            dt.Columns.Add("TotalGrossSalary", typeof(decimal));
            dt.Columns.Add("TransportAllowance", typeof(decimal));
            dt.Columns.Add("PerformanceAllowance", typeof(decimal));
            dt.Columns.Add("TotalSalary", typeof(decimal));
            dt.Columns.Add("MonthlyTaxDeduction", typeof(decimal));
            dt.Columns.Add("NetSalary", typeof(decimal));
            dt.Columns.Add("NetSalaryInWords", typeof(string));
            return dt;
        }

        private void LoadReportWithData(DataTable dt)
        {
            LoadReportWithData(dt, _certificateDate);
        }

        private void LoadReportWithData(DataTable dt, DateTime? certificateDate)
        {
            ReportDataSource rds = new ReportDataSource("EmployeeData", dt);

            ReportViewer reportViewer = rv;
            System.Diagnostics.Debug.WriteLine($"Report Viewer is null: {reportViewer == null}");

            if (reportViewer == null)
            {
                MessageBox.Show("Report Viewer control is not initialized", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set the report to local processing mode
            reportViewer.ProcessingMode = ProcessingMode.Local;

            // Load the RDLC report - use absolute path
            string reportPath = Path.Combine(Application.StartupPath, "SalaryCertificate_new.rdlc");
            System.Diagnostics.Debug.WriteLine($"Report Path: {reportPath}");
            System.Diagnostics.Debug.WriteLine($"Report File Exists: {File.Exists(reportPath)}");

            if (!File.Exists(reportPath))
            {
                MessageBox.Show($"Report file not found: {reportPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            reportViewer.LocalReport.ReportPath = reportPath;

            // Enable external images (we'll pass a LogoPath parameter pointing to assets/logo.png)
            reportViewer.LocalReport.EnableExternalImages = true;
            string logoFile = System.IO.Path.Combine(Application.StartupPath, "assets", "logo.png");
            
            List<ReportParameter> parameters = new List<ReportParameter>();
            
            if (System.IO.File.Exists(logoFile))
            {
                var logoUri = new Uri(logoFile).AbsoluteUri; // file:///C:/...
                parameters.Add(new ReportParameter("LogoPath", logoUri));
            }

            // Add certificate date parameter if provided
            if (certificateDate.HasValue)
            {
                parameters.Add(new ReportParameter("CertificateDate", certificateDate.Value.ToString("dd MMMM yyyy")));
            }

            if (parameters.Count > 0)
            {
                reportViewer.LocalReport.SetParameters(parameters.ToArray());
            }

            // Clear existing data sources and add the new one
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(rds);
            System.Diagnostics.Debug.WriteLine("About to refresh report");
            // Refresh the report
            reportViewer.RefreshReport();
            System.Diagnostics.Debug.WriteLine("Report refreshed successfully");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_employeeData == null)
                {
                    MessageBox.Show("No employee data available", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Render the current employee's report as PDF
                ReportViewer reportViewer = rv;
                if (reportViewer == null || reportViewer.LocalReport == null)
                {
                    MessageBox.Show("Report is not loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                byte[] pdf = reportViewer.LocalReport.Render("PDF");

                // Show save dialog
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "PDF Files|*.pdf";
                    saveDialog.FileName = $"{_employeeData.FirstName}_{_employeeData.LastName}_SalaryCertificate.pdf";
                    saveDialog.Title = "Save Salary Certificate";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Save the PDF file
                        File.WriteAllBytes(saveDialog.FileName, pdf);

                        // Show success message
                        MessageBox.Show("PDF saved successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Open the PDF file
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = saveDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Clipboard.SetText(ex.ToString());
                MessageBox.Show($"Error saving PDF: {ex.Message}\n\n{ex.StackTrace}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ReportViewer reportViewer = rv;
                if (reportViewer == null)
                {
                    MessageBox.Show("Report Viewer is not initialized", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                reportViewer.PrintDialog();
            }
            catch (Exception ex)
            {
                Clipboard.SetText(ex.ToString());
                MessageBox.Show($"Error printing: {ex.Message}\n\n{ex.StackTrace}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
