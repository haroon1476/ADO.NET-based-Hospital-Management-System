using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDAL
{
    public class Patient
    {
        public int PatientID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Disease { get; set; }

        public void DisplayPatientDetails()
        {
            Console.WriteLine($"Patient Id = {PatientID} , Name = {Name} , Email = {Email} , Disease = {Disease}");
        }
    }
}
