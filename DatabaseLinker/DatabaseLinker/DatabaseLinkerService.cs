using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DatabaseLinker
{
    public partial class DatabaseLinkerService : ServiceBase
    {

        public static HttpClient GetterClient = new HttpClient();
        public static HttpClient SetterClient = new HttpClient();
        Timer timer;
        int PeriodMilliseconds;
        bool Overwrite;
        private string pOST_URL;
        private string gET_URL;
        public string POST_RESOURCE;
        public string GET_RESOURCE;
        string LOG_PATH;
        Log log;

        public string POST_URL
        {
            get => pOST_URL; set
            {
                if (value is null) return;
                pOST_URL = value;
                SetterClient = new HttpClient
                {
                    BaseAddress = new Uri(value)
                };
                SetterClient.DefaultRequestHeaders.Accept.Clear();
                SetterClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

            }
        }
        public string GET_URL
        {
            get => gET_URL; set
            {
                if (value is null) return;
                gET_URL = value;
                GetterClient = new HttpClient
                {
                    BaseAddress = new Uri(value)
                };
                GetterClient.DefaultRequestHeaders.Accept.Clear();
                GetterClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public DatabaseLinkerService()
        {
            InitializeComponent();
        }
        public void GetConfigurationValues()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["PeriodMilliseconds"], out PeriodMilliseconds))
            {
                PeriodMilliseconds = 60 * 5 * 1000; //5 Minutes
            }
            if (!bool.TryParse(ConfigurationManager.AppSettings["Overwrite"], out Overwrite))
            {
                Overwrite = false;
            }
            POST_URL = ConfigurationManager.AppSettings["POST_URL"];
            GET_URL = ConfigurationManager.AppSettings["GET_URL"];
            POST_RESOURCE = ConfigurationManager.AppSettings["POST_RESOURCE"];
            GET_RESOURCE = ConfigurationManager.AppSettings["GET_RESOURCE"];
            LOG_PATH = ConfigurationManager.AppSettings["LOG_PATH"];
            if (string.IsNullOrEmpty(LOG_PATH))
            {
                LOG_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DatabaseLinkerExecution.log");
            }
        }
        protected override void OnStart(string[] args)
        {
            GetConfigurationValues();
            log = new Log(LOG_PATH);
            timer = new Timer(ExecuteAsync, null, 0, PeriodMilliseconds);
        }

        private async void ExecuteAsync(object state)
        {
            var previousTime = PeriodMilliseconds;
            GetConfigurationValues();
            log.Path = LOG_PATH;
            if (previousTime != PeriodMilliseconds)
            {
                timer.Change(PeriodMilliseconds, PeriodMilliseconds);
            }
            var response = await GET();
            log.Write("GET: " + response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                log.Write("GET: " + response.Content);
            }
            else
            {
                log.Write("GET: ERROR");
            }
            response = await SET(response.Content);
            log.Write("SET: " + response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                log.Write("SET: " + response.Content);
            }
            else
            {
                log.Write("SET: ERROR");
            }
        }
        public async Task<HttpResponseMessage> GET()
        {
            HttpResponseMessage response = await GetterClient.GetAsync(GET_RESOURCE);
            return response;

        }

        public async Task<HttpResponseMessage> SET(HttpContent content)
        {
            HttpResponseMessage response = await SetterClient.PostAsync(POST_RESOURCE, content);
            return response;

        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        public void Start()
        {
            OnStart(null);
        }
    }
    public class Log
    {
        private string path;

        public Log(string path)
        {
            this.Path = path;
        }

        public string Path { get => path; set => path = value; }

        public void Write(string message)
        {
            using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}
