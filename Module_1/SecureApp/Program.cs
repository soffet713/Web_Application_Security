using System;
using System.Net;
using System.Text;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Database db = new Database();

        Console.WriteLine("Initializing Server...");
        try {
            // Simulated connection test
            Console.WriteLine("Database Connection: OK");
        } catch (Exception ex) {
            Console.WriteLine("Database Error: " + ex.Message);
        }

        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        Console.WriteLine("Listening for connections on http://localhost:8080/");

        while (true)
        {
            // Wait for a request
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string responseString = "";

            if (request.HttpMethod == "GET")
            {
                responseString = @"
                    <html>
                    <body>
                        <h1>Secure Portal (Legacy .NET)</h1>
                        <form method='POST' action='/login'>
                            <h3>Login</h3>
                            Username: <input type='text' name='user' /><br/>
                            Password: <input type='password' name='pass' /><br/>
                            <input type='submit' value='Login' />
                        </form>
                    </body>
                    </html>";
            }
            else if (request.HttpMethod == "POST")
            {
                string input = new StreamReader(request.InputStream).ReadToEnd();
                
                // Logic to parse 'input' and call db.AuthenticateUser(user, pass) would go here
                // For this simulation, we'll assume the check happens:
                bool isAuthenticated = true; 

                if (isAuthenticated)
                    responseString = "<h1>Login Successful!</h1>";
                else
                    responseString = "<h1>Login Failed.</h1>";
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}
