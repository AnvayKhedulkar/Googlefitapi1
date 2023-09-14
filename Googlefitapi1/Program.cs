using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Fitness.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleFitLogin
{
    public partial class Form1 : Form
    {
        private FitnessClient fitnessClient;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Create an OAuth2 client for Google Fit.
            var clientSecrets = new ClientSecrets
            {
                ClientId = "315763140443-8horjv732uls3k9fs7e69sa04trr5tuh.apps.googleusercontent.com",
                ClientSecret = "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"
            };
            var client = new OAuth2Client(clientSecrets);

            // Start the OAuth2 authorization flow.
            var authContext = new AuthorizationCodeFlow(client, "http://localhost:5000/google-fit-auth-callback");
            authContext.Start(this);
        }

        private void OnAuthorizationCompleted(AuthorizationCodeCompletedEventArgs eventArgs)
        {
            // Get the access token from the authorization flow.
            var accessToken = eventArgs.AccessToken;

            // Create a Fitness client with the access token.
            fitnessClient = new FitnessClient(accessToken);

            // Enable the retrieve data button.
            buttonRetrieveData.Enabled = true;
        }

        private void buttonRetrieveData_Click(object sender, EventArgs e)
        {
            // Retrieve the user's heart rate and SpO2 data from Google Fit.
            var heartRateRequest = new HeartRateRequest();
            var spo2Request = new SpO2Request();

            var requests = new List<IFitnessDataPointRequest>
            {
                heartRateRequest,
                spo2Request
            };

            try
            {
                // Execute the requests.
                fitnessClient.ExecuteBatchRequest(requests);
            }
            catch (Exception ex)
            {
                // Handle the error.
                Console.WriteLine(ex.Message);
            }

            // Display the retrieved data.
            foreach (var request in requests)
            {
                var dataPoints = request.GetDataPoints();

                foreach (var dataPoint in dataPoints)
                {
                    Console.WriteLine($"{request.DataType}: {dataPoint.Value}");
                }
            }
        }
    }
}