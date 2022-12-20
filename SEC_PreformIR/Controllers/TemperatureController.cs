using System.Configuration;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SEC_PreformIR.Models;

namespace SEC.PreformTempControl.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemperatureController : ControllerBase
    {

        #region Fields

        public IConfiguration Configuration { get; }
        private readonly ILogger<TemperatureController> _logger;
        private readonly EntityContext context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment env;
        private float _temperature = 0;

        #endregion

        #region Constructor

        public TemperatureController(ILogger<TemperatureController> logger, IConfiguration configuration, EntityContext context )
        {
            _logger = logger;
            Configuration = configuration;
            this.context = context;
            this.env = env;
        }

        #endregion

        #region API METHODS

        [HttpGet(Name = "GetTemperature")]
        public float Get()
        {
            return _temperature;
        }

        [HttpPost(Name = "WriteTemperatureTag")]
        public async Task<IActionResult> Post([FromBody] TemperatureModel model)
        {

            //update kepware tag
            //await SetOPCFloat("E80_RSLogix_5000.Full_Bin_PU.Global.test_temp", temperatureValue);


            ////send signalR off to client application to show change
            ///
            /// try  
            /// 

            context.Add(new TemperatureModel() { MachineID = 6, Timestamp = DateTime.Now, TemperatureValue = model.TemperatureValue });
            try
            {
                await _SendClientNotification(model);
                
                //await _SendClientNotification("66");
            }

            catch (Exception e)
            {

                return BadRequest(e);
            }

            await context.SaveChangesAsync();
            return Ok();
        }



        #endregion

        #region Talk to Kepware
        private async Task<OpcResultRoot> SetOPCFloat(string tagname, float value)
        {
            try
            {
                var urlString = "http://OH-OPC-01.sec.int:39320/iotgateway/write";
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), urlString))
                    {
                        var list = new List<object>();
                        dynamic root = new ExpandoObject();
                        root.id = tagname;
                        root.v = value;
                        list.Add(root);
                        var data = JsonConvert.SerializeObject(list);
                        request.Content = new StringContent(data);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        var response = await httpClient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var ret = await response.Content.ReadAsAsync<OpcResultRoot>();
                            return ret;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Talk To Client
        private async Task _SendClientNotification(TemperatureModel model)
        {
            string url = "";

            url = "http://oh-resin02/preformtemperature/temperatureHub";

            try
            {
                var connection = new HubConnectionBuilder().WithUrl(url, options =>
                    {
                        options.UseDefaultCredentials = true;
                    })
                    .Build();
                connection.Closed += async error =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await connection.StartAsync();
                };
                await connection.StartAsync();
                //await connection.InvokeAsync("NotifyTempChange", model.TemperatureValue);
                await connection.InvokeAsync("NotifyTempChange", model);
                // await connection.InvokeAsync("SendRefresh");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.LogInformation(ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Helper Classes  
        public class OpcResultRoot
        {
            public List<WriteResult> writeResults { get; set; }
        }

        public class WriteResult
        {
            public string id { get; set; }
            public bool s { get; set; }
            public string r { get; set; }
        }
        #endregion
    }
}