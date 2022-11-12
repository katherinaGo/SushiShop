using EASendMail;

namespace SushiShop.EmailService;

public delegate void SendEmailIfPaid(Customer customer);

public static class EmailSender
{
    private const string PathToEmailCreds = "/Users/kate/RiderProjects/SushiShop/SushiShop/EmailService/emailInfo.txt";

    public static void SendEmailWithResults(Customer customer)
    {
        var emailAndPassword = GetCredsFromFile();

        try
        {
            var smtpMail = new SmtpMail("TryIt")
            {
                From = emailAndPassword?.Item1,
                To = customer.Email,
                Subject = "Your order on the small sushi-market. The best sushi in the world!",
                TextBody = $"Dear {customer.Name}.\n" +
                           $"Your order has received and is going be delivered in 1.5h to {customer.Address}.\n" +
                           "\nBest regards, \nSmall sushi-market"
            };
            // smtpMail.AddAttachment("attachmentsHere");

            var oServer = new SmtpServer("smtp.mail.ru")
            {
                User = emailAndPassword?.Item1,
                Password = emailAndPassword?.Item2,
                Port = 465,
                ConnectType = SmtpConnectType.ConnectSSLAuto
            };

            var oSmtp = new SmtpClient();
            oSmtp.SendMail(oServer, smtpMail);
        }
        catch (Exception ep)
        {
            Console.WriteLine(ep.Message);
            Console.WriteLine(ep.StackTrace);
        }
    }

    private static Tuple<string, string> GetCredsFromFile()
    {
        var lines = File.ReadAllLines(PathToEmailCreds);

        var email = "";
        var password = "";

        foreach (var line in lines)
        {
            if (line.Contains("email"))
            {
                email = line.Split(":").Last().Trim();
            }

            if (line.Contains("password"))
            {
                password = line.Split(":").Last().Trim();
            }
        }

        return Tuple.Create(email, password);
    }
}