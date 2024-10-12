using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json; // for json serialization and deserialization

namespace HospitalDAL
{
    public class History
    {
        // This func store the patient before it's deletion into a json file
        public void saveToDeletedPatients(Patient patient)
        {
            using (FileStream fin = new FileStream("DeletedPatients.txt", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new(fin))
                {
                    string jasonString = JsonSerializer.Serialize(patient);
                    sw.WriteLine(jasonString); // writing in file as an object
                }
            }
        }

        public void saveToDeletedDoctors(Doctor doctor)
        {
            using (FileStream fin = new FileStream("DeletedDoctors.txt", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new(fin))
                {
                    string jasonString = JsonSerializer.Serialize(doctor);
                    sw.WriteLine(jasonString); // writing in file as an object
                }
            }
        }

        public void saveToCancelledAppointments(Appointment appointment)
        {
            using (FileStream fin = new FileStream("DeletedAppointments.txt", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new(fin))
                {
                    string jasonString = JsonSerializer.Serialize(appointment);
                    sw.WriteLine(jasonString); // writing in file as an object
                }
            }
        }

        public void ShowDeletedPatients()
        {
            using (FileStream fin = new("DeletedPatients.txt", FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader sr = new(fin))
                {
                    string data;
                    while ((data = sr.ReadLine()) != null)
                    {
                        Patient patient = JsonSerializer.Deserialize<Patient>(data);
                        patient.DisplayPatientDetails();
                    }
                }
            }
        }

        public void ShowDeletedDoctors()
        {
            using (FileStream fin = new("DeletedDoctors.txt", FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader sr = new(fin))
                {
                    string data;
                    while ((data = sr.ReadLine()) != null)
                    {
                        Doctor doctor = JsonSerializer.Deserialize<Doctor>(data);
                        doctor.DisplayDoctorDetails();
                    }
                }
            }
        }

        public void ShowCancelledAppoinments()
        {
            using (FileStream fin = new("DeletedAppointments.txt", FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader sr = new(fin))
                {
                    string data;
                    while ((data = sr.ReadLine()) != null)
                    {
                        Appointment appointment = JsonSerializer.Deserialize<Appointment>(data);
                        appointment.DisplayAppointmentDetails();
                    }
                }
            }
        }
    }
}
