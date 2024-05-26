using BlazorApp.DB;

public class User {
	public string UserName { get; set; }
	public int UserID {get;}
	public int Priviledges {get;} //Manager = 2, Ing = 1 User = 0

	public Session UserSession{get;set;}
	
	public string Password { get; set; } 

	public static User CurrentUser;
	
	//Funktionen
	public User(string UserName,int UserID,int Priviledges,string Password)
	{
		this.UserName = UserName;
		this.UserID = UserID;
		this.Priviledges = Priviledges;
		this.Password = Password;
		UserSession = new Session();
	}	

	public bool checkPriviledges(int AccessLevel)
	{
		if(Priviledges == AccessLevel)
		{
			return true;
		}
		else
		{
			return false;
		}	
	}
}
