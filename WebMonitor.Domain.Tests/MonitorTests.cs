using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace WebMonitor.Domain.Tests
{
	[TestClass]
	public class MonitorTests
	{

		private Monitor _monitor;

		[TestInitialize]
		public void Init()
		{
			_monitor = GetDefaultMonitor();
		}

		private Monitor GetDefaultMonitor()
		{
			Guid accountId = Guid.NewGuid();
			string name = "Anna's";
			string endPoint = "https://google.com";
			TimeSpan interval = TimeSpan.FromMinutes(1); // interval = 1 minute

			var monitor = new Monitor(accountId, name, endPoint, interval);

			return monitor;
		}

		[TestMethod]
		public void WhenMonitorCreated_ThenItHasIdAssignedAndPropertiesCreated()
		{
			Guid accountId = Guid.NewGuid();
			string name = "Anna's";
			string endPoint = "https://google.com";
			TimeSpan interval = TimeSpan.FromMinutes(1);

			var monitor = new Monitor(accountId, name, endPoint, interval);

			Assert.AreNotEqual(Guid.Empty, monitor.Id);
			Assert.AreEqual(accountId, monitor.AccountId);
			Assert.AreEqual(name, monitor.Name);
			Assert.AreEqual(endPoint, monitor.EndPoint);
			Assert.IsTrue(monitor.CreatedDate <= DateTime.UtcNow);
			Assert.IsNull(monitor.LastExecutionDate);
			Assert.AreEqual(TimeSpan.FromMinutes(1), monitor.Interval);
			Assert.IsFalse(monitor.IsOnline);
		}

		[TestMethod]
		public void WhenRefreshCurrentStatusCalledAndEndPointOnline_ThenIsOnlineEqualsTrue()
		{
			_monitor.Refresh().Wait();
			Assert.IsTrue(_monitor.IsOnline);
		}

		[TestMethod]
		public void WhenRefreshCurrentStatusCalledAndEndPointOffline_ThenIsOnlineEqualsFalse()
		{
			_monitor.EndPoint = "https://r12rrrrr.com";

			_monitor.Refresh().Wait();
			Assert.IsFalse(_monitor.IsOnline);
		}

		[TestMethod]
		public void WhenMonitorCreated_ThenNextExecutionTimeIsInPast()
		{
			Assert.IsTrue(_monitor.NextExecutionTime <= DateTime.UtcNow);
		}

		[TestMethod]
		public void WhenRefreshCurrentStatusCalled_ThenLastExecutionTimeUpdated()
		{
			_monitor.Refresh().Wait();

			Assert.IsNotNull(_monitor.LastExecutionDate);
			Assert.IsTrue(_monitor.LastExecutionDate < DateTime.UtcNow);
		}

		[TestMethod]
		public void WhenRefreshCurrentStatusCalled_ThenNextExecutionTimeUpdated()
		{
			_monitor.Refresh().Wait();
			
			Assert.IsNotNull(_monitor.NextExecutionTime);
			Assert.AreEqual(_monitor.NextExecutionTime, _monitor.LastExecutionDate + _monitor.Interval);
		}

	}
}
