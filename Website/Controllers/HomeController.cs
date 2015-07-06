using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreateJob()
        {
            var jobId = Guid.NewGuid();

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["QueueStorageConnectionString"].ConnectionString);
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();

            var queue = cloudQueueClient.GetQueueReference("jobqueue");
            queue.CreateIfNotExists();

            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(jobId));
            await queue.AddMessageAsync(message);

            return RedirectToAction("JobStatus", new {jobId});
        }

        public ActionResult JobStatus(Guid jobId)
        {
            var viewModel = new JobStatusViewModel
            {
                JobId = jobId
            };

            return View(viewModel);
        }

        public ActionResult ProgressNotification(Guid jobId, int progress)
        {
            var connections = JobProgressHub.GetUserConnections(jobId);

            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    // Notify the client to refresh the list of connections
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<JobProgressHub>();
                    hubContext.Clients.Clients(new[] { connection }).updateProgress(progress);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}