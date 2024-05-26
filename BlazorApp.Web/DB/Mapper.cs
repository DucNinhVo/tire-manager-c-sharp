using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using Microsoft.Data.Sqlite;

namespace BlazorApp.DB
{
    public static class Mapper
    {

        public static User mapUser(SqliteDataReader reader)
        {
            int id = 0;
            string userName = "" ;
            string password = "";
            int priviledges = 0;

            if (!reader.IsDBNull(0))
            id = reader.GetInt32(0);

            if(!reader.IsDBNull(1))
             userName = reader.GetString(1);

            if (!reader.IsDBNull(2))
                password = reader.GetString(2);

            if (!reader.IsDBNull(3))
                priviledges = reader.GetInt32(3);

            
            User user = new User(userName, id, priviledges, password);
            
            user.Password = password;

            return user;
        }


        public static Race mapRace(SqliteDataReader reader)
        {
            Race race = new Race("");

            if (!reader.IsDBNull(0))
                race.ID = reader.GetInt32(0);
            if (!reader.IsDBNull(1))
                race.Name = reader.GetString(1);
            if (!reader.IsDBNull(2))
                race.Datum = reader.GetDateTime(2);
            if (!reader.IsDBNull(3))
                race.PreFix_SlickCold = reader.GetString(3);
            if (!reader.IsDBNull(4))
                race.SlickCold = reader.GetInt32(4);
            if (!reader.IsDBNull(5))
                race.PreFix_SlickMedium = reader.GetString(5);
            if (!reader.IsDBNull(6))
                race.SlickMedium = reader.GetInt32(6);
            if (!reader.IsDBNull(7))
                race.PreFix_SlickHot = reader.GetString(7);
            if (!reader.IsDBNull(8))
                race.SlickHot = reader.GetInt32(8);
            if (!reader.IsDBNull(9))
                race.PreFix_Inters = reader.GetString(9);
            if (!reader.IsDBNull(10))
                race.Inters = reader.GetInt32(10);
            if (!reader.IsDBNull(11))
                race.PreFix_RainDryWet = reader.GetString(11);
            if (!reader.IsDBNull(12))
                race.RainDryWet = reader.GetInt32(12);
            if (!reader.IsDBNull(13))
                race.PreFix_RainHeavyWet = reader.GetString(14);
            if (!reader.IsDBNull(14))
                race.RainHeavyWet = reader.GetInt32(14);

            return race;
        }

        public static Car mapCar(SqliteDataReader reader)
        {
            //ToDo: Car.Name?
            int id = 0;
            if (!reader.IsDBNull(0))
                id = reader.GetInt32(0);
            Car car = new Car(id);

            if (!reader.IsDBNull(1))
                car.Name = reader.GetString(1);
            return car;
        }


        public static Parameter mapParameter(SqliteDataReader reader)
        {
            Parameter parameter = new Parameter();
            if (!reader.IsDBNull(0))
                parameter.parameterId = reader.GetInt32(0);
            if (!reader.IsDBNull(1))
                parameter.parameterName = reader.GetString(1);
            if (!reader.IsDBNull(2))
                parameter.parameterText = reader.GetString(2);

            return parameter;
        }

        public static Driver mapDriver(SqliteDataReader reader)
        {
            Driver driver = new Driver();
            if (!reader.IsDBNull(0))
                driver.ID = reader.GetInt32(0);
            if (!reader.IsDBNull(1))
                driver.Name = reader.GetString(1);

            return driver;
        }

        public static Weather mapWeather(SqliteDataReader reader)
        {
            int id = 0;
            DateTime time =  DateTime.Now;
            double asphaltTemp = 0.0;
            double airTemp = 0.0;           
            string weatherCond = "";

            if (!reader.IsDBNull(0))
                id = reader.GetInt32(0);

            if (!reader.IsDBNull(1))
                time = reader.GetDateTime(1);

            if (!reader.IsDBNull(3))
                asphaltTemp = reader.GetDouble(3);

            if (!reader.IsDBNull(2))
                airTemp = reader.GetDouble(2);
        
            if (!reader.IsDBNull(4))
                weatherCond = reader.GetString(4);

            Weather weather = new Weather(id,time,asphaltTemp,airTemp,weatherCond);
            return weather;

        }

        public static TireSet mapTireSetOrder(SqliteDataReader reader)
        {
            int id = 0;
            int typeId = 0;
            int tireVar = 0;
            string orderDate = "";
            string orderTime = "";
            string pickupTime = "";
            string note = "";

            if (!reader.IsDBNull(0))
                id = reader.GetInt32(0);

            if (!reader.IsDBNull(1))
                typeId = reader.GetInt32(1);

            if (!reader.IsDBNull(2))
                tireVar = reader.GetInt32(2);

            if (!reader.IsDBNull(3))
                orderDate = reader.GetString(3);

            if (!reader.IsDBNull(4))
                orderTime = reader.GetString(4);

            if (!reader.IsDBNull(5))
                pickupTime = reader.GetString(5);

            if (!reader.IsDBNull(6))
                note = reader.GetString(6);

            TireSet tireset = new TireSet(id, typeId, tireVar, orderDate, orderTime, pickupTime, note);
            return tireset;
        }

        public static TireSet mapTireSetHeating(SqliteDataReader reader)
        {
            int id = 0;
            double startingTemp = 0.0;
            double heatingTemp = 90;
            int duration = 90;
            string startTime = "";
            string endTime = "";
            

            if (!reader.IsDBNull(0))
                id = reader.GetInt32(0);

            if (!reader.IsDBNull(1))
                startingTemp = reader.GetDouble(1);

            if (!reader.IsDBNull(2))
                heatingTemp = reader.GetDouble(2);

            if (!reader.IsDBNull(3))
                duration = reader.GetInt32(3);

            if (!reader.IsDBNull(4))
                startTime = reader.GetString(4);

            if (!reader.IsDBNull(5))
                endTime = reader.GetString(5);

        

            TireSet tireset = new TireSet(id, startingTemp, heatingTemp, duration, startTime, endTime);
            return tireset;
        }

        public static TireSet mapTireSet(SqliteDataReader reader)
        {
            int id = 0;
            int typeId = 0;
            int tireVar = 0;
            string orderDate = "";
            string orderTime = "";
            string pickupTime = "";
            double startingTemp = 0;
            double heatingTemp = 90;
            int duration = 90;
            string startTime = "";
            string endTime = "";
            string note = "";


            if (!reader.IsDBNull(0))
                id = reader.GetInt32(0);

            if (!reader.IsDBNull(1))
                typeId = reader.GetInt32(1);

            if (!reader.IsDBNull(2))
                tireVar = reader.GetInt32(2);

            if (!reader.IsDBNull(3))
                orderDate = reader.GetString(3);

            if (!reader.IsDBNull(4))
                orderTime = reader.GetString(4);

            if (!reader.IsDBNull(5))
                pickupTime = reader.GetString(5);

            if (!reader.IsDBNull(6))
                startingTemp = reader.GetDouble(6);

            if (!reader.IsDBNull(7))
                heatingTemp = reader.GetDouble(7);

            if (!reader.IsDBNull(8))
                duration = reader.GetInt32(8);

            if (!reader.IsDBNull(9))
                startTime = reader.GetString(9);

            if (!reader.IsDBNull(10))
                endTime = reader.GetString(10);

            if (!reader.IsDBNull(11))
                note = reader.GetString(11);

            TireSet tireset = new TireSet(id, typeId, tireVar, orderDate, orderTime, pickupTime, startingTemp, heatingTemp, duration, startTime, endTime, note);
            return tireset;
        }

        public static TireType mapTireType(SqliteDataReader reader)
        {
            int id = 0;
            string type = "";
            string description = "";

            if (!reader.IsDBNull(0))
                id = reader.GetInt32(0);

            if (!reader.IsDBNull(1))
                type = reader.GetString(1);

            if (!reader.IsDBNull(2))
                description = reader.GetString(2);

            TireType tireType = new TireType(id, type, description);
            return tireType;
        }

        public static Tire mapTire(SqliteDataReader reader)
        {
            int id = 0;
            int setId = 0;
            double press = 0.0;
            double temp = 0.0;
            string status = "";
            int raceId = 0;
            int type = 0;
            
            string position = "";

            if (!reader.IsDBNull(0))
                id = reader.GetInt32(0);

            if (!reader.IsDBNull(1))
                setId = reader.GetInt32(1);

            if (!reader.IsDBNull(2))
                press = reader.GetDouble(2);

            if (!reader.IsDBNull(3))
                temp = reader.GetDouble(3);
         
            if (!reader.IsDBNull(4))
                position = reader.GetString(4);

            if (!reader.IsDBNull(5))
                status = reader.GetString(5);

            if (!reader.IsDBNull(6))
                raceId = reader.GetInt32(6);

            if (!reader.IsDBNull(7))
                type = reader.GetInt32(7);


            Tire tire = new Tire(id,setId,press,temp,position, status , raceId, type);
            tire.RaceId = raceId;
            tire.Status = status;
            return tire;
        }
    }
}
