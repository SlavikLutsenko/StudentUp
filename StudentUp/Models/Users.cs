using System;
using System.Web;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий лубого пользователя
	/// </summary>
	public class Users: IPartInstitute
	{
		/// <summary>
		/// Перечисление описывающие тип пользователя
		/// </summary>
		public enum UserType
		{
			/// <summary>
			/// Пользователь есть студентом
			/// </summary>
			Student,
			/// <summary>
			/// Пользоваетель есть преподователем
			/// </summary>
			Lecturer
		}

		/// <summary>
		/// Идентификатор пользователя
		/// </summary>
		protected int userId = -1;

		/// <summary>
		/// Email пользователя
		/// </summary>
		protected string email = string.Empty;

		/// <summary>
		/// Пароль пользователя
		/// </summary>
		protected string passwodr = string.Empty;

		/// <summary>
		/// Уровень доступа пользователя в систему
		/// 0 - только чтени и изменение собственных данных
		/// 1 - чтение, изменение собственных данных, выставление оценок
		/// 2 - все можно
		/// </summary>
		protected int accessLevel = -1; //0, 1, 2

		/// <summary>
		/// Тип пользователя
		/// </summary>
		protected UserType userType;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newUserId">Идентификатор пользователя</param>
		public Users(int newUserId)
		{
			if(newUserId <= 0) throw new Exception("no id");
			this.userId = newUserId;
		}

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="newEmail">Email пользователя</param>
        public Users(string newEmail)
        {
	        if (!Validation.IsEmail(newEmail)) throw new ValidationDataException("no email");
            this.email = newEmail;
        }

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newUser">Новый пользователь</param>
		public Users(Users newUser)
		{
			this.userId = newUser.userId;
			this.email = newUser.email;
			this.passwodr = newUser.passwodr;
			this.accessLevel = newUser.accessLevel;
			this.userType = newUser.userType;
		}

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newEmail">Email пользователя</param>
		/// <param name="newPassword">Пароль пользователя</param>
		public Users(string newEmail, string newPassword)
		{
			Messages messages = new Messages();
			if (!Validation.IsEmail(newEmail))
				messages.Add(Messages.Message.TypeMessage.error, "no email");
			if (!Validation.IsPassword(newPassword))
				messages.Add(Messages.Message.TypeMessage.error, "no password");
			if (messages.Count != 0) throw new ValidationDataException(messages);
			this.email = newEmail;
			this.passwodr = newPassword;
		}

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newUserId">Идентификатор пользователя</param>
		/// <param name="newEmail">Email пользователя</param>
		/// <param name="newPassword">Пароль пользователя</param>

		/// <param name="newUserType">Тип пользователя</param>
		public Users(int newUserId, string newEmail, string newPassword, Users.UserType newUserType)
		{
			Messages messages = new Messages();
			if (newUserId <= 0)
				messages.Add(Messages.Message.TypeMessage.error, "no id");
			if (!Validation.IsEmail(newEmail))
				messages.Add(Messages.Message.TypeMessage.error, "no email");
			if (!Validation.IsPassword(newPassword))
				messages.Add(Messages.Message.TypeMessage.error, "no password");
			if (messages.Count != 0) throw new ValidationDataException(messages);
			this.userId = newUserId;
			this.email = newEmail;
			this.passwodr = newPassword;
			this.userType = newUserType;
		}

		/// <summary>
		/// Возвращает идентификатор пользователя
		/// </summary>
		public int ID { get { return this.userId; } }

		/// <summary>
		/// Возвращает email пользователя
		/// </summary>
		public string Email { get { return this.email; } }

		/// <summary>
		/// Возвращает пароль пользователя в зашифрованном виде
		/// </summary>
		public string Password { get { return this.passwodr; } }

		/// <summary>
		/// Возвращает уровень доступа пользователя в систему
		/// </summary>
		public int AccessLevel { get { return this.accessLevel; } }

		/// <summary>
		/// Возвращает тип пользователя
		/// </summary>
		public UserType Type { get { return this.userType; } }

		/// <summary>
		/// Проверяет заригестрирован ли пользователь в системе
		/// Проверка происходит в трех случаях:
		/// 1. Если задан идентификатор пользователя
		/// 2. Если задан email пользователя
		/// 3. Если задан и email и пароль пользователя
		/// </summary>
		/// <returns>
		/// true - Зарегистрирован
		/// flase - Не зарегистрирован
		/// </returns>
		public bool IsExistsInDB()
		{
			DB db = new DB();
			DB.ResponseTable users = null;
			if (this.email != string.Empty && this.passwodr != string.Empty)
				users = db.QueryToRespontTable(string.Format("select * from Users where Email='{0}' and Password='{1}';", this.Email, this.Password));
			else
				if (this.userId != -1)
					users = db.QueryToRespontTable(string.Format("select * from Users where User_id='{0}';", this.userId));
				else
					if (this.email != string.Empty)
						users = db.QueryToRespontTable(string.Format("select * from Users where Email='{0}';", this.email));
			return users != null && users.CountRow == 1;
		}

		/// <summary>
		/// Получает оновную информацию про пользователя из БД
		/// </summary>
		/// <returns>Если пользователь не зарегистрирован возращает false, иначе true</returns>
		public virtual bool GetInformationAboutUserFromDB()
		{
			if (!this.IsExistsInDB()) return false;
			DB db = new DB();
			DB.ResponseTable users = null;
			if (this.email != string.Empty && this.passwodr != string.Empty)
				users = db.QueryToRespontTable(string.Format("select * from Users where Email='{0}' and Password='{1}';", this.Email, this.Password));
			else
				if (this.userId != -1)
					users = db.QueryToRespontTable(string.Format("select * from Users where User_id='{0}';", this.userId));
				else
					if(this.email != "")
						users = db.QueryToRespontTable(string.Format("select * from Users where Email='{0}';", this.Email));
			if (users == null || users.CountRow <= 0) return false;
			users.Read();
			this.userId = (int)users["User_id"];
			this.email = (string)users["Email"];
			this.passwodr = (string)users["Password"];
			this.accessLevel = (int)users["Access_level"];
			this.userType = users["Student_id"] == null ? UserType.Lecturer : UserType.Student;
			return true;
		}

		/// <summary>
		/// Создает сессию для текущего пользователя
		/// </summary>
		/// <param name="Response">Необходим для вставки в coocke-файл пользователя идентификатор сессии</param>
		public void CreateSession(HttpResponseBase Response)
		{
			if (this.IsExistsInDB())
				Response.SetCookie(new HttpCookie("userID", this.userId.ToString()));
		}

		/// <summary>
		/// ПРоверяет созданна ли сессия пользователя
		/// </summary>
		/// <param name="Request">Необходим для получени coocke-файл пользователя идентификатор сессии</param>
		/// <returns>true - созданна, false - не созданна</returns>
		public static bool IsCreatedSession(HttpRequestBase Request)
		{
			return Request.Cookies["userID"] != null && Request.Cookies["userID"].Value != "";
		}

		/// <summary>
		/// Получает идентификатор пользователи из сессии и заполняет информацию о нем
		/// </summary>
		/// <param name="Request">Необходим для получени coocke-файл пользователя идентификатор сессии</param>
		/// <returns>Пользователь - если сессия была созданна и null - ессли нет</returns>
		public static Users GetSession(HttpRequestBase Request)
		{
			HttpCookie userCookieSession = Request.Cookies["userID"];
			if (userCookieSession != null && userCookieSession.Value != "")
			{
				Users user = new Users(int.Parse(userCookieSession.Value));
				user.GetInformationAboutUserFromDB();
				return user;
			}
			return null;
		}

		/// <summary>
		/// Удаляет сессию пользователя
		/// </summary>
		/// <param name="Response">Результат запроса</param>
		/// <param name="Request">Сам запрос</param>
		public static void DeleteSession(HttpResponseBase Response, HttpRequestBase Request)
		{
			if (IsCreatedSession(Request))
			{
				HttpCookie myCookie = new HttpCookie("userID") {Expires = DateTime.Now.AddDays(-1d)};
				Response.Cookies.Add(myCookie);
			}
		}

		static Users AddUser(string email, string password, int accessLevel, int idStudent, int idLecturer)
		{
			DB db = new DB();
			string query;
			if (idStudent == 0 && idLecturer != 0)
				query = string.Format("insert into Users(Lecturer_id, Email, Password, Access_level) value({0}, '{1}', '{2}', '{3}');", idLecturer, email, password, accessLevel);
			else
				if (idStudent != 0 && idLecturer == 0)
					query = string.Format("insert into Users(Student_id, Email, Password, Access_level) value({0}, '{1}', '{2}', '{3}');", idStudent, email, password, accessLevel);
				else
					throw new Exception("Не переданна вся информация");
			db.QueryToRespontTable(query);
			Users user = new Users(email, password);
			user.GetInformationAboutUserFromDB();
			return user;
		}

		static public Users AddUserStudent(string email, string password, int accessLevel, int idStudent)
		{
			return Users.AddUser(email, password, accessLevel, idStudent, 0);
		}

		static public Users AddUserLecturer(string email, string password, int accessLevel, int idLecturer)
		{
			return Users.AddUser(email, password, accessLevel, 0, idLecturer);
		}
	}
}
