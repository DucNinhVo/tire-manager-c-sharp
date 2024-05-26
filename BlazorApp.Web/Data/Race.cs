using BlazorApp.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorApp.Data
{
    public class Race
    {
        public int ID{get;set;}
        public string Name{get;set;}
        public DateTime Datum{get;set;}

        public int SlickCold{get;set;}
        public string PreFix_SlickCold{get;set;}
        public int SlickMedium{get;set;}
        public string PreFix_SlickMedium{get;set;}
        public int SlickHot{get;set;}
        public string PreFix_SlickHot{get;set;}
        public int Inters{get;set;}
        public string PreFix_Inters{get;set;}
        public int RainDryWet{get;set;}
        public string PreFix_RainDryWet{get;set;}
        public int RainHeavyWet{get;set;}
        public string PreFix_RainHeavyWet{get;set;}

        public static List<Race> ListOfRaces = new List<Race>();

        public static Race CurrentRace { get; set; }
           

        public static void addRace(Race Insert)
        {
            ListOfRaces.Add(Insert);
        }

        public static void refreshCurrentRace()
        {
            Race.setRace(DataBase.loadRacesAll().OrderByDescending(i => i.ID).First().Name);
        }

        public static void setRace(string RaceSelectValue)
        {
            foreach (Race race in ListOfRaces)
            {
                if (race.Name == RaceSelectValue)
                {
                    Race.CurrentRace = race;
                }
            }
        }
        public static void setRace(int  id)
        {
            CurrentRace = DataBase.loadRaceById(id);
        }
        public static void setRace(Race  race)
        {
            CurrentRace = race;
        }

        public Race(string Name,
                    string PreFix_SlickCold,
                    int SlickCold,
                    string PreFix_SlickMedium,
                    int SlickMedium,
                    string PreFix_SlickHot,
                    int SlickHot,
                    string PreFix_Inters,
                    int Inters,
                    string PreFix_RainDryWet,
                    int RainDryWet,
                    string PreFix_RainHeavyWet,
                    int RainHeavyWet
                    )
                    {
                        this.Name = Name;
                        this.Datum = DateTime.Now;
                        this.PreFix_SlickCold = PreFix_SlickCold;
                        this.SlickCold = SlickCold;
                        this.PreFix_SlickMedium = PreFix_SlickMedium;
                        this.SlickMedium = SlickMedium;
                        this.PreFix_SlickHot = PreFix_SlickHot;
                        this.SlickHot = SlickHot;
                        this.PreFix_Inters = PreFix_Inters;
                        this.Inters = Inters;
                        this.PreFix_RainDryWet = PreFix_RainDryWet;
                        this.RainDryWet = RainDryWet;
                        this.PreFix_RainHeavyWet = PreFix_RainHeavyWet;
                        this.RainHeavyWet = RainHeavyWet;

                        bool InUse = false;

                        while(true)
                        {
                            if(ID > 255)
                            {
                                break;
                            }

                            foreach(Race race in ListOfRaces)
                            {
                                if(this.ID == race.ID)
                                {
                                    InUse = true;
                                }
                            }
                            if(InUse == false)
                            {
                                break;
                            }
                            else
                            {
                                this.ID++;    
                            }
                        }
                        ListOfRaces.Add(this);
                        DB.DataBase.saveRace(this);
                    }

        public Race(string Name)
        {
            this.Name = Name;
            bool InUse = false;
            ID = 1;

            while(true)
            {
                if(ID > 255)
                {
                    break;
                }

                foreach(Race race in ListOfRaces)
                {
                    if(this.ID == race.ID)
                    {
                        InUse = true;
                    }
                }
                if(InUse == false)
                {
                    break;
                }
                else
                {
                    this.ID++;    
                }
            }

            ListOfRaces.Add(this);
        }
    }
}
