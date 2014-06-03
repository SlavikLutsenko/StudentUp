﻿using System.Configuration;

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
				this.userId = (int)users["User_id"];
				this.accessLevel = (int)users["Access_level"];
				this.lecturerID = (int)users["Lecturer_id"];
				this.departmentID = (int) users["Department_id"];
				this.name = (string)users["Name"];
				this.surname = (string)users["Surname"];
				this.secondName = (string)users["Second_name"];
				this.position = (string)users["Position"];
				this.telephone = (string)users["Telephone"];

				/*
				 * Создаем сесию и куки
				 * */

				return true;
			}
			return false;
		}

		/// <summary>
		/// Считывает информацию об преподователе из БД
		/// </summary>
		/// <returns></returns>
		public override bool GetInformationAboutUserFromDB()
		{
			if (!this.IsExistsInDB()) return false;
			DB db = new DB();
			DB.ResponseTable users = null;
			if (this.email != string.Empty && this.passwodr != string.Empty)
				users = db.QueryToRespontTable(string.Format("select * from Lecturer inner join Users on Lecturer.Lecturer_id = Users.Lecturer_id where Email='{0}' and Password='{1}';", this.Email, this.Password));
			else
				if (this.userId != -1)
					users = db.QueryToRespontTable(string.Format("select * from Student inner join Users on Lecturer.Lecturer_id = Users.Lecturer_id where User_id = {0};", this.userId));
			if (users != null && users.CountRow == 1)
			{
				users.Read();
				this.userId = (int)users["User_id"];
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

		public static Lecturer AddLecturer(string name, string surname, string lastName, string email, string telephone, string department, string position, bool admin = false)
		{
			DB db = new DB();
			DB.ResponseTable table = db.QueryToRespontTable(string.Format("select Department_id from Department where name='{0}';", department));
			string userPassword = Validation.GeneratePassword(8);
			table.Read();
			int Department_id = (int)table["Department_id"];
			db.QueryToRespontTable(string.Format("insert into Lecturer(Department_id, Name, Surname, Second_name, Position, Telephone) value({0}, '{1}', '{2}', '{3}', '{4}', '{5}');", Department_id, name, surname, lastName, position, telephone));
			table = db.QueryToRespontTable("select LAST_INSERT_ID();");
			table.Read();
			int lecturerId = int.Parse(table[0].ToString());
			Lecturer lecturer = new Lecturer(Users.AddUserLecturer(email, userPassword, (admin ? 2 : 1), lecturerId));
			lecturer.GetInformationAboutUserFromDB();
			Mail.SendMail("smtp.gmail.com",
							ConfigurationManager.AppSettings.Get("AIDemail"),
							ConfigurationManager.AppSettings.Get("AIDpassword"),
							email,
							"Вы новый пользователь StudentUp",
							"Здравствуйте, " + email +
							"\n\nВы были зарегистрированы на сайте StudentUp\n\nДля доступа в систему Вам понадобится ввесли свой Email и пароль\n\nEmail: " + email + "\nПароль: " + userPassword + "\nСсылка на наш сайт:http://localhost:11292/");
			return lecturer;
		}
	}
}