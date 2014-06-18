using System;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий преподователя
	/// </summary>
	public class Lecturer : Users
	{
		/// <summary>
		/// Идентификатор преподователя
		/// </summary>
		int lecturerID = -1;

		/// <summary>
		/// Кафедра где работает преподователь
		/// </summary>
		int departmentID = -1;

		/// <summary>
		/// Имя преподователя
		/// </summary>
		string name = string.Empty;

		/// <summary>
		/// Фамилия преподователя
		/// </summary>
		string surname = string.Empty;

		/// <summary>
		/// Отчество преподователя
		/// </summary>
		string secondName = string.Empty;

		/// <summary>
		/// Должность преподователя
		/// </summary>
		string position = string.Empty;

		/// <summary>
		/// Телефон преподователя
		/// </summary>
		string telephone = string.Empty;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newLecturerID">Идентификатор преподователя</param>
		public Lecturer(int newLecturerID)
		{
			this.lecturerID = newLecturerID;
			this.userType = UserType.Lecturer;
		}

		/// <summary>
		/// Коструктор класса
		/// </summary>
		/// <param name="newUser">Пользователь являющийся преподователем</param>
		public Lecturer(Users newUser) : base(newUser) { }

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newEmail">Email преподователя</param>
		/// <param name="newPassword">Пароль преподователя</param>
		public Lecturer(string newEmail, string newPassword)
			: base(newEmail, newPassword)
		{
			this.userType = UserType.Lecturer;
		}

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newId">Идентификатор преподователя</param>
		/// <param name="newEmail">Email преподователя</param>
		/// <param name="newPassword">Пароль преподователя</param>
		public Lecturer(int newId, string newEmail, string newPassword) : base(newId, newEmail, newPassword, UserType.Lecturer) { }

		/// <summary>
		/// Возвращает идентификатор студента
		/// </summary>
		public new int ID { get { return this.lecturerID; } }

		/// <summary>
		/// Возвращает индетификатор пользователя
		/// </summary>
		public int UserID { get { return base.ID; } }

		/// <summary>
		/// Возвращает идентификатор кафедры где работает преподователь
		/// </summary>
		public int DepartmentID { get { return this.departmentID; } }

		/// <summary>
		/// Возвращает имя студента
		/// </summary>
		public string Name { get { return this.name; } }

		/// <summary>
		/// Возвращает фамилию студента
		/// </summary>
		public string Surname { get { return this.surname; } }

		/// <summary>
		/// Возвращает отчество студента
		/// </summary>
		public string SecondName { get { return this.secondName; } }

		/// <summary>
		/// Возвращает полное имя преподователя
		/// </summary>
		public string FullName { get { return string.Format("{0} {1} {2}", this.surname, this.name, this.secondName); } }

		/// <summary>
		/// Возвращает должность преподователя
		/// </summary>
		public string Position { get { return this.position; } }

		/// <summary>
		/// Возвращает телефон студента
		/// </summary>
		public string Telephone { get { return this.telephone; } }

		/// <summary>
		/// Производи вход пользоваетля в систему
		/// </summary>
		/// <returns>Зарегистрирован ли пользователь в системе</returns>
		public bool Login()
		{
			DB db = new DB();
			DB.ResponseTable users = db.QueryToRespontTable(string.Format("select * from Lecturer inner join Users on Lecturer.Lecturer_id = Users.Lecturer_id where Email='{0}' and Password='{1}';", this.Email, this.Password));
			if (users != null && users.CountRow == 1)
			{
				users.Read();
				this.userID = (int)users["User_id"];
				this.accessLevel = (int)users["Access_level"];
				this.lecturerID = (int)users["Lecturer_id"];
				this.departmentID = (int)users["Department_id"];
				this.name = (string)users["Name"];
				this.surname = (string)users["Surname"];
				this.secondName = (string)users["Second_name"];
				this.position = (string)users["Position"];
				this.telephone = (string)users["Telephone"];
				return true;
			}
			return false;
		}

		/// <summary>
		/// Считывает информацию об преподователе из БД
		/// </summary>
		/// <returns>Возвращает всю информацию о преподователе</returns>
		public override bool GetInformationAboutUserFromDB()
		{
			DB db = new DB();
			string query;
			if (this.email != string.Empty && this.passwodr != string.Empty)
				query = string.Format("select * from Lecturer inner join Users on Lecturer.Lecturer_id = Users.Lecturer_id where Users.Email='{0}' and Users.Password='{1}';", this.Email, this.Password);
			else
				if (this.userID != -1)
					query = string.Format("select * from Lecturer inner join Users on Lecturer.Lecturer_id = Users.Lecturer_id where Users.User_id = {0};", this.userID);
				else
					if (this.lecturerID != -1)
						query = string.Format("select * from Lecturer inner join Users on Lecturer.Lecturer_id = Users.Lecturer_id where Lecturer.Lecturer_id = {0};", this.lecturerID);
					else query = "";
			DB.ResponseTable users = db.QueryToRespontTable(query);
			if (users != null && users.CountRow == 1)
			{
				users.Read();
				this.userID = (int)users["User_id"];
				this.email = (string)users["Email"];
				this.passwodr = (string)users["Password"];
				this.accessLevel = (int)users["Access_level"];
				this.lecturerID = (int)users["Lecturer_id"];
				this.departmentID = (int)users["Department_id"];
				this.name = (string)users["Name"];
				this.surname = (string)users["Surname"];
				this.secondName = (string)users["Second_name"];
				this.telephone = (string)users["Telephone"];
				this.position = (string)users["Position"];
				this.userType = UserType.Lecturer;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Возвращает все группы которым преподает текущий преподователь
		/// </summary>
		/// <returns>Масив групп</returns>
		public Group[] GetMyGroups()
		{
			this.GetInformationAboutUserFromDB();
			Group[] result = null;
			DB db = new DB();
			Subject[] subjects = this.GetMySubjects();
			string subjectsID = "(";
			for (int i = 0, end = subjects.Length; i < end; i++)
				subjectsID += subjects[i].ID + (i != end - 1 ? "," : "");
			subjectsID += ")";
			DB.ResponseTable groupID = db.QueryToRespontTable(string.Format("select groups.Group_id from groups inner join student inner join StudentSubject on groups.Group_id = student.Group_id and student.Student_id = StudentSubject.Student_id and StudentSubject.Subject_id in {0} Group by groups.Group_id order by groups.Name;", subjectsID));
			if (groupID != null)
			{
				result = new Group[groupID.CountRow];
				for (int i = 0, end = result.Length; i < end && groupID.Read(); i++)
				{
					result[i] = new Group(Convert.ToInt32(groupID["Group_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			return result;
		}

		/// <summary>
		/// Возвращает всех студентов тех групп которым перподает текущий перподаватель
		/// </summary>
		/// <param name="groups">Масив идентификаторов группы</param>
		/// <returns>Масив студентов</returns>
		public Student[] GetMyStudents(Group[] groups)
		{
			this.GetInformationAboutUserFromDB();
			Student[] result = null;
			DB db = new DB();
			string groupssID = "(";
			for (int i = 0, end = groups.Length; i < end; i++)
				groupssID += groups[i].ID + (i != end - 1 ? "," : "");
			groupssID += ")";
			DB.ResponseTable studentID = db.QueryToRespontTable(string.Format("select student.Student_id from student inner join groups on student.Group_id = groups.Group_id and student.Group_id in {0} order by groups.Name;", groupssID));
			if (studentID != null)
			{
				result = new Student[studentID.CountRow];
				for (int i = 0, end = result.Length; i < end && studentID.Read(); i++)
				{
					result[i] = new Student(Convert.ToInt32(studentID["Student_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			return result;
		}

		/// <summary>
		/// Добовляет нового преподователя
		/// </summary>
		/// <param name="name">Имя преподователя</param>
		/// <param name="surname">Фамилия преподователя</param>
		/// <param name="secondName">Отчество преподователя</param>
		/// <param name="email">Email преподователя</param>
		/// <param name="position">Должность преподователя</param>
		/// <param name="telephone">Телефон преподователя</param>
		/// <param name="department">Кафедра преподователя, где он будет работать</param>
		/// <param name="admin">Будет ли преподователь администратором</param>
		/// <returns>Новый преподователь</returns>
		public static Lecturer AddLecturer(string name, string surname, string secondName, string email, string position, string telephone, int department, bool admin)
		{
			DB db = new DB();
			db.QueryToRespontTable(
				string.Format(
					"insert into Lecturer(Department_id, Name, Surname, Second_name, Position, Telephone) value({0}, '{1}', '{2}', '{3}', '{4}', '{5}');", department, name, surname, secondName, position, telephone));
			DB.ResponseTable userIdTable = db.QueryToRespontTable("select LAST_INSERT_ID() as id;");
			userIdTable.Read();
			Lecturer user = new Lecturer(Users.AddLecturerUsers(email, admin ? 2 : 1, Convert.ToInt32(userIdTable["id"])));
			user.GetInformationAboutUserFromDB();
			return user;
		}
	}
}