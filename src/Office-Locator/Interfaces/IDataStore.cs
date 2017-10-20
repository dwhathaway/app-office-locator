using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using OfficeLocator.Model;

namespace OfficeLocator
{
	public interface IDataStore
	{
		Task Init();
		Task<IEnumerable<Location>> GetLocationsAsync();
		Task<Location> AddLocationAsync (Location office);
		Task<bool> RemoveLocationAsync(Location office);
		Task<Location> UpdateLocationAsync (Location office);
		Task<Feedback> AddFeedbackAsync(Feedback feedback);
		Task<IEnumerable<Feedback>> GetFeedbackAsync();
		Task<bool> RemoveFeedbackAsync(Feedback feedback);
		Task SyncLocationsAsync();
		Task SyncFeedbacksAsync();
	}
}

