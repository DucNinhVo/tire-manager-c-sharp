public class TireType {
	public int TypeId { get; set; }
	public string Description {get;set;}
	public string Type { get; set; }
	public string Variant { get; set; }

	public static int AmountOfTypes =6;

	public TireType(int id, string type, string desc)
	{
		this.TypeId = id;
		this.Type = type;
		this.Description = desc;
	}

	public TireType(int TireType) //Typen müssen noch definiert werden
	{
		if(TireType == 0)
		{
			Type = "Slick";
			Variant = "Cold";
			Description = "";
		}
		
		if(TireType == 1)
		{
			Type = "Slick";
			Variant = "Medium";
			Description = "";
		}
		
		if(TireType == 2)
		{
			Type = "Slick";
			Variant = "Hot";
			Description = "";
		}
		
		if(TireType == 3)
		{
			Type = "Inters";
			Variant = "Inters";
			Description = "";
		}
		
		if(TireType == 4)
		{
			Type = "Rain";
			Variant = "DryWet";
			Description = "";
		}
		
		if(TireType == 5)
		{
			Type = "Rain";
			Variant = "HeavyWet";
			Description = "";
		}
		
		//Reifentypen Schablonen falls mehr notwendig
		/*
		if(TireType == 5)
		{
			Type = "";
			Variant = "";
			Description = "";
		}
		*/
		
	}

	public static TireType getTypeByInput(string InputType,string InputVariant)
	{
		string Type = InputType;
		string Variant = InputVariant;

		for(int i = 0; i < AmountOfTypes;i++)
		{
			TireType Compare = new TireType(i);
			if(InputType == Compare.Type && InputVariant == Compare.Variant)
			{
				return Compare;
			}
		}

		return null;
	}

	public static int getIdByType(TireType Input)
	{
		for(int i = 0;i < 6;i++)
		{
			TireType Compare = new TireType(i);
			if(Compare.Variant == Input.Variant && Compare.Type == Input.Type)
			{
				return i;
			}

		}

		return -1;
	}
}
