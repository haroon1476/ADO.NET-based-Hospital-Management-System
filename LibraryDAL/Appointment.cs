namespace HospitalDAL
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }

        public void DisplayAppointmentDetails()
        {
            Console.WriteLine($"Appointment Id = {AppointmentId} , Patient Id = {PatientId} , Doctor Id = {DoctorId} , Appointment Date-Time = {AppointmentDate}");
        }
    }
}
