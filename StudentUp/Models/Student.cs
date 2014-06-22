using System;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий студента
	/// </summary>
	public class Student : Users
	{
		/// <summary>
		/// Тип обучения в университете
		/// </summary>
		public enum TypeOfEducation
		{
			/// <summary>
			/// Дневная форма обучения - бюджет
			/// </summary>
			dayBudget,
			/// <summary>
			/// Дневная форма обучения - контракт
			/// </summary>
			dayContract,
			/// <summary>
			/// Заочная форма обучения
			/// </summary>
			extramural
		}

		/// <summary>
		/// Идентификатор студента
		/// </summary>
		int studentID = -1;

		/// <summary>
		/// Идентификатор группы студента
		/// </summary>
		int groupID = -1;

		/// <summary>
		/// Имя студента
		/// </summary>
		string name = string.Empty;

		/// <summary>
		/// Фамилия студента
		/// </summary>
		string surname = string.Empty;

		/// <summary>
		/// Отчество студента
		/// </summary>
		string secondName = string.Empty;

		/// <summary>
		/// Текущий семетр на котором учится студента
		/// </summary>
		int currentSemester = -1;

		/// <summary>
		/// Адресс студента
		/// </summary>
		string address = string.Empty;

		/// <summary>
		/// Телефон студента
		/// </summary>
		string telephone = string.Empty;

		/// <summary>
		/// Зачетная книга студента
		/// </summary>
		string recordBook = string.Empty;

		/// <summary>
		/// Тип обучения студента
		/// </summary>
		TypeOfEducation typeOfEducetion;

		/// <summary>
		/// Контакты родителей студента
		/// </summary>
		string contactsParents = string.Empty;

		/// <summary>
		/// Род деятельности на кафедре
		/// </summary>
		string employmentInTheDepartment = string.Empty;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newStudentID">Идентификатор студента</param>
		public Student(int newStudentID)
		{
			this.studentID = newStudentID;
			this.userType = UserType.Student;
		}

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newUser">Пользователь являющийся студентом</param>
		public Student(Users newUser)
			: base(newUser)
		{
			this.userType = UserType.Student;
		}

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newEmail">Email студента</param>
		/// <param name="newPassword">Пароль студента</param>
		public Student(string newEmail, string newPassword)
			: base(newEmail, newPassword)
		{
			this.userType = UserType.Student;
		}

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newId">Идентификатор студента</param>
		/// <param name="newEmail">Email студента</param>
		/// <param name="newPassword">Пароль студента</param>
		public Student(int newId, string newEmail, string newPassword) : base(newId, newEmail, newPassword, UserType.Student) { }

		/// <summary>
		/// Возвращает идентификатор студента
		/// </summary>
		public new int ID { get { return this.studentID; } }

		/// <summary>
		/// Возвращает идентификатор пользователя
		/// </summary>
		public int UserID { get { return base.ID; } }

		/// <summary>
		/// Возвращает идентификатор группы студента
		/// </summary>
		public int GroupID { get { return this.groupID; } }

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
		/// Возвращает полное имя студента
		/// </summary>
		public string FullName { get { return string.Format("{0} {1} {2}", this.surname, this.name, this.secondName); } }

		/// <summary>
		/// Возвращает фамилию и инициалы студента
		/// </summary>
		public string ShortName{get { return string.Format("{0} {1}.{2}.", this.surname, this.name[0], this.secondName[0]); }}

		/// <summary>
		/// Возвращает семестр на котором учится студент
		/// </summary>
		public int CurrentSemester { get { return this.currentSemester; } }

		/// <summary>
		/// Возвращает адресс проживания студента
		/// </summary>
		public string Address { get { return this.address; } }

		/// <summary>
		/// Возвращает телефон студента
		/// </summary>
		public string Telephone { get { return this.telephone; } }

		/// <summary>
		/// Возвращает номер зачетной книжки студента
		/// </summary>
		public string RecordBook { get { return this.recordBook; } }

		/// <summary>
		/// Возвращает тип обучения студента
		/// </summary>
		public TypeOfEducation TypeEducetion { get { return this.typeOfEducetion; } }

		/// <summary>
		/// Возвращает контакты родителей студента
		/// </summary>
		public string ContactsParents { get { return this.contactsParents; } }

		/// <summary>
		/// Возвращает род деятельности студента на кафедре
		/// </summary>
		public string EmploymentInTheDepartment { get { return this.employmentInTheDepartment; } }

		/// <summary>
		/// Производит вход пользователя в систему
		/// </summary>
		/// <returns>true - залогинен, false - не залогинен</returns>
		public bool Login()
		{
			DB db = new DB();
			DB.ResponseTable users = db.QueryToRespontTable(string.Format("select * from Student inner join Users on Student.Student_id = Users.Student_id where Email='{0}' and Password='{1}';", this.Email, this.Password));
			if (users != null && users.CountRow == 1)
			{
				users.Read();
				this.userID = Convert.ToInt32(users["User_id"]);
				this.accessLevel = Convert.ToInt32(users["Access_level"]);
				this.studentID = Convert.ToInt32(users["Student_id"]);
				this.groupID = Convert.ToInt32(users["Group_id"]);
				this.name = (string)users["Name"];
				this.surname = (string)users["Surname"];
				this.secondName = (string)users["Second_name"];
				this.currentSemester = Convert.ToInt32(users["Semester"]);
				this.address = (string)users["Address"];
				this.telephone = (string)users["Telephone"];
				this.recordBook = (string)users["Record_book"];
				this.typeOfEducetion = Student.ConverStringToEnum((string)users["Type_of_education"]);
				this.contactsParents = (string)users["Contacts_parents"];
				this.employmentInTheDepartment = (string)users["Employment_in_the_department"];
				return true;
			}
			return false;
		}

		/// <summary>
		/// Считывает информацию об студенте из БД
		/// </summary>
		/// <returns></returns>
		public override bool GetInformationAboutUserFromDB()
		{
			DB db = new DB();
			string query;
			if (this.email != string.Empty && this.passwodr != string.Empty)
				query = string.Format("select * from Student inner join Users on Student.Student_id = Users.Student_id where Users.Email='{0}' and Users.Password='{1}';", this.Email, this.Password);
			else
				if (this.userID != -1)
					query = string.Format("select * from Student inner join Users on Student.Student_id = Users.Student_id where Users.User_id = {0};", this.userID);
				else 
					if (this.studentID != -1)
						query = string.Format("select * from Student inner join Users on Student.Student_id = Users.Student_id where Student.Student_id = {0};", this.studentID);
					else query = "";
			DB.ResponseTable users = db.QueryToRespontTable(query);
			if (users != null && users.CountRow == 1)
			{
				users.Read();
				this.userID = Convert.ToInt32(users["User_id"]);
				this.email = (string)users["Email"];
				this.passwodr = (string)users["Password"];
				this.accessLevel = Convert.ToInt32(users["Access_level"]);
				this.studentID = Convert.ToInt32(users["Student_id"]);
				this.groupID = Convert.ToInt32(users["Group_id"]);
				this.name = (string)users["Name"];
				this.surname = (string)users["Surname"];
				this.secondName = (string)users["Second_name"];
				this.currentSemester = Convert.ToInt32(users["Semester"]);
				this.address = (string)users["Address"];
				this.telephone = (string)users["Telephone"];
				this.recordBook = (string)users["Record_book"];
				this.typeOfEducetion = Student.ConverStringToEnum((string)users["Type_of_education"]);
				this.contactsParents = (string)users["Сontacts_parents"];
				this.employmentInTheDepartment = (string)users["Employment_in_the_department"];
				return true;
			}
			return false;
		}

		/// <summary>
		/// Добавление студента
		/// </summary>
		/// <param name="name">Имя студента</param>
		/// <param name="surname">Фамилия студента</param>
		/// <param name="secondName">Отчество студента</param>
		/// <param name="email">Email студента</param>
		/// <param name="telephone">Телефон студента</param>
		/// <param name="group">Группа студента</param>
		/// <param name="currentSemestr">Семестр на котором учится студент</param>
		/// <param name="address">Адрес студента</param>
		/// <param name="recordBook">Код зачетной книги студента</param>
		/// <param name="typeOfEducation">Тип обралования студента ('дена','заочна')</param>
		/// <param name="contactsParents">Контакты родителей студента</param>
		/// <param name="employmentInTheDepartment">Чем занимается студент на кафедре</param>
		/// <param name="admin">Является ли студент администраторм системы</param>
		/// <returns>Новый студент</returns>
		public static Student AddStudent(string surname, string name, string secondName, string email, string telephone,
			int group, int currentSemestr, string address, string recordBook, Student.TypeOfEducation typeOfEducation, string contactsParents,
			string employmentInTheDepartment, bool admin)
		{
			DB db = new DB();
			db.QueryToRespontTable(string.Format("insert into Student(Group_id, Surname, Name, Second_name, Semester, Address, Telephone, Record_book, Type_of_education, Сontacts_parents, Employment_in_the_department) values ({0}, '{1}', '{2}', '{3}', {4}, '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');", group, surname, name, secondName, currentSemestr, address, telephone, recordBook, Student.GetEnumDescription(typeOfEducation), contactsParents, employmentInTheDepartment));
			DB.ResponseTable userIdTable = db.QueryToRespontTable("select LAST_INSERT_ID() as id;");
			userIdTable.Read();
			Student user = new Student(Users.AddStudentUsers(email, admin ? 2 : 0, Convert.ToInt32(userIdTable["id"])));
			user.GetInformationAboutUserFromDB();
			return user;
		}

		/// <summary>
		/// Преобразовывает элемент перчисления в строку
		/// </summary>
		/// <param name="value">Элемент перечисления</param>
		/// <returns>Строковая интерпритация элемнта перечисления</returns>
		public static string GetEnumDescription(Student.TypeOfEducation value)
		{
			switch (value)
			{
				case TypeOfEducation.dayBudget:
					return "денна - бюджет";
				case TypeOfEducation.dayContract:
					return "денна - контракт";
				case TypeOfEducation.extramural:
					return "заочна";
			}
			return null;
		}

		/// <summary>
		/// Преобразовывает строку в элемент перчисления
		/// </summary>
		/// <param name="value">Строковая интерпритация элемнта перечисления</param>
		/// <returns>Элемент перечисления</returns>
		public static Student.TypeOfEducation ConverStringToEnum(string value)
		{
			switch (value)
			{
				case "денна - бюджет":
					return TypeOfEducation.dayBudget;
				case "денна - контракт":
					return TypeOfEducation.dayContract;
				case "заочна":
					return TypeOfEducation.extramural;
			}
			return Student.TypeOfEducation.dayBudget;
		}

		/// <summary>
		/// Возвращает все атестации студента по заданным предметам
		/// </summary>
		/// <param name="numberAttestation">Номер атестации</param>
		/// <param name="subjectsID">Масив идентификаторов предметов</param>
		/// <returns>Масив атестаций предметов</returns>
		public Examination[] GetAttestation(int numberAttestation, int[] subjectsID)
		{
			Examination[] result = null;
			string subjects = "(";
			for(int i = 0; i < subjectsID.Length; i++)
				subjects += subjectsID[i] + (i != subjectsID.Length - 1 ? "," : "");
			subjects += ")";
			DB.ResponseTable attestationID = (new DB()).QueryToRespontTable(string.Format("select examination.Examination_id from users inner join student inner join studentsubject inner join examination on users.Student_id = student.Student_id and student.Student_id = studentsubject.Student_id and studentsubject.StudentSubject_id = examination.StudentSubject_id and studentsubject.Subject_id in {0} and users.User_id = {1} and examination.Exam_type = '{2}' order by studentsubject.Subject_id;", subjects, this.userID, "атестація" + numberAttestation));
			if (attestationID != null)
			{
				result = new Examination[attestationID.CountRow];
				for (int i = 0, end = result.Length; i < end && attestationID.Read(); i++)
				{
					result[i] = new Examination(Convert.ToInt32(attestationID["Examination_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			return result;
		}
	}
}