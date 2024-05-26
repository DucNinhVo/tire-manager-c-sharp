using BlazorApp.Data;
using BlazorApp.DB;
using BlazorApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
   
public class Tire {
    public int TireId { get; set; }
    public int TireSetId { get; set; }
    public double Pressure { get; set; }
    public double TireTemperature { get; set; }
    public string Position { get; set; }
    public int CarId {get;set;}
	public TireType Type {get;set;}
    public int RaceId { get; set; }
    public string Status { get; set; }
	
	public bool Sipped {get;set;}
	public bool Grooved {get;set;} //Kann beides sein

    public Tire(int tireID, int tireSetID, double Pressure, double Temperature, string Position)
    {
        this.TireId = tireID;
        this.TireSetId = tireSetID;
        this.Pressure = Pressure;
        this.TireTemperature = Temperature;       
        this.Position = Position;
        Race.refreshCurrentRace();
        this.RaceId = Race.CurrentRace.ID;
    } 
    public Tire(int tireID, int tireSetID, double Pressure, double Temperature, string Position,string status,  int raceID, int type)
    {
        
        this.TireId = tireID;
        this.TireSetId = tireSetID;
        this.Pressure = Pressure;
        this.TireTemperature = Temperature;       
        this.Position = Position;
        this.Status = status;
        this.RaceId = raceID;
        this.Type = new TireType(type);
    }
    public Tire(int tireID, int tireSetID,  string Position,  TireType type, int raceId, string status, bool siped, bool grooved)
    {
        this.TireId = tireID;
        this.TireSetId = tireSetID;
        this.Position = Position;
        this.Type = type;
        this.RaceId = raceId;
        this.Status = status;
        this.Sipped = siped;
        this.Grooved = grooved;
    }

    public Tire(int CarID, int TypeID, double Pressure, double TireTemperature, bool Sipped, bool Grooved, string Position)
    {
        this.CarId = CarID;
        this.Type = new TireType(TypeID);
        this.Pressure = Pressure;
        this.TireTemperature = TireTemperature;
        this.Sipped = Sipped;
        this.Grooved = Grooved;
        this.Position = Position;
        Race.refreshCurrentRace();
        this.RaceId = Race.CurrentRace.ID;
    }

    public Tire(int CarID, TireType Type, double Pressure, double TireTemperature, bool Sipped, bool Grooved, string Position)
    {
        this.CarId = CarID;
        this.Type = Type;
        this.Pressure = Pressure;
        this.TireTemperature = TireTemperature;
        this.Sipped = Sipped;
        this.Grooved = Grooved;
        this.Position = Position;
        Race.refreshCurrentRace();
        this.RaceId = Race.CurrentRace.ID;
    }

    public Tire(int CarID, int TypeID, bool Grooved, string Position)
    {
        this.Type = new TireType(TypeID);
        this.Grooved = Grooved;
        this.CarId = CarID;
        this.Position = Position;
        Race.refreshCurrentRace();
        this.RaceId = Race.CurrentRace.ID;
    }

    public Tire(int CarID, TireType Type, bool Grooved, string Position)
    {
        this.Type = Type;
        this.Grooved = Grooved;
        this.CarId = CarID;
        this.Position = Position;
        Race.refreshCurrentRace();
        this.RaceId = Race.CurrentRace.ID;
    }

    public void calculatePressure() 
    {
        if (Pressure == 0)
            return;
        Parameter formula = DataBase.loadParameterByName("FORMULA");
        if (formula.parameterText.isNullOrEmpty())
            return;

        var splitted = formula.parameterText.Split(" ");
        List<string> validList = new List<string>() { "{REIFENDRUCK}", "{FELGENTEMP}", "{STRECKENTEMP}", "{LUFTTEMP}", "+", "-", "*", "/", "(", ")" };

        //pr�fen, ob Formel g�ltig ist
        foreach (var item in splitted)
        {
            if (!validList.Contains(item))
            {
                if (!Double.TryParse(item, out _))
                {
                    return;
                }
            }
        }
        string[] splittedTemp = (string[])splitted.Clone();

        Weather currentWeather = DataBase.loadWeatherAll().LastOrDefault();

        //Parameter durch Werte ersetzen
        for (int i = 0; i < splittedTemp.Length; i++)
        {
            switch (splittedTemp[i])
            {
                case "{REIFENDRUCK}":
                    splitted[i] = Pressure.ToString();
                    break;
                case "{FELGENTEMP}":
                    splitted[i] = TireTemperature.ToString();
                    break;
                case "{STRECKENTEMP}":
                    if (currentWeather == null)
                    {
                        return;
                    }
                    splitted[i] = currentWeather.AsphaltTemp.ToString();
                    break;
                case "{LUFTTEMP}":
                    if (currentWeather == null)
                    {
                        return;
                    }
                    splitted[i] = currentWeather.AirTemp.ToString();
                    break;
                default:
                    break;
            }
        }

        var newPressure = calcFunction(splitted);
        if (newPressure != 0)
        {
            Pressure =(float) Math.Round(newPressure, 2);
        }
    }

    private double solveBrackets(string[] function)
    {
        var splitted = function.SplitWithInnerBrackets();
        var first = splitted.First();
        var last = splitted.Last();
        var current = splitted.ElementAt(1);

        string[] solved  = new string[] { calcFunction(current).ToString() };
        
        if(first.Contains("(") || last.Contains("("))
        {
            return solveBrackets(first.Concat(solved).ToArray().Concat(last).ToArray());
        }
        else
        {
            return calcFunction(first.Concat(solved).ToArray().Concat(last).ToArray());
        }

    }

    private double calcFunction(string[] function)
    {
        //Klammern zuerst
        if (function.Contains("("))
        {
            return solveBrackets(function);
        }
        //jetzt Punkt vor Strich
        if (function.Contains("*") || function.Contains("/"))
        {
            int indexPoint = Int32.MaxValue;
            if (function.Contains("*"))
            {
                indexPoint = Array.IndexOf(function, "*");
            }
            if (function.Contains("/") && Array.IndexOf(function, "/") < indexPoint)
            {
                indexPoint = Array.IndexOf(function, "/");
            }
            var firstSplitPoint = function.SplitAt(indexPoint - 1);
            var indexNewPoint = Int32.MaxValue;
            if (firstSplitPoint.Last().Contains("*"))
            {
                indexNewPoint = Array.IndexOf(firstSplitPoint.Last(), "*");
            }
            if (firstSplitPoint.Last().Contains("/") && Array.IndexOf(firstSplitPoint.Last(), "/") < indexNewPoint)
            {
                indexNewPoint = Array.IndexOf(firstSplitPoint.Last(), "/");
            }
            var secondSplit = firstSplitPoint.Last().SplitAt(indexNewPoint + 2);
            var firstPoint = firstSplitPoint.First();
            var lastPoint = secondSplit.Last();
            var currentPoint = secondSplit.First();
            if ((firstPoint == null || firstPoint.Count() == 0) && lastPoint.Count() == 0)
            {

                if (currentPoint.Count() != 3)
                    return 0;
                return calcShort(currentPoint[0], currentPoint[1], currentPoint[2]);
            }
            else
            {
                if (firstPoint == null || firstPoint.Count() == 0)
                {
                    string[] newFunc = new string[] { calcFunction(currentPoint).ToString() };
                    return calcFunction(newFunc.Concat(lastPoint).ToArray());
                }
                else if (lastPoint.Count() == 0)
                {
                    string[] newFunc = new string[] { calcFunction(currentPoint).ToString() };
                    return calcFunction(firstPoint.Concat(newFunc).ToArray());
                }
                else
                {
                    string[] newFunc = new string[] { calcFunction(currentPoint).ToString() };
                    return calcFunction(firstPoint.Concat(newFunc).ToArray().Concat(lastPoint).ToArray());
                }
            }
        }
        //jetzt Strich
        if (function.Contains("+") || function.Contains("-"))
        {
            var split = function.SplitAt(3);
            var first = split.First();
            var last = split.Last();
            if (last.Count() == 0)
            {
                return calcShort(first[0], first[1], first[2]);
            }
            else
            {
                string[] newFunc = new string[] { calcFunction(first).ToString() };
                return calcFunction(newFunc.Concat(last).ToArray());
            }
        }

        return 0;
    }
    private double calcShort(string first, string operation, string last)
    {
        double one = 0;
        Double.TryParse(first, out one);
        double two = 0;
        Double.TryParse(last, out two);

        switch (operation)
        {
            case "+":
                return one + two;
            case "-":
                return one - two;
            case "*":
                return one * two;
            case "/":
                return one / two;
            default:
                return 0;
        }
    }

    public void Sippe()
    {
        this.Sipped = true;
    }
}
