using BlazorApp.DB;
using System;

public class Weather {

    public int WeatherId { get; set; }
    public DateTime AddWeatherDate { get; set; }
    public double AsphaltTemp { get; set; }
    public double AirTemp { get; set; }
    public string WeatherCond { get; set; }
    public static Weather CurrentWeather{get;set;} // = Database.loadNewestWeather;

    public Weather(int id, DateTime time, double asphaltTemp, double airTemp, string weatherCond) 
    {
        this.WeatherId = id;
        this.AddWeatherDate = time;
        this.AsphaltTemp = asphaltTemp;
        this.AirTemp = airTemp ;
        this.WeatherCond = weatherCond;
        CurrentWeather = this;
    }

}