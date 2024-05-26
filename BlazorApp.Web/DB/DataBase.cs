using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using Microsoft.Data.Sqlite;
using BlazorApp.Extensions;

namespace BlazorApp.DB
{
    public static class DataBase
    {
        static SqliteConnection conn = new SqliteConnection("Data Source=.\\DB\\LandMotorsport.db");

        public static int getSequence(string TableName)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT seq FROM sqlite_sequence WHERE name = '{TableName}'";

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();

                    if (!reader.IsDBNull(0))
                        return reader.GetInt32(0) + 1;

                    return 0;
                }
            }
        }


        #region Users
        public static User loadUserById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Users WHERE userId = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapUser(reader);
                    }
                }

                return null;
            }

        }
        public static User loadUserByUserName(string name)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Users WHERE userName = '{name}'";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapUser(reader);
                    }
                }
                return null;
            }
        }

        //alle Nutzer einer Gruppe (lädt anhand der Nutzerrechte)
        public static List<User> loadUsersByGroup(int group)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Users WHERE userrights = {group}";

                List<User> userList = new List<User>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = Mapper.mapUser(reader);
                        if (user != null)
                            userList.Add(user);
                    }
                }
                return userList;
            }
        }
        public static List<User> loadUsersAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Users";

                List<User> userList = new List<User>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = Mapper.mapUser(reader);
                        if (user != null)
                            userList.Add(user);
                    }
                }
                return userList;
            }
        }

        public static void saveUser(User user)
        {
            //prüfen, ob ob user schon existiert
            User testUser = loadUserById(user.UserID);
            User testUserName = loadUserByUserName(user.UserName);
            if (testUserName == null || testUser == testUserName)
            {
                using (conn)
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        var command = conn.CreateCommand();
                        if (testUser == null || testUser.UserName.isNullOrEmpty())
                        {
                            command.CommandText =
                        @"INSERT INTO Users (userId, userName,  password, userrights) VALUES (@UserId, @UserName, @Password, @Userrights)";
                        }
                        else
                        {
                            command.CommandText =
                        @"UPDATE  Users SET userName = @UserName, password = @Password, userrights =  @Userrights WHERE userId = @UserId ";
                        }

                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@UserId";
                        parameter.SqliteType = SqliteType.Integer;
                        parameter.Value = user.UserID;
                        command.Parameters.Add(parameter);

                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@UserName";
                        parameter.SqliteType = SqliteType.Text;
                        if (user.UserName.isNotNullOrEmpty())
                        {
                            parameter.Value = user.UserName;
                        }
                        else
                        {
                            parameter.Value = "";
                        }

                        command.Parameters.Add(parameter);

                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@Password";
                        parameter.SqliteType = SqliteType.Text;
                        if (user.Password != null)
                        {
                            parameter.Value = user.Password;
                        }
                        else
                        {
                            parameter.Value = "";
                        }
                        command.Parameters.Add(parameter);

                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@Userrights";
                        parameter.SqliteType = SqliteType.Integer;
                        parameter.Value = user.Priviledges;
                        command.Parameters.Add(parameter);

                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
            else
            {
                //ToDo: Message senden
            }
        }

        public static void deleteUser(User user)
        {
            User testUser = loadUserById(user.UserID);
            if (testUser == null)
            {
                return;
            }

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    command.CommandText =
                @"DELETE FROM Users WHERE userId = @UserId";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@UserId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = user.UserID;
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
        #endregion

        #region Cars
        public static Car loadCarById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Cars WHERE carId = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapCar(reader);
                    }
                }

                return null;
            }
        }

        public static List<Car> loadCarsAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Cars";

                List<Car> carList = new List<Car>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        carList.Add(Mapper.mapCar(reader));
                    }
                }

                return carList;
            }
        }

        public static void saveCar(Car car)
        {
            //prüfen, ob ob user schon existiert
            Car testCar = loadCarById(car.ID);

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    if (testCar == null)
                    {
                        command.CommandText =
                    @"INSERT INTO Cars (carId, carName) VALUES (@CarId, @CarName)";
                    }
                    else
                    {
                        command.CommandText =
                    @"UPDATE  Cars SET carName = @CarName WHERE carId = @CarId ";
                    }

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@CarId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = car.ID;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@CarName";
                    parameter.SqliteType = SqliteType.Text;
                    //ToDo: car.name
                    //if (car.Name.isNotNullOrEmpty())
                    //{
                    //    parameter.Value = user.UserName;
                    //}
                    //else
                    //{
                    parameter.Value = "";
                    //}
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public static void deleteCar(Car car)
        {
            Car testCar = loadCarById(car.ID);
            if (testCar == null)
            {
                return;
            }

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    command.CommandText =
                @"DELETE FROM Cars WHERE carId = @CarId";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@CarId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = car.ID;
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region Races 
        //ToDo: Races mapping und Funktionen
        public static Race loadRaceById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Races WHERE raceId = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapRace(reader);
                    }
                }

                return null;
            }
        }
        public static List<Race> loadRacesByName(string name)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Races WHERE raceName = '{name}'";

                List<Race> raceList = new List<Race>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        raceList.Add(Mapper.mapRace(reader));
                    }
                }

                return raceList;
            }
        }
        public static List<Race> loadRacesAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Races";

                List<Race> raceList = new List<Race>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        raceList.Add(Mapper.mapRace(reader));
                    }
                }

                return raceList;
            }
        }


        public static void saveRace(Race race)
        {
            Race testRace = loadRaceById(race.ID);

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    if (testRace == null)
                    {
                        command.CommandText =
                    @"INSERT INTO Races (raceId, raceName, raceDate, prefixSlickCold, counterSlickCold, 
                        prefixSlickMedium, counterSlickMedium, prefixSlickHot, counterSlickHot, 
                        prefixInters, counterInters , prefixRainDryWet, counterRainDryWet, prefixRainHeavyWet, counterRainHeavyWet) 
                        VALUES (@RaceId, @RaceName, @RaceDate, @PrefixSlickCold, @CounterSlickCold, @PrefixSlickMedium, @CounterSlickMedium, @PrefixSlickHot, @CounterSlickHot, 
                        @PrefixInters, @CounterInters, @PrefixRainDryWet, @CounterRainDryWet, @PrefixRainHeavyWet, @CounterRainHeavyWet)";
                    }
                    else
                    {
                        command.CommandText =
                    @"UPDATE  Races SET raceName  = @RaceName, raceDate = @RaceDate, prefixSlickCold = @PrefixSlickCold, counterSlickCold = @CounterSlickCold, 
                        prefixSlickMedium = @PrefixSlickMedium, counterSlickMedium = @CounterSlickMedium, prefixSlickHot = @PrefixSlickHot, counterSlickHot = @CounterSlickHot, 
                        prefixInters = @PrefixInters, counterInters = @PrefixInters , prefixRainDryWet = @PrefixRainDryWet, counterRainDryWet = @CounterRainDryWet,
                        prefixRainHeavyWet = @PrefixRainHeavyWet, counterRainHeavyWet = @CounterRainHeavyWet WHERE raceId = @RaceId ";
                    }

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@RaceId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = race.ID;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@RaceName";
                    parameter.SqliteType = SqliteType.Text;
                    if (race.Name.isNotNullOrEmpty())
                    {
                        parameter.Value = race.Name;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@RaceDate";
                    parameter.SqliteType = SqliteType.Text;
                    if (race.Datum != DateTime.MinValue)
                    {
                        parameter.Value = race.Datum;
                    }
                    else
                    {
                        parameter.Value = DateTime.Now;
                    }

                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@PrefixSlickCold";
                    parameter.SqliteType = SqliteType.Text;
                    if (race.PreFix_SlickCold.isNotNullOrEmpty())
                    {
                        parameter.Value = race.PreFix_SlickCold;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@CounterSlickCold";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = race.SlickCold;

                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@PrefixSlickMedium";
                    parameter.SqliteType = SqliteType.Text;
                    if (race.PreFix_SlickMedium.isNotNullOrEmpty())
                    {
                        parameter.Value = race.PreFix_SlickMedium;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@CounterSlickMedium";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = race.SlickMedium;


                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@PrefixSlickHot";
                    parameter.SqliteType = SqliteType.Text;
                    if (race.PreFix_SlickHot.isNotNullOrEmpty())
                    {
                        parameter.Value = race.PreFix_SlickHot;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@CounterSlickHot";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = race.SlickHot;

                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@PrefixInters";
                    parameter.SqliteType = SqliteType.Text;
                    if (race.PreFix_Inters.isNotNullOrEmpty())
                    {
                        parameter.Value = race.PreFix_Inters;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);


                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@CounterInters";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = race.Inters;


                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@PrefixRainDryWet";
                    parameter.SqliteType = SqliteType.Text;
                    if (race.PreFix_RainDryWet.isNotNullOrEmpty())
                    {
                        parameter.Value = race.PreFix_RainDryWet;
                    }
                    else
                    {
                        parameter.Value = "";
                    }


                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@CounterRainDryWet";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = race.RainDryWet;
                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@PrefixRainHeavyWet";
                    parameter.SqliteType = SqliteType.Text;
                    if (race.PreFix_RainHeavyWet.isNotNullOrEmpty())
                    {
                        parameter.Value = race.PreFix_RainHeavyWet;
                    }
                    else
                    {
                        parameter.Value = "";
                    }


                    command.Parameters.Add(parameter);
                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@CounterRainHeavyWet";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = race.RainHeavyWet;
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public static void deleteRace(Race race)
        {
            Race testRace = loadRaceById(race.ID);
            if (testRace == null)
            {
                return;
            }

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    command.CommandText =
                @"DELETE FROM Races WHERE raceId = @RaceId";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@RaceId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = race.ID;
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region Parameter

        public static Parameter loadParameterById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Parameter WHERE parameterId = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapParameter(reader);
                    }
                }

                return null;
            }
        }

        public static Parameter loadParameterByName(string name)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Parameter WHERE parameterName = '{name}'";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapParameter(reader);
                    }
                }

                return null;
            }
        }

        public static List<Parameter> loadParameterAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Paramerter";

                List<Parameter> paramList = new List<Parameter>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        paramList.Add(Mapper.mapParameter(reader));
                    }
                }

                return paramList;
            }
        }

        public static void saveParameter(Parameter param)
        {
            //prüfen, ob ob user schon existiert
            Parameter testParam = loadParameterById(param.parameterId);

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    if (testParam == null)
                    {
                        command.CommandText =
                    @"INSERT INTO Parameter (parameterId, parameterName, parameterText) VALUES (@ParamId, @ParamName, @ParamText)";
                    }
                    else
                    {
                        command.CommandText =
                    @"UPDATE  Parameter SET parameterName = @ParamName, parameterText = @ParamText WHERE parameterId = @ParamId ";
                    }

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@ParamId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = param.parameterId;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@ParamName";
                    parameter.SqliteType = SqliteType.Text;
                    if (param.parameterName.isNotNullOrEmpty())
                    {
                        parameter.Value = param.parameterName;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@ParamText";
                    parameter.SqliteType = SqliteType.Text;
                    if (param.parameterText.isNotNullOrEmpty())
                    {
                        parameter.Value = param.parameterText;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
        public static void deleteParameter(Parameter param)
        {
            Parameter testParam = loadParameterById(param.parameterId);
            if (testParam == null)
            {
                return;
            }

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    command.CommandText =
                @"DELETE FROM Parameter WHERE parameterId = @ParameterId";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@ParameterId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = param.parameterId;
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region Driver
        public static Driver loadDriverById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Drivers WHERE driverId = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapDriver(reader);
                    }
                }

                return null;
            }
        }
        public static Driver loadDriverByName(string name)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Drivers WHERE driverName = '{name}'";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapDriver(reader);
                    }
                }

                return null;
            }
        }

        public static List<Driver> loadDriversAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Driver";

                List<Driver> driverList = new List<Driver>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        driverList.Add(Mapper.mapDriver(reader));
                    }
                }

                return driverList;
            }
        }

        public static void saveDriver(Driver driver)
        {
            //prüfen, ob ob user schon existiert
            Driver testDriver = loadDriverById(driver.ID);

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    if (testDriver == null)
                    {
                        command.CommandText =
                    @"INSERT INTO Driver (driverId, driverName) VALUES (@DriverId, @DriverName)";
                    }
                    else
                    {
                        command.CommandText =
                    @"UPDATE  Driver SET driverName = @DriverName WHERE driverId = @DriverId ";
                    }

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@DriverId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = driver.ID;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@DriverName";
                    parameter.SqliteType = SqliteType.Text;
                    if (driver.Name.isNotNullOrEmpty())
                    {
                        parameter.Value = driver.Name;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
        public static void deleteDriver(Driver driver)
        {
            Driver testDriver = loadDriverById(driver.ID);
            if (testDriver == null)
            {
                return;
            }

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    command.CommandText =
                @"DELETE FROM Drivers WHERE driverId = @DriverId";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@DriverId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = driver.ID;
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region Weather
        public static Weather loadWeatherById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Weather WHERE weatherID = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapWeather(reader);
                    }
                }
                return null;
            }
        }
        public static void saveWeather(Weather weather)
        {
            //prüfen, ob ob user schon existiert
            Weather testWeather = loadWeatherById(weather.WeatherId);

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    if (testWeather == null)
                    {
                        command.CommandText =
                    @"INSERT INTO Weather (weatherID, addWeatherTime,  airTemperature, asphaltTemperature, weatherCondition) VALUES (@WeatherId, @AddWeatherTime, @AirTemp, @AsphaltTemp, @WeatherCond)";
                    }
                    else
                    {
                        command.CommandText =
                    @"UPDATE Weather SET addWeatherTime = @AddWeatherTime, asphaltTemperature = @AsphaltTemp, airTemperature = @AirTemp, weatherCondition = @WeatherCond WHERE weatherId = @WeatherId ";
                    }

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@WeatherId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = weather.WeatherId;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@AddWeatherTime";
                    parameter.SqliteType = SqliteType.Text;

                    parameter.Value = weather.AddWeatherDate;

                    command.Parameters.Add(parameter);


                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@AsphaltTemp";
                    parameter.SqliteType = SqliteType.Real;
                    if (weather.AsphaltTemp != 0.0)
                    {
                        parameter.Value = weather.AsphaltTemp;
                    }
                    else
                    {
                        parameter.Value = 0.0;
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@AirTemp";
                    parameter.SqliteType = SqliteType.Real;
                    if (weather.AirTemp != 0.0)
                    {
                        parameter.Value = weather.AirTemp;
                    }
                    else
                    {
                        parameter.Value = 0.0;
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@WeatherCond";
                    parameter.SqliteType = SqliteType.Text;
                    if (weather.WeatherCond.isNotNullOrEmpty())
                    {
                        parameter.Value = weather.WeatherCond;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }

        }
        public static List<Weather> loadWeatherAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Weather";

                List<Weather> weatherList = new List<Weather>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Weather weather = Mapper.mapWeather(reader);
                        if (weather != null)
                            weatherList.Add(weather);
                    }
                }
                return weatherList;
            }
        }

        #endregion

        #region TireSet
        public static TireSet loadTireSetById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM TireSet WHERE tireSetID = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapTireSet(reader);
                    }
                }
                return null;
            }
        }
        public static TireSet loadTireSetOrderById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT tireSetId, tireTypeId, tireVariant, orderDate, orderTime, pickUpTime, specicalNote FROM TireSet WHERE tireSetID = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapTireSetOrder(reader);
                    }
                }
                return null;
            }
        }
        public static TireSet loadTireSetHeatingById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT tireSetId, startingTemp, heatingTemp, heatingDuration, heatingStartTime, heatingEndTime, specialNote * FROM TireSet WHERE tireSetID = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapTireSetOrder(reader);
                    }
                }
                return null;
            }
        }
        public static List<TireSet> loadTireSetOrderAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT tireSetId, tireTypeId, tireVariant, orderDate, orderTime, pickUpTime, specialNote FROM TireSet";

                List<TireSet> tireSetList = new List<TireSet>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TireSet tireSet = Mapper.mapTireSetOrder(reader);
                        if (tireSet != null)
                            tireSetList.Add(tireSet);
                    }
                }
                return tireSetList;
            }
        }
        public static List<TireSet> loadTireSetHeatingAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT tireSetId, startingTemp, heatingTemp, heatingDuration, heatingStartTime, heatingEndTime, specialNote FROM TireSet";

                List<TireSet> tireSetList = new List<TireSet>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TireSet tireSet = Mapper.mapTireSetHeating(reader);
                        if (tireSet != null)
                            tireSetList.Add(tireSet);
                    }
                }
                return tireSetList;
            }
        }
        public static List<TireSet> loadTireSetAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM TireSet";

                List<TireSet> tireSetList = new List<TireSet>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TireSet tireSet = Mapper.mapTireSet(reader);
                        if (tireSet != null)
                            tireSetList.Add(tireSet);
                    }
                }
                return tireSetList;
            }
        }
        public static void saveTireSetOrder(TireSet tireSet)
        {
            //prüfen, ob ob user schon existiert
            TireSet testTireSet = loadTireSetById(tireSet.tireSetId);

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    if (testTireSet == null)
                    {
                        command.CommandText =
                    @"INSERT INTO TireSet (tireSetId, tireTypeId, tireVariant, orderDate, orderTime, pickUpTime, specialNote) VALUES (@tireSetId, @tireTypeId, @tireVariant, @orderDate, @orderTime, @pickUpTime, @specialNote)";
                    }
                    else
                    {
                        command.CommandText =
                    @"UPDATE TireSet SET tireTypeId = @tireTypeId,tireVariant = @tireVariant, orderDate = @orderDate, orderTime = @orderTime, pickUpTime = @pickUpTime, specialNote = @specialNote WHERE tireSetId = @tireSetId ";
                    }

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireSetId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = tireSet.tireSetId;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireTypeId";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.tireTypeId != 0)
                    {
                        parameter.Value = tireSet.tireTypeId;
                    }
                    else
                    {
                        parameter.Value = 0;
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireVariant";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = tireSet.tireVariant;

                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@orderDate";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.orderDate.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.orderDate;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@orderTime";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.orderTime.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.orderTime;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@pickUpTime";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.pickUpTime.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.pickUpTime;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@specialNote";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.specNote.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.specNote;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
        public static void saveTireSetHeating(TireSet tireSet)
        {
            //prüfen, ob ob user schon existiert
            TireSet testTireSet = loadTireSetById(tireSet.tireSetId);

            if (testTireSet != null)
            {
                using (conn)
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        var command = conn.CreateCommand();
                        command.CommandText = @"UPDATE TireSet SET startingTemp = @startingTemp, heatingTemp = @heatingTemp, heatingDuration = @heatingDuration, 
                        heatingStartTime = @heatingStartTime, heatingEndTime = @heatingEndTime WHERE tireSetId = @tireSetId";

                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@tireSetId";
                        parameter.SqliteType = SqliteType.Integer;
                        parameter.Value = tireSet.tireSetId;
                        command.Parameters.Add(parameter);

                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@startingTemp";
                        parameter.SqliteType = SqliteType.Real;
                        if (tireSet.startingTemp != 0.0)
                        {
                            parameter.Value = tireSet.startingTemp;
                        }
                        else
                        {
                            parameter.Value = 0.0;
                        }
                        command.Parameters.Add(parameter);


                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@heatingTemp";
                        parameter.SqliteType = SqliteType.Real;
                        if (tireSet.heatingTemp != 0.0)
                        {
                            parameter.Value = tireSet.heatingTemp;
                        }
                        else
                        {
                            parameter.Value = 90.0;
                        }
                        command.Parameters.Add(parameter);


                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@heatingDuration";
                        parameter.SqliteType = SqliteType.Integer;
                        if (tireSet.heatingDuration != 0)
                        {
                            parameter.Value = tireSet.heatingDuration;
                        }
                        else
                        {
                            parameter.Value = 90;
                        }
                        command.Parameters.Add(parameter);


                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@heatingStartTime";
                        parameter.SqliteType = SqliteType.Text;
                        if (tireSet.heatingStartTime.isNotNullOrEmpty())
                        {
                            parameter.Value = tireSet.heatingStartTime;
                        }
                        else
                        {
                            parameter.Value = "";
                        }
                        command.Parameters.Add(parameter);

                        parameter = command.CreateParameter();
                        parameter.ParameterName = "@heatingEndTime";
                        parameter.SqliteType = SqliteType.Text;
                        if (tireSet.heatingEndTime.isNotNullOrEmpty())
                        {
                            parameter.Value = tireSet.heatingEndTime;
                        }
                        else
                        {
                            parameter.Value = "";
                        }
                        command.Parameters.Add(parameter);

                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }
        public static void saveTireSet(TireSet tireSet)
        {
            //prüfen, ob ob user schon existiert
            TireSet testTireSet = loadTireSetById(tireSet.tireSetId);

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    if (testTireSet == null)
                    {
                        command.CommandText =
                    @"INSERT INTO TireSet (tireSetId, tireTypeId, tireVariant, orderDate, orderTime, pickUpTime, startingTemp, heatingTemp, heatingDuration, heatingStartTime, heatingEndTime, specialNote) 
                    VALUES (@tireSetId, @tireTypeId, @tireVariant, @orderDate, @orderTime, @pickUpTime, @startingTemp, @heatingTemp, @heatingDuration, @heatingStartTime, @heatingEndTime, @specialNote)";
                    }
                    else
                    {
                        command.CommandText =
                    @"UPDATE TireSet SET tireTypeId = @tireTypeId, tireVariant = @tireVariant, orderDate = @orderDate, orderTime = @orderTime, pickUpTime = @pickUpTime, startingTemp = @startingTemp, 
                    heatingTemp = @heatingTemp, heatingDuration = @heatingDuration, heatingStartTime = @heatingStartTime, heatingEndTime = @heatingEndTime, specialNote = @specialNote WHERE tireSetId = @tireSetId ";
                    }

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireSetId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = tireSet.tireSetId;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireTypeId";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.tireTypeId != 0)
                    {
                        parameter.Value = tireSet.tireTypeId;
                    }
                    else
                    {
                        parameter.Value = 0;
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireVariant";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = tireSet.tireVariant;

                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@orderDate";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.orderDate.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.orderDate;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@orderTime";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.orderTime.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.orderTime;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);


                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@pickUpTime";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.pickUpTime.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.pickUpTime;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@startingTemp";
                    parameter.SqliteType = SqliteType.Real;
                    if (tireSet.startingTemp != 0.0)
                    {
                        parameter.Value = tireSet.startingTemp;
                    }
                    else
                    {
                        parameter.Value = 0.0;
                    }
                    command.Parameters.Add(parameter);


                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@heatingTemp";
                    parameter.SqliteType = SqliteType.Real;
                    if (tireSet.heatingTemp != 0.0)
                    {
                        parameter.Value = tireSet.heatingTemp;
                    }
                    else
                    {
                        parameter.Value = 90.0;
                    }
                    command.Parameters.Add(parameter);


                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@heatingDuration";
                    parameter.SqliteType = SqliteType.Integer;
                    if (tireSet.heatingDuration != 0)
                    {
                        parameter.Value = tireSet.heatingDuration;
                    }
                    else
                    {
                        parameter.Value = 90;
                    }
                    command.Parameters.Add(parameter);


                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@heatingStartTime";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.heatingStartTime.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.heatingStartTime;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@heatingEndTime";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.heatingEndTime.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.heatingEndTime;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@specialNote";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireSet.specNote.isNotNullOrEmpty())
                    {
                        parameter.Value = tireSet.specNote;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region TireType
        public static TireType loadTireTypeById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM TireType WHERE tireTypeID = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapTireType(reader);
                    }
                }
                return null;
            }
        }
        public static void saveTireType(TireType tireType)
        {
            TireType testTireType = loadTireTypeById(tireType.TypeId);

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    if (testTireType == null)
                    {
                        command.CommandText =
                    @"INSERT INTO TireType (tireTypeId, type, description) VALUES (@tireTypeId, @type, @description)";
                    }
                    else
                    {
                        command.CommandText =
                    @"UPDATE TireType SET type = @type, description =  @description WHERE tireTypeId = @tireTypeId ";
                    }

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireSetId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = tireType.TypeId;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireType";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireType.Type.isNotNullOrEmpty())
                    {
                        parameter.Value = tireType.Type;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@description";
                    parameter.SqliteType = SqliteType.Text;
                    if (tireType.Type.isNotNullOrEmpty())
                    {
                        parameter.Value = tireType.Description;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
        public static List<TireType> loadTireTypeAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM TireType";

                List<TireType> tireTypeList = new List<TireType>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TireType tireType = Mapper.mapTireType(reader);
                        if (tireType != null)
                            tireTypeList.Add(tireType);
                    }
                }
                return tireTypeList;
            }
        }

        #endregion

        #region Tire
        public static TireType loadTireById(int id)
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Tires WHERE tireID = {id}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Mapper.mapTireType(reader);
                    }
                }
                return null;
            }
        }
        public static void saveTire(Tire tire)
        {
            TireType testTireType = loadTireById(tire.TireId);

            using (conn)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var command = conn.CreateCommand();
                    if (testTireType == null)
                    {
                        command.CommandText =
                    @"INSERT INTO Tires (tireId, tireSetId, tirePressure, tireTemp, tirePosition, tireStatus, raceId, tireType) VALUES (@tireId, @tireSetId, @tirePressure, @tireTemp, @tirePosition, @TireStatus, @RaceId, @TireType)";
                    }
                    else
                    {
                        command.CommandText =
                    @"UPDATE Tires SET tireSetId = @tireSetId, tirePressure = @tirePressure, tireTemp = @tireTemp, tirePosition = @tirePosition, tireStatus = @TireStatus, raceId = @RaceId, tireType = @TireType
                    WHERE tireId = @tireId";
                    }

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireId";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = tire.TireId;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireSetId";
                    parameter.SqliteType = SqliteType.Text;
                    if (tire.TireSetId != 0)
                    {
                        parameter.Value = tire.TireSetId;
                    }
                    else
                    {
                        parameter.Value = 0;
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@tirePressure";
                    parameter.SqliteType = SqliteType.Text;
                    if (tire.Pressure != 0.0)
                    {
                        parameter.Value = tire.Pressure;
                    }
                    else
                    {
                        parameter.Value = 0;
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@tireTemp";
                    parameter.SqliteType = SqliteType.Text;
                    if (tire.TireTemperature != 0.0)
                    {
                        parameter.Value = tire.TireTemperature;
                    }
                    else
                    {
                        parameter.Value = 0.0;
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@tirePosition";
                    parameter.SqliteType = SqliteType.Text;
                    if (tire.Position.isNotNullOrEmpty())
                    {
                        parameter.Value = tire.Position;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@TireStatus";
                    parameter.SqliteType = SqliteType.Text;
                    if (tire.Status.isNotNullOrEmpty())
                    {
                        parameter.Value = tire.Status;
                    }
                    else
                    {
                        parameter.Value = "";
                    }
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@RaceId";
                    parameter.SqliteType = SqliteType.Integer;

                    parameter.Value = tire.RaceId;
                    command.Parameters.Add(parameter);

                    parameter = command.CreateParameter();
                    parameter.ParameterName = "@TireType";
                    parameter.SqliteType = SqliteType.Integer;
                    parameter.Value = tire.Type.TypeId;

                    command.Parameters.Add(parameter);




                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }
        public static List<Tire> loadTireAll()
        {
            using (conn)
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @$"SELECT * FROM Tires";

                List<Tire> tireList = new List<Tire>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tire tire = Mapper.mapTire(reader);
                        if (tire != null)
                            tireList.Add(tire);
                    }
                }
                return tireList;
            }
        }

        #endregion

    }


}
