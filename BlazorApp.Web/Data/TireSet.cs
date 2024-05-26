public class TireSet
{
    public int tireSetId { get; set; }
    public int tireTypeId { get; set; }
    public int tireVariant { get; set; }
    public string orderDate { get; set; }
    public string orderTime { get; set; }
    public string pickUpTime { get; set; }
    public double startingTemp { get; set; }
    public double heatingTemp { get; set; }//90 oder 40
    public int heatingDuration { get; set; } = 90;//90 minuten
    public string heatingStartTime { get; set; }
    public string heatingEndTime { get; set; }
    public string specNote { get; set; }  
    public bool isUsed { get; set; }
    public bool isOnCar { get; set; }
    public bool isTireSetDone{ get; set; }
    public TireSet(int id, int tireTypeId, int tireVariant, string orderTime, string pickUpTime, string orderDate, string note)
    {
        this.tireSetId = id;
        this.tireTypeId = tireTypeId;
        this.tireVariant = tireVariant;
        this.orderDate = orderTime;
        this.orderTime = orderTime;
        this.pickUpTime = pickUpTime;
        this.orderDate = orderDate;
        this.specNote = note;
    }

    public TireSet(int id, double startingTemp, double heatingTemp, int heatingDuration, string startTime, string endTime)
    {
        this.tireSetId= id;
        this.startingTemp = startingTemp;
        this.heatingTemp = heatingTemp;
        this.heatingDuration = heatingDuration;   
        this.heatingStartTime = startTime; 
        this.heatingEndTime = endTime;
    }

    public TireSet(int id, int tireTypeId, int tireVariant, string orderDate, string orderTime, string pickupTime, double startingTemp, double heatingTemp, int heatingDuration, string startTime, string endTime, string note)
    {
        this.tireSetId = id;
        this.tireTypeId= tireTypeId;
        this.tireVariant= tireVariant;
        this.orderDate= orderDate;
        this.orderTime= orderTime;
        this.pickUpTime= pickupTime;
        this.startingTemp = startingTemp;
        this.heatingTemp = heatingTemp;
        this.heatingDuration = heatingDuration;
        this.heatingStartTime = startTime;
        this.heatingEndTime = endTime;
        this.specNote= note;

    }

    //True if tire is fitted 
    public bool tireSetOnCar(TireSet t)
    {
        t.isUsed = false;
        return t.isOnCar = true;
    }

    //true if tire is used
     public bool tireUsed(TireSet t)
    {
        return t.isUsed = true;
    }

    public bool tireSetState(TireSet t)
    {
        if(t.isOnCar == true)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    //use if tire is done and not changed cuz of weather conditions
    public bool tireSetDone(TireSet t)
    {
        return t.isTireSetDone = true;
    }

}
