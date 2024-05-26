using System;
public class Session {
	public Session()
	{
		LogInTime = DateTime.Now;
	}

	public Session(DateTime LogInTime)
	{
		this.LogInTime = LogInTime;
	}
	public DateTime LogInTime{get;set;}

}
