using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebMonitor.Domain
{
	public class Monitor
	{
		public Guid Id { get; protected set; }
		public Guid AccountId { get; protected set; }
		public string Name { get; set; }
		public string EndPoint { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? LastExecutionDate { get; set; }
		public bool IsOnline { get; set; }
		public TimeSpan Interval { get; set; }
		public DateTime NextExecutionTime { get; protected set;	}

		private static HttpClient httpClient = new HttpClient();

		protected Monitor()
		{			
		}

		protected DateTime RecalculateNextExecutionTime()
		{
			if (LastExecutionDate == null)
			{
				return DateTime.UtcNow;
			}

			return LastExecutionDate.Value + Interval;
		}

		public Monitor(Guid accountId, string name, string endPoint, TimeSpan interval)
		{
			Id = Guid.NewGuid();
			AccountId = accountId;
			Name = name;
			EndPoint = endPoint;
			CreatedDate = DateTime.UtcNow;
			LastExecutionDate = null;
			Interval = interval;
		}

		public async Task Refresh()
		{
			// Send GET to the end point
			try
			{
				HttpResponseMessage result = await httpClient.GetAsync(EndPoint);
				IsOnline = result.StatusCode == System.Net.HttpStatusCode.OK;
				LastExecutionDate = DateTime.UtcNow;
				NextExecutionTime = RecalculateNextExecutionTime();
			}
			catch(Exception )
			{
				IsOnline = false;
			}
		}		
	}
}
