using System;
using System.Timers;

public class Manager 
{
	public class TireTimer
	{
		DateTime TimeFromDatabase = new DateTime();
		public String Name;
		public Timer Watch;

		public TireTimer(String Name,Timer Watch)
		{
			this.Name = Name;
			this.Watch = Watch;
		}

		public long getTimeRemaining()
		{
			//TimeFromDataBase = getTimeFromDatabase();
			DateTime TimerStart = TimeFromDatabase; 
			long TimeRemaining = ((DateTimeOffset) DateTime.Now).ToUnixTimeMilliseconds() - ((DateTimeOffset) TimerStart).ToUnixTimeMilliseconds();
			return TimeRemaining;
		}
	}
	
	public static int WheelSetCounter{get;private set;}
	
	public void setWheelSetCounter(int Count)
	{
		WheelSetCounter = Count;
	}
}
