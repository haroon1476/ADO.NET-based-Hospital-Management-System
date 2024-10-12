using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace HospitalConsoleApp
{

    internal class Menu
    {
        public void DiplayMenu()
        {
            WriteLine("\nSelect one of the following options!");
            Write("\n1) Add a patient \n2) Update a patient \n3) Delete a patient \n4) Search for patients by name \n5) View all patients \n6) Add a new doctor \n7) Update a doctor \n8) Delete a doctor \n9) Search for doctors by specialization \n10) View all doctors \n11) Book an appointment \n12) View all appointments \n13) Search appointments by doctor or patient \n14) Cancel an appointment \n15 View history of deleted records \n16) Exit the application \n : ");
        }
    }
}
