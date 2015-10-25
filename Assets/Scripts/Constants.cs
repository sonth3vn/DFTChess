public class Constants {
	public const string DOMAIN = "http://dft.vn:3000";
	public const string URLLOGIN = "http://127.0.0.1:3000/login";
	public const string URLREG = "http://127.0.0.1:3000/register";
//	public const string URLLOGIN = "http://dft.vn:3000/login";
//	public const string URLREG = "http://dft.vn:3000/register";
	public const string GATESERVER = "gate.handle.connect";
	public const string LOGINSERVER = "connector.ConnectorHandler.login";
	public const string JOINTABLE = "chinesechess.Handler.createorjoin";
	public const string STARTGAME = "chinesechess.Handler.begin";
	public const string SELECTCHESS = "chinesechess.Handler.selected";
	public const string MOVECHESS = "chinesechess.Handler.chess";
	public const string SHOWHIDDENCHESS = "chinesechess.Handler.showhiddenchess";
	public const string EXITTABLE = "chinesechess.Handler.exit";

	public const string ERROR = "Có lỗi xảy ra";
	public const string OK = "Đồng ý";
	public const string CANCEL = "Quay lại";
	public const string LOGIN_NOT_ENOUGH = "Thiếu thông tin đăng nhập";
};

public enum TableStatus{
	kStatusNone,
	kStatusWait,
	kStatusFull
};

public enum GameState{
	kStateLoading,
	kStateHome,
	kStateLobby,
	kStateJoinRoom,
	kStateJoinTable,
	kStateWatch,
	kStateWait,
	kStatePlay
};

public enum PlayerColor{
	kPlayerColorHost = -1,
	kPlayerColorGuest = 1
};

public enum ChessColor{
	RED = 1,
	BLUE = -1,
	REDHIDDEN = 11,
	BLUEHIDDEN = -11
}