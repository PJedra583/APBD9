namespace APBD9.Models.DTO_s;

public class AssignClientDTO
{
    public static int idCounter = 10000;
    public AssignClientDTO(string firstName, string lastName, string email, string telephone, string pesel, int idTrip, string tripName, string paymentDate)
    {
        id = idCounter++;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Telephone = telephone;
        Pesel = pesel;
        IdTrip = idTrip;
        TripName = tripName;
        PaymentDate = paymentDate;
    }
    public int id { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }
    public int IdTrip { get; set; }
    public string TripName { get; set; }
    public string PaymentDate { get; set; }

    public DateTime? GetPaymentDateAsDateTime()
    {
        if (DateTime.TryParse(PaymentDate, out DateTime paymentDateTime))
        {
            return paymentDateTime;
        }
        return null;
    }
}