using static System.Console;
using HospitalDAL;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using System.Numerics;

namespace HospitalConsoleApp
{
    class Program
    {

        public static DataAccess dataAccess = new(); // to access the functionalities
        public static History history = new();
        static void Main()
        {

            Write("\n Welcome to Hospital Management System!");

            Menu menu = new();

            do
            {
                menu.DiplayMenu();
                int opt = GetIntInput();

                switch (opt)
                {
                    case 1:
                        AddPatient();
                        break;

                    case 2:
                        UpdatePatient();
                        break;

                    case 3:
                        DeletePatient();
                        break;

                    case 4:
                        SearchPatientsByName();
                        break;

                    case 5:
                        ViewAllPatients();
                        break;

                    case 6:
                        AddDoctor();
                        break;

                    case 7:
                        UpdateDoctor();
                        break;

                    case 8:
                        DeleteDoctor();
                        break;

                    case 9:
                        SearchDoctorBySpecialization();
                        break;

                    case 10:
                        ViewAllDoctors();
                        break;

                    case 11:
                        BookAppointment();
                        break;

                    case 12:
                        ViewAllAppointments();
                        break;

                    case 13:
                        SearchAppointmentByDoctorOrPatient();
                        break;

                    case 14:
                        CancelAppointment();
                        break;

                    case 15:
                        ViewDeletedHistory();
                        break;

                    case 16:
                        Write("\n Closing the Hospital Management System Console Application!");
                        return;
                        break;

                    default:
                        Write("\n Invalid input!");
                        break;
                }

            } while (true);
        }

        public static bool IsNull(string temp)
        {
            return (temp.IsNullOrEmpty());
        }
        public static bool ValidateFormat(string email)
        {
            // Storing disallowed characters in a set
            HashSet<char> disallowedCharacters = new HashSet<char>
        {
            ' ', ',', ':', ';', '<', '>', '(', ')', '[', ']', '\\', '/', '"', '~'
        };
            bool retVal = true;
            int alphas = 0; // to check whether @ exits in email or not
            foreach (var ch in email)
            {
                if (ch == '@')
                {
                    alphas++;
                }
                else if (disallowedCharacters.Contains(ch))
                {
                    retVal = false;
                    break;
                }
            }
            if (alphas != 1)
            {
                retVal = false;
            }
            else
            {
                int i = email.IndexOf('@');
                string domain = email.Substring(i + 1);
                string local = email.Substring(0, i);

                var tokens = domain.Split('.');
                if (string.IsNullOrWhiteSpace(local) || string.IsNullOrWhiteSpace(domain))
                {
                    retVal = false;
                }
                else if (tokens.Count() > 3)
                {
                    retVal = false;
                }
                else if (local.Contains(".."))
                {
                    retVal = false;
                }
                else if (local.StartsWith(".") || local.EndsWith("."))
                {
                    retVal = false;
                }

                if (retVal)
                {
                    foreach (var token in tokens)
                    {
                        if (token.Count() < 2)
                        {
                            retVal = false;
                            break;
                        }
                    }
                }
            }
            return retVal;
        }

        public static int GetIntInput()
        {
            string input;
            int number;
            input = ReadLine();

            bool success;

            do
            {
                success = int.TryParse(input, out number);
                if (success)
                {
                    return number;
                }
                else
                {
                    Write("\n Invalid value inserted !");
                    Write("\n Enter again : ");
                    input = ReadLine();
                }

            } while (true);

        }

        public static string GetStringInput()
        {
            string input;
            input = ReadLine();
            while (IsNull(input))
            {
                Write("\n Invalid input!\n Enter again : ");
                input = ReadLine();
            }

            return input;
        }

        public static void AddPatient()
        {
            // Taking details of patient
            string name, email, disease;

            Write("\n Enter patient name : ");
            name = GetStringInput();


            Write("\n Enter patient email : ");
            email = GetStringInput();
            while (!ValidateFormat(email))
            {
                Write("\n Invalid email format!\n Enter valid email : ");
                email = ReadLine();
            }

            Write("\n Enter patient disease : ");
            disease = GetStringInput();

            Patient p = new()
            {
                PatientID = dataAccess.patientIdGenerator,
                Name = name,
                Email = email,
                Disease = disease
            };

            dataAccess.InsertPatient(p);
        }

        public static void UpdatePatient()
        {
            int id;
            string name, email, disease;

            Write("\n Enter patient id : ");
            id = GetIntInput();

            Write("\n Enter updated name : ");
            name = GetStringInput();

            Write("\n Enter updated email : ");
            email = GetStringInput();

            Write("\n Enter updated disease : ");
            disease = GetStringInput();

            Patient p = new()
            {
                PatientID = id,
                Name = name,
                Email = email,
                Disease = disease
            };

            dataAccess.UpdatePatientInDatabase(p);
        }

        public static void DeletePatient()
        {
            int id;
            Write("\n Enter id of patient to delete : ");
            id = GetIntInput();
            dataAccess.DeletePatientFromDatabase(id);
        }

        public static void SearchPatientsByName()
        {
            string name;

            Write("\n Enter name of patient to search in database : ");
            name = GetStringInput();

            List<Patient> patients = dataAccess.SearchPatientsInDatabase(name);

            if (patients.Count == 0)
            {
                Write("\n No matching records!");
            }
            else
            {
                Write("\n\n The patients are ");
                foreach (Patient p in patients)
                {
                    p.DisplayPatientDetails();
                }
            }
        }

        public static void ViewAllPatients()
        {
            List<Patient> patients = dataAccess.GetAllPatientsFromDatabase();

            if (patients.Count == 0)
            {
                Write("\n No patients in database!");
            }
            else
            {
                Write("\n\n All the patients are \n");
                foreach (Patient p in patients)
                {
                    p.DisplayPatientDetails();
                }
            }
        }

        public static void AddDoctor()
        {
            string name, specialization;


            Write("\n Enter name of doctor : ");
            name = GetStringInput();

            Write("\n Enter specialization of doctor : ");
            specialization = GetStringInput();


            Doctor doc = new Doctor()
            {
                DoctorId = dataAccess.doctorIdGenerator,
                Name = name,
                Specialization = specialization
            };

            dataAccess.InsertDoctor(doc);
        }

        public static void UpdateDoctor()
        {
            string name, specialization;
            int id;

            Write("\n Enter id of doctor to update : ");
            id = GetIntInput();


            Write("\n Enter updated name : ");
            name = GetStringInput();


            Write("\n Enter updated specialization : ");
            specialization = GetStringInput();


            Doctor doc = new Doctor()
            {
                DoctorId = dataAccess.doctorIdGenerator,
                Name = name,
                Specialization = specialization
            };

            dataAccess.UpdateDoctorInDatabase(doc);
        }

        public static void DeleteDoctor()
        {
            int id;
            Write("\n Enter id of doctor to delete : ");
            id = GetIntInput();

            dataAccess.DeleteDoctorFromDatabase(id);
        }

        public static void SearchDoctorBySpecialization()
        {
            string specialization;
            Write("\n Enter specialization field of doctor to search in database : ");
            specialization = GetStringInput();


            List<Doctor> doctors = dataAccess.SearchDoctorsInDatabase(specialization);

            if (doctors.Count == 0)
            {
                Write("\n No matching records found!");
            }
            else
            {
                Write("\n\n The doctors are \n");
                foreach (Doctor doc in doctors)
                {
                    doc.DisplayDoctorDetails();
                }
            }
        }

        public static void ViewAllDoctors()
        {
            List<Doctor> doctors = dataAccess.GetAllDoctorsFromDatabase();

            if (doctors.Count == 0)
            {
                Write("\n No doctors record in database!");
            }
            else
            {
                Write("\n\n All the doctors are \n");
                foreach (Doctor doc in doctors)
                {
                    doc.DisplayDoctorDetails();
                }
            }
        }

        public static void BookAppointment()
        {
            int pId, dId;
            DateTime dateTime;

            Write("\n Enter patient id : ");
            pId = GetIntInput();

            Write("\n Enter doctor id : ");
            dId = GetIntInput();

            Write("\n Enter appointment date and time : ");
            string dateString = ReadLine();
            bool isValid = DateTime.TryParse(dateString, out dateTime);

            while (dateTime < DateTime.Now && !isValid)
            {
                Write("\n Invalid date and time. Future date and time must be entered and valid months,days count should be entered! ");
                Write("\n Enter appointment date and time : ");
                dateString = ReadLine();
                isValid = DateTime.TryParse(dateString, out dateTime);
            }

            Appointment appointment = new()
            {
                AppointmentId = dataAccess.appointMentIdGenerator,
                PatientId = pId,
                DoctorId = dId,
                AppointmentDate = dateTime
            };

            dataAccess.InsertAppointment(appointment);
        }

        public static void ViewAllAppointments()
        {
            List<Appointment> appointments = dataAccess.GetAllAppointmentsFromDatabase();

            if (appointments.Count == 0)
            {
                Write("\n No appointment records in database!");
            }
            else
            {
                Write("\n All the appointments are \n");
                foreach (Appointment appointment in appointments)
                {
                    appointment.DisplayAppointmentDetails();
                }
            }
        }

        public static void SearchAppointmentByDoctorOrPatient()
        {
            int opt;
            int id;
            Write("\n Choose an option to filter out appointments \n1) Patient\n 2) Doctor : ");
            opt = GetIntInput();

            switch (opt)
            {
                case 1:
                    Write("\n Enter patient id to view all the appointments : ");
                    id = GetIntInput();
                    dataAccess.ViewXPatientAppointments(id);
                    break;

                case 2:
                    Write("\n Enter doctor id to view all the appointments : ");
                    id = GetIntInput();
                    dataAccess.ViewXDoctorAppointments(id);
                    break;

                default:
                    Write("\n Invalid input!");
                    break;
            }
        }

        public static void CancelAppointment()
        {

            Write("\n Enter appointment id : ");
            int id = GetIntInput();
            dataAccess.DeleteAppointmentFromDatabase(id);
        }

        public static void ViewDeletedHistory()
        {
            Write("\n Enter catagory to view deleted history\n1)Patient \n2Doctor \n3)Appointment : ");
            int catagory = GetIntInput();

            switch (catagory)
            {
                case 1:
                    history.ShowDeletedPatients();
                    break;

                case 2:
                    history.ShowDeletedDoctors();
                    break;

                case 3:
                    history.ShowCancelledAppoinments();
                    break;

                default:
                    Write("\n Invalid input!");
                    break;
            }

        }
    }
}