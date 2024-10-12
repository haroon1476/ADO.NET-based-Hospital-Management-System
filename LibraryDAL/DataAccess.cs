using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using HospitalDAL;
using Microsoft.Data.SqlClient;
using static System.Console;

namespace HospitalDAL
{
    public class DataAccess
    {
        public string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=HospitalManagementSystem;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";


        // Global variables for keeping check of id uniqueness
        public int patientIdGenerator = 0;
        public int doctorIdGenerator = 0;
        public int appointMentIdGenerator = 0;
        public static History history = new(); // to save records before deletion

        public DataAccess()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select ISNULL(max(PatientId), 0) from Patient";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar(); // Execute the query and get the result

                    if (result != DBNull.Value)
                    {
                        patientIdGenerator = Convert.ToInt32(result); // setting the patientIdGenerator to the next ID
                    }
                    else
                    {
                        patientIdGenerator = 1; // if no records, starting with 1
                    }

                }
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select isNull(max(DoctorId) ,0) from Doctor";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar(); // Execute the query and get the result

                    if (result != DBNull.Value)
                    {
                        doctorIdGenerator = Convert.ToInt32(result); // Set the patientIdGenerator to the next ID
                    }
                    else
                    {
                        doctorIdGenerator = 1; // If no records, start with 1
                    }

                }
            }


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select isNull(max(AppointmentId) ,0) from Appointment";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar(); // Execute the query and get the result

                    if (result != DBNull.Value)
                    {
                        appointMentIdGenerator = Convert.ToInt32(result); // Set the patientIdGenerator to the next ID
                    }
                    else
                    {
                        appointMentIdGenerator = 1; // If no records, start with 1
                    }

                }
            }
        }

        // <-----  INSERT FUNCTIONS


        // This function inserts the patient in the database
        public void InsertPatient(Patient patient)
        {
            patient.PatientID = ++patientIdGenerator; // using current ID

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "insert into Patient values (@PatientID, @Name, @Email, @Disease)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patient.PatientID);
                    cmd.Parameters.AddWithValue("@Name", patient.Name);
                    cmd.Parameters.AddWithValue("@Email", patient.Email);
                    cmd.Parameters.AddWithValue("@Disease", patient.Disease);

                    conn.Open();

                    int flag = cmd.ExecuteNonQuery();
                    if (flag >= 1)
                    {
                        Console.WriteLine("\n Patient record inserted!");
                    }
                    else
                    {
                        Console.WriteLine("\n Unable to execute the query!");
                    }
                }
            }

        }



        // This function inserts the doctor in the database
        public void InsertDoctor(Doctor doctor)
        {
            doctor.DoctorId = ++doctorIdGenerator;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "insert into Doctor values (@DoctorId , @Name , @Specialization)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctor.DoctorId);
                    cmd.Parameters.AddWithValue("Name", doctor.Name);
                    cmd.Parameters.AddWithValue("@Specialization", doctor.Specialization);

                    conn.Open();
                    int afftectedRows = cmd.ExecuteNonQuery();
                    if (afftectedRows >= 1)
                    {
                        Console.WriteLine("\n Doctor record inserted!");
                    }
                    else
                    {
                        Console.WriteLine("\n Unable to execute the query!");
                    }
                }
            }
        }

        // This function inserts the appointment in the database
        public void InsertAppointment(Appointment appointment)
        {
            try
            {
                appointment.AppointmentId = ++appointMentIdGenerator;
                using (SqlConnection conn = new(connectionString))
                {
                    string query = "Insert into Appointment values (@AppointmentId , @PatientId , @DoctorId , @AppointmentDate)";
                    using (SqlCommand cmd = new(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentId", appointment.AppointmentId);
                        cmd.Parameters.AddWithValue("PatientId", appointment.PatientId);
                        cmd.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
                        cmd.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);

                        conn.Open();
                        int afftectedRows = cmd.ExecuteNonQuery();
                        if (afftectedRows >= 1)
                        {
                            Console.WriteLine("\n Appointment Booked!");
                        }
                        else
                        {
                            Console.WriteLine("\n Unable to execute the query!");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("\nCannot book an appointment because invalid doctor or patient id mentioned.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nAn error occurred: {e.Message}");
            }

        }


        // <------- READ FUNCTIONS
        public List<Patient> GetAllPatientsFromDatabase()
        {
            List<Patient> patients = new(); // Creating a list to store Patients
            using (SqlConnection conn = new(connectionString))
            {
                string query = $"select * from Patient";

                using (SqlCommand cmd = new(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader sr = cmd.ExecuteReader())
                    {
                        while (sr.Read())
                        {
                            int id = sr.GetInt32(0);
                            string name = sr.GetString(1);
                            string email = sr.GetString(2);
                            string disease = sr.GetString(3);

                            Patient patient = new()
                            {
                                PatientID = id,
                                Name = name,
                                Email = email,
                                Disease = disease
                            };

                            patients.Add(patient); // adding patient to list
                        }
                    }
                }

            }
            return patients;
        }

        public List<Doctor> GetAllDoctorsFromDatabase()
        {
            List<Doctor> doctors = new(); // Creating a list to store Doctors
            using (SqlConnection conn = new(connectionString))
            {
                string query = $"select * from Doctor";

                using (SqlCommand cmd = new(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader sr = cmd.ExecuteReader())
                    {
                        while (sr.Read())
                        {
                            int id = sr.GetInt32(0);
                            string name = sr.GetString(1);
                            string specialization = sr.GetString(2);

                            Doctor doc = new()
                            {
                                DoctorId = id,
                                Name = name,
                                Specialization = specialization
                            };

                            doctors.Add(doc); // adding doctor to list
                        }
                    }
                }

            }
            return doctors; // returning the list
        }

        public List<Appointment> GetAllAppointmentsFromDatabase()
        {
            List<Appointment> appointments = new(); // Creating a list to store Appointments
            using (SqlConnection conn = new(connectionString))
            {
                string query = $"select * from Appointment";

                using (SqlCommand cmd = new(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader sr = cmd.ExecuteReader())
                    {
                        while (sr.Read())
                        {
                            int id = sr.GetInt32(0);
                            int pId = sr.GetInt32(1);
                            int dId = sr.GetInt32(2);
                            DateTime dt = sr.GetDateTime(3);

                            Appointment appointment = new()
                            {
                                AppointmentId = id,
                                PatientId = pId,
                                DoctorId = dId,
                                AppointmentDate = dt
                            };

                            appointments.Add(appointment); // adding doctor to list
                        }
                    }
                }

            }

            return appointments; // returning the list
        }


        // <----- UPDATE FUNCTIONS

        public void UpdatePatientInDatabase(Patient patient)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "Update Patient set Name = @name, Email = @email, Disease = @disease WHERE PatientId = @patientId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", patient.Name);
                    cmd.Parameters.AddWithValue("@email", patient.Email);
                    cmd.Parameters.AddWithValue("@disease", patient.Disease);
                    cmd.Parameters.AddWithValue("@patientId", patient.PatientID);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected >= 1)
                    {
                        WriteLine("\nRecord updated!");
                    }
                    else
                    {
                        WriteLine("\nNo matching records found!");
                    }
                }
            }
        }


        public void UpdateDoctorInDatabase(Doctor doctor)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "Update Doctor set Name = @name , Specialization = @specialization where DoctorId = @oldDoctorId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("name", doctor.Name);
                    cmd.Parameters.AddWithValue("specialization", doctor.Specialization);
                    cmd.Parameters.AddWithValue("oldDoctorId", doctor.DoctorId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected >= 1)
                    {
                        WriteLine("\nRecord updated!");
                    }
                    else
                    {
                        WriteLine("\nNo matching records found!");
                    }
                }
            }
        }

        public void UpdateAppointmentInDatabase(Appointment appointment)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "Update Appointment set DoctorId = @doctorId , PatientId = @patientId , AppointmentDate = @appointmentDate where AppointmentId = @appointmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("appointmentId", appointment.AppointmentId);
                    cmd.Parameters.AddWithValue("doctorId", appointment.DoctorId);
                    cmd.Parameters.AddWithValue("patientId", appointment.PatientId);
                    cmd.Parameters.AddWithValue("appointmentDate", appointment.AppointmentDate);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected >= 1)
                    {
                        WriteLine("\nRecord updated!");
                    }
                    else
                    {
                        WriteLine("\nNo matching records found!");
                    }
                }
            }
        }


        // <----------- DELETE FUNCTIONS
        public void DeletePatientFromDatabase(int patientId)
        {
            try
            {
                // First , getting the patient to del for writing purposes
                Patient? patientToDelete = null;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string selectQuery = "Select * from Patient where PatientId = @patientId";

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@patientId", patientId);
                        conn.Open();

                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Assuming Patient class has a constructor that accepts parameters
                                patientToDelete = new Patient
                                {
                                    PatientID = (int)reader["PatientId"],
                                    Name = reader["Name"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Disease = reader["Disease"].ToString()
                                };
                            }
                        }
                    }
                }

                // If the patient was found, proceeding to delete
                if (patientToDelete != null)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {

                        string deleteQuery = "Delete from Patient where PatientId = @patientId";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@patientId", patientId);

                            conn.Open();
                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            if (rowsAffected >= 1)
                            {
                                // Saving data in history file before deletion
                                history.saveToDeletedPatients(patientToDelete);
                                Console.WriteLine("\nPatient Deleted!");
                            }
                            else
                            {
                                Console.WriteLine("\nNo matching records found!");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nNo matching records found!");
                }
            }
            catch (SqlException ex)
            {
                // Handle foreign key violations or other database issues
                if (ex.Message.Contains("REFERENCE constraint"))
                {
                    Console.WriteLine("\nCannot delete patient because it is referenced by other records.");
                }
                else
                {
                    Console.WriteLine($"\nSQL error occurred: {ex.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nAn error occurred: {e.Message}");
            }
        }

        public void DeleteDoctorFromDatabase(int doctorId)
        {
            try
            {

                // First , getting the doctor to del for writing purposes
                Doctor? doctorToDelete = null;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string selectQuery = "Select * from Doctor where DoctorId = @doctorId";

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@doctorId", doctorId);
                        conn.Open();

                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                doctorToDelete = new Doctor
                                {
                                    DoctorId = (int)reader["DoctorId"],
                                    Name = reader["Name"].ToString(),
                                    Specialization = reader["Specialization"].ToString()
                                };
                            }
                        }
                    }
                }

                // If the doctor was found, proceeding to delete
                if (doctorToDelete != null)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {

                        string deleteQuery = "Delete from Doctor where DoctorId = @doctorId";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@doctorId", doctorId);

                            conn.Open();
                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            if (rowsAffected >= 1)
                            {
                                // Saving data in history file before deletion
                                history.saveToDeletedDoctors(doctorToDelete);
                                Console.WriteLine("\nDoctor Deleted!");
                            }
                            else
                            {
                                Console.WriteLine("\nNo matching records found!");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nNo matching records found!");
                }
            }
            catch (SqlException ex)
            {
                // Handle foreign key violations or other database issues
                if (ex.Message.Contains("REFERENCE constraint"))
                {
                    Console.WriteLine("\nCannot delete doctor because it is referenced by other records.");
                }
                else
                {
                    Console.WriteLine($"\nSQL error occurred: {ex.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nAn error occurred: {e.Message}");
            }
        }

        public void DeleteAppointmentFromDatabase(int appointmentId)
        {

            try
            {
                // First , getting the appointment to del for writing purposes
                Appointment? appointmentToDelete = null;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string selectQuery = "Select * from Appointment where AppointmentId = @appointmentId";

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@appointmentId", appointmentId);
                        conn.Open();

                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                appointmentToDelete = new Appointment
                                {
                                    AppointmentId = (int)reader["AppointmentId"],
                                    PatientId = (int)reader["PatientId"],
                                    DoctorId = (int)reader["DoctorId"],
                                    AppointmentDate = (DateTime)reader["AppointmentDate"]
                                };
                            }
                        }
                    }
                }

                // If the appointment was found, proceeding to delete
                if (appointmentToDelete != null)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {

                        string deleteQuery = "Delete from Appointment where AppointmentId = @appointmentId";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@appointmentId", appointmentId);

                            conn.Open();
                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            if (rowsAffected >= 1)
                            {
                                // Saving data in history file before deletion
                                history.saveToCancelledAppointments(appointmentToDelete);
                                Console.WriteLine("\nAppointment cancelled!");
                            }
                            else
                            {
                                Console.WriteLine("\nNo matching records found!");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nNo matching records found!");
                }
            }
            catch (SqlException ex)
            {
                // Handle foreign key violations or other database issues
                if (ex.Message.Contains("REFERENCE constraint"))
                {
                    Console.WriteLine("\nCannot cancel appointments because it is referenced by other records.");
                }
                else
                {
                    Console.WriteLine($"\nSQL error occurred: {ex.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nAn error occurred: {e.Message}");
            }
        }

        // <-------- SEARCHING FUNCTION
        public List<Patient> SearchPatientsInDatabase(string name)
        {
            List<Patient> patients = new();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "Select * from Patient where Name = @name";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("name", name);

                    conn.Open();
                    using (SqlDataReader sr = cmd.ExecuteReader())
                    {
                        while (sr.Read())
                        {
                            patients.Add(new Patient()
                            {
                                PatientID = sr.GetInt32(0),
                                Name = sr.GetString(1),
                                Email = sr.GetString(2),
                                Disease = sr.GetString(3)
                            });
                        }
                    }
                }
            }

            return patients;
        }

        public List<Doctor> SearchDoctorsInDatabase(string specialization)
        {
            List<Doctor> doctors = new List<Doctor>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "Select * from Doctor where Specialization = @specialization";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Specialization", specialization);

                    conn.Open();
                    using (SqlDataReader sr = cmd.ExecuteReader())
                    {
                        while (sr.Read())
                        {
                            doctors.Add(new Doctor()
                            {
                                DoctorId = sr.GetInt32(0),
                                Name = sr.GetString(1),
                                Specialization = sr.GetString(2)
                            });
                        }
                    }
                }
            }

            return doctors;
        }



        // Some additional functions
        public void ViewXPatientAppointments(int id)
        {
            string query = "Select * from Appointment where PatientId = @patientId";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("patientId", id);

                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Appointment appointment = new()
                            {
                                AppointmentId = dr.GetInt32(0),
                                PatientId = dr.GetInt32(1),
                                DoctorId = dr.GetInt32(2),
                                AppointmentDate = dr.GetDateTime(3)
                            };
                            appointment.DisplayAppointmentDetails();
                        }
                    }
                }
            }
        }

        public void ViewXDoctorAppointments(int id)
        {
            string query = "Select * from Appointment where DoctorId = @doctorId";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("doctorId", id);

                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Appointment appointment = new()
                            {
                                AppointmentId = dr.GetInt32(0),
                                PatientId = dr.GetInt32(1),
                                DoctorId = dr.GetInt32(2),
                                AppointmentDate = dr.GetDateTime(3)
                            };
                            appointment.DisplayAppointmentDetails();
                        }
                    }
                }
            }
        }

    }

}
