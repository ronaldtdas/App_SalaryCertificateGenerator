using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_SalaryCertificate
{
    public class clsEmployee
    {
		public int EmployeeID { get; set; }
		public string? Salutation { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public DateTime JoiningDate { get; set; }
		public string? Designation { get; set; }
		public decimal BasicSalary { get; set; }
		public decimal HouseAllowance { get; set; }
		public decimal MedicalAllowance { get; set; }
		public decimal TravelAllowance { get; set; }
		public decimal Miscellaneous { get; set; }
		public decimal TotalGrossSalary
		{
			get
			{
				return BasicSalary + HouseAllowance + MedicalAllowance + TravelAllowance + Miscellaneous;
			}
		}
		public decimal TransportAllowance { get; set; }
		public decimal PerformanceAllowance { get; set; }

		public decimal TotalSalary
		{
			get
			{
				return TotalGrossSalary + TransportAllowance + PerformanceAllowance;
			}
		}
		public decimal MonthlyTaxDeduction { get; set; }
		public decimal NetSalary
		{
			get
			{
				return TotalSalary - MonthlyTaxDeduction;
			}
		}
		public string? NetSalaryInWords { get; set; }
	}
}
