using System;
using System.Collections.Generic;

public class Car {
	public int ID {get;}
	public string Name { get; set; }
	public List<String> Driver = new List<String>();
	public Car(int ID)
	{
		this.ID = ID;
	}
	
	public Car(int ID,List<String> Driver)
	{
		this.ID = ID;
		this.Driver = Driver;
	}
	
	public void addDriver(String Driver)
	{
		this.Driver.Add(Driver);
	}
	
	public void deleteDriver(String Driver)
	{
		this.Driver.Remove(Driver);
	}
	
	public List<String> getListOfDrivers()
	{
		return Driver;
	}
	//Eventuell getDriver Funktion
	
}