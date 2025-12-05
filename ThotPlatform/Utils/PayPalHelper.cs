using System;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ThotPlatform.Utils
{
    /// <summary>
    /// Classe utilitaire pour l'integration PayPal
    /// </summary>
    public static class PayPalHelper
    {
        private static readonly string PayPalMode = ConfigurationManager.AppSettings["PayPalMode"];
        private static readonly string ClientId = ConfigurationManager.AppSettings["PayPalClientId"];
        private static readonly string ClientSecret = ConfigurationManager.AppSettings["PayPalClientSecret"];

        private static string BaseUrl => PayPalMode == "sandbox"
            ? "https://api-m.sandbox.paypal.com"
            : "https://api-m.paypal.com";

        private static void EnsureCredentialsConfigured()
        {
            if (string.IsNullOrWhiteSpace(ClientId) || ClientId == "votre-client-id" ||
                string.IsNullOrWhiteSpace(ClientSecret) || ClientSecret == "votre-client-secret")
            {
                throw new InvalidOperationException("Les identifiants PayPal ne sont pas correctement configures.");
            }
        }

        /// <summary>
        /// Obtient un token d'acces PayPal
        /// </summary>
        private static async Task<string> GetAccessTokenAsync()
        {
            using (var client = new HttpClient())
            {
                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");

                var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await client.PostAsync($"{BaseUrl}/v1/oauth2/token", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                    return result.access_token;
                }

                throw new Exception("Impossible d'obtenir le token d'acces PayPal");
            }
        }

        /// <summary>
        /// Cree un paiement PayPal
        /// </summary>
        public static async Task<PayPalPaymentResult> CreatePaymentAsync(decimal amount, string description, string returnUrl, string cancelUrl)
        {
            try
            {
                EnsureCredentialsConfigured();
                var accessToken = await GetAccessTokenAsync();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var payment = new
                    {
                        intent = "sale",
                        payer = new
                        {
                            payment_method = "paypal"
                        },
                        transactions = new[]
                        {
                            new
                            {
                                amount = new
                                {
                                    total = amount.ToString("F2", CultureInfo.InvariantCulture),
                                    currency = "CAD"
                                },
                                description = description
                            }
                        },
                        redirect_urls = new
                        {
                            return_url = returnUrl,
                            cancel_url = cancelUrl
                        }
                    };

                    var json = JsonConvert.SerializeObject(payment);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{BaseUrl}/v1/payments/payment", content);
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                        string approvalUrl = null;
                        foreach (var link in result.links)
                        {
                            if (link.rel == "approval_url")
                            {
                                approvalUrl = link.href;
                                break;
                            }
                        }

                        return new PayPalPaymentResult
                        {
                            Success = true,
                            PaymentId = result.id,
                            ApprovalUrl = approvalUrl
                        };
                    }
                    else
                    {
                        return new PayPalPaymentResult
                        {
                            Success = false,
                            ErrorMessage = $"Erreur PayPal: {jsonResponse}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new PayPalPaymentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Execute un paiement PayPal apres approbation
        /// </summary>
        public static async Task<PayPalExecutionResult> ExecutePaymentAsync(string paymentId, string payerId)
        {
            try
            {
                EnsureCredentialsConfigured();
                var accessToken = await GetAccessTokenAsync();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var execution = new
                    {
                        payer_id = payerId
                    };

                    var json = JsonConvert.SerializeObject(execution);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{BaseUrl}/v1/payments/payment/{paymentId}/execute", content);
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                        return new PayPalExecutionResult
                        {
                            Success = true,
                            PaymentId = result.id,
                            State = result.state,
                            TransactionId = result.transactions[0].related_resources[0].sale.id
                        };
                    }
                    else
                    {
                        return new PayPalExecutionResult
                        {
                            Success = false,
                            ErrorMessage = $"Erreur lors de l'execution du paiement: {jsonResponse}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new PayPalExecutionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Verifie le statut d'un paiement
        /// </summary>
        public static async Task<PayPalPaymentStatus> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                EnsureCredentialsConfigured();
                var accessToken = await GetAccessTokenAsync();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var response = await client.GetAsync($"{BaseUrl}/v1/payments/payment/{paymentId}");
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                        return new PayPalPaymentStatus
                        {
                            Success = true,
                            State = result.state,
                            Amount = decimal.Parse(result.transactions[0].amount.total.ToString())
                        };
                    }
                    else
                    {
                        return new PayPalPaymentStatus
                        {
                            Success = false,
                            ErrorMessage = $"Erreur lors de la verification: {jsonResponse}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new PayPalPaymentStatus
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }

    /// <summary>
    /// Resultat de la creation d'un paiement PayPal
    /// </summary>
    public class PayPalPaymentResult
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; }
        public string ApprovalUrl { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Resultat de l'execution d'un paiement PayPal
    /// </summary>
    public class PayPalExecutionResult
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; }
        public string State { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Statut d'un paiement PayPal
    /// </summary>
    public class PayPalPaymentStatus
    {
        public bool Success { get; set; }
        public string State { get; set; }
        public decimal Amount { get; set; }
        public string ErrorMessage { get; set; }
    }
}


