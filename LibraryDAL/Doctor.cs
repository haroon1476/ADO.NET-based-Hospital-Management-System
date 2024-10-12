using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDAL
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string? Name { get; set; }
        public string? Specialization { get; set; }

        public void DisplayDoctorDetails()
        {
            Console.WriteLine($"Doctor Id = {DoctorId} , Name = {Name} , Specialization = {Specialization}");
        }

    }
}
