using System;
using System.Configuration;
using System.Web;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий лубого пользователя
	/// </summary>
	public class Users : IPartInstitute
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
		protected int userID = -1;

		/// <summary>
		/// Индетификатор студента
		/// </summary>
		int studentID = -1;

		/// <summary>
		/// Индетификатор преподователя
		/// </summary>
		int lecturerID = -1;

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
		/// Конструктор предназначенн для создания классов потомков без участия этого класса
		/// </summary>
		protected Users() { }

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newUserId">Идентификатор пользователя</param>
		public Users(int newUserId)
		{
			if (newUserId <= 0) throw new Exception("no id");
			this.userID = newUserId;
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
			this.userID = newUser.userID;
			this.studentID = newUser.studentID;
			this.lecturerID = newUser.lecturerID;
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
			this.userID = newUserId;
			this.email = newEmail;
			this.passwodr = newPassword;
			this.userType = newUserType;
		}

		/// <summary>
		/// Возвращает идентификатор пользователя
		/// </summary>
		public int ID { get { return this.userID; } }

		/// <summary>
		/// Возвращает идентификатор студента
		/// </summary>
		public int StudentID { get { return this.studentID; } }

		/// <summary>
		/// Возвращает идентификатор преподователя
		/// </summary>
		public int LecturerID { get { return this.lecturerID; } }

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
				if (this.userID != -1)
					users = db.QueryToRespontTable(string.Format("select * from Users where User_id='{0}';", this.userID));
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
				if (this.userID != -1)
					users = db.QueryToRespontTable(string.Format("select * from Users where User_id='{0}';", this.userID));
				else
					if (this.email != "")
						users = db.QueryToRespontTable(string.Format("select * from Users where Email='{0}';", this.Email));
			if (users == null || users.CountRow <= 0) return false;
			users.Read();
			this.userID = (int)users["User_id"];
			this.studentID = users["Student_id"] == null ? -1 : Convert.ToInt32(users["Student_id"]);
			this.lecturerID = users["Lecturer_id"] == null ? -1 : Convert.ToInt32(users["Lecturer_id"]);
			this.email = (string)users["Email"];
			this.passwodr = (string)users["Password"];
			this.accessLevel = (int)users["Access_level"];
			this.userType = users["Student_id"] == null ? UserType.Lecturer : UserType.Student;
			return true;
		}

		/// <summary>
		/// Возвращает предметы пользователя
		/// </summary>
		/// <returns>Масив предметов</returns>
		public Subject[] GetMySubjects()
		{
			Subject[] result = null;
			DB db = new DB();
			string query;
			if (this.userType == UserType.Student)
				query = string.Format("select studentsubject.Subject_id from studentsubject inner join student inner join users on studentsubject.Student_id = student.Student_id and student.Student_id = users.Student_id and users.User_id = {0};", this.userID);
			else
				query = string.Format("select subject.Subject_id from subject inner join lecturer inner join users on subject.Lecturer_id = lecturer.Lecturer_id and lecturer.Lecturer_id = users.Lecturer_id and users.User_id = {0};", this.userID);
			DB.ResponseTable subjectsID = db.QueryToRespontTable(query);
			if (subjectsID != null)
			{
				result = new Subject[subjectsID.CountRow];
				for (int i = 0, end = result.Length; i < end && subjectsID.Read(); i++)
				{
					result[i] = new Subject(Convert.ToInt32(subjectsID["Subject_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			return result;
		}

		/// <summary>
		/// Возвращает масив оценок пользователя
		/// </summary>
		/// <returns>Масив оценок</returns>
		public Marks[] GetMyMarks()
		{
			Marks[] result = null;
			DB db = new DB();
			string query;
			if (this.userType == UserType.Student)
				query = string.Format("select marks.Mark_id from marks inner join studentsubject inner join student inner join users on marks.StudentSubject_id = studentsubject.StudentSubject_id and studentsubject.Student_id = student.Student_id and student.Student_id = users.Student_id and users.User_id = {0} order by marks.Date;", this.userID);
			else
				query = string.Format("select marks.Mark_id from marks inner join studentsubject inner join subject inner join lecturer inner join users on marks.StudentSubject_id = studentsubject.StudentSubject_id and studentsubject.Subject_id = subject.Subject_id and subject.Lecturer_id = lecturer.Lecturer_id and lecturer.Lecturer_id = users.Lecturer_id and users.User_id = {0} order by marks.Date;", this.userID);
			DB.ResponseTable markID = db.QueryToRespontTable(query);
			if(markID != null && markID.CountRow > 0)
			{
				result = new Marks[markID.CountRow];
				for (int i = 0, end = result.Length; i < end && markID.Read(); i++)
				{
					result[i] = new Marks(Convert.ToInt32(markID["Mark_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			return result;
		}

		/// <summary>
		/// Создает сессию для текущего пользователя
		/// </summary>
		/// <param name="Response">Необходим для вставки в coocke-файл пользователя идентификатор сессии</param>
		public void CreateSession(HttpResponseBase Response)
		{
			if (this.IsExistsInDB()) Response.SetCookie(new HttpCookie("userID", this.userID.ToString()));
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
				Users user = new Users(Convert.ToInt32(userCookieSession.Value));
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
				HttpCookie myCookie = new HttpCookie("userID") { Expires = DateTime.Now.AddDays(-1d) };
				Response.Cookies.Add(myCookie);
			}
		}

		/// <summary>
		/// Добавляет нового пользователя
		/// </summary>
		/// <param name="email">Email пользователя</param>
		/// <param name="accessLevel">Уровень доступа пользователя</param>
		/// <param name="idStudent">Идентификатор студента, если это не студент необходимо передавать число меньшее 0</param>
		/// <param name="idLecturer">Идентификатор преподователя, если это не преподователь необходимо передавать число меньшее 0</param>
		/// <returns>Новый пользователь</returns>
		private static Users AddUsers(string email, int accessLevel, int idStudent, int idLecturer)
		{
			if (!Validation.IsEmail(email)) throw new ValidationDataException("no email");
			DB db = new DB();
			string password = Validation.GeneratePassword(8);
			if (idLecturer <= 0)
				db.QueryToRespontTable(string.Format("insert into Users(Student_id, Email, Password, Access_level) values ({0}, '{1}', '{2}', {3});", idStudent, email, password, accessLevel));
			else
				if (idStudent <= 0)
					db.QueryToRespontTable(string.Format("insert into Users(Lecturer_id, Email, Password, Access_level) values ({0}, '{1}', '{2}', {3});", idLecturer, email, password, accessLevel));
				else throw new Exception("error in data add user");
			Users user = new Users(email, password);
			user.GetInformationAboutUserFromDB();
			Mail.SendMail("smtp.gmail.com",
							ConfigurationManager.AppSettings.Get("AIDemail"),
							ConfigurationManager.AppSettings.Get("AIDpassword"),
							email,
							"Вы наш новый пользователь",
							"Здравствуйте, " + email +
							"\n\nВы теперь зарегистрированны в StudentUp\n\n" +
							"Вы можете зайти в системы использу я следущие:\n\n" +
							"Свой email: " + email + "\n" +
							"Пароль: " + password + "\n" +
							"Перейдя по ссылке: http://127.0.0.1/");
			return user;
		}

		/// <summary>
		/// Добавление студента
		/// </summary>
		/// <param name="email">Email студента</param>
		/// <param name="accessLevel">Уровень доступа студента</param>
		/// <param name="idStudent">Идентификатор студента</param>
		/// <returns></returns>
		public static Users AddStudentUsers(string email, int accessLevel, int idStudent)
		{
			return Users.AddUsers(email, accessLevel, idStudent, -1);
		}

		/// <summary>
		/// Добавление преподователя
		/// </summary>
		/// <param name="email">Email преподователя</param>
		/// <param name="accessLevel">Уровень доступа преподователя</param>
		/// <param name="idLecturer">Идентификатор преподователя</param>
		/// <returns></returns>
		public static Users AddLecturerUsers(string email, int accessLevel, int idLecturer)
		{
			return Users.AddUsers(email, accessLevel, -1, idLecturer);
		}
	}
}
