﻿using System;

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
			/// Дневная форма обучения
			/// </summary>
			дена,
			/// <summary>
			/// Заочная форма обучения
			/// </summary>
			заочна
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
		/// <returns></returns>
		public bool Login()
		{
			DB db = new DB();
			DB.ResponseTable users = db.QueryToRespontTable(string.Format("select * from Student inner join Users on Student.Student_id = Users.Student_id where Email='{0}' and Password='{1}';", this.Email, this.Password));
			if (users != null && users.CountRow == 1)
			{
				users.Read();
				this.userId = (int)users["User_id"];
				this.groupID = (int)users["Group_id"];
				this.accessLevel = (int)users["Access_level"];
				this.studentID = (int)users["Student_id"];
				this.name = (string)users["Name"];
				this.surname = (string)users["Surname"];
				this.secondName = (string)users["Second_name"];
				this.currentSemester = (int)users["Semester"];
				this.address = (string)users["Address"];
				this.telephone = (string)users["Telephone"];
				this.recordBook = (string)users["Record_book"];
				this.typeOfEducetion = (Student.TypeOfEducation)Enum.Parse(typeof(Student.TypeOfEducation), (string)users["Type_of_education"]);
				this.contactsParents = (string)users["ontacts_parents"];
				this.employmentInTheDepartment = (string)users["Employment_in_the_department"];

				/*
				 * Создаем сесию и куки
				 * */

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
			if (!this.IsExistsInDB()) return false;
			DB db = new DB();
			DB.ResponseTable users = null;
			if (this.email != string.Empty && this.passwodr != string.Empty)
				users = db.QueryToRespontTable(string.Format("select * from Student inner join Users on Student.Student_id = Users.Student_id where Email='{0}' and Password='{1}';", this.Email, this.Password));
			else
				if (this.userId != -1)
					users = db.QueryToRespontTable(string.Format("select * from Student inner join Users on Student.Student_id = Users.Student_id where User_id = {0};", this.userId));
			if (users != null && users.CountRow == 1)
			{
				users.Read();
				this.userId = (int)users["User_id"];
				this.accessLevel = (int)users["Access_level"];
				this.studentID = (int)users["Student_id"];
				this.groupID = (int)users["Group_id"];
				this.name = (string)users["Name"];
				this.surname = (string)users["Surname"];
				this.secondName = (string)users["Second_name"];
				this.currentSemester = (int)users["Semester"];
				this.address = (string)users["Address"];
				this.telephone = (string)users["Telephone"];
				this.recordBook = (string)users["Record_book"];
				this.typeOfEducetion = (Student.TypeOfEducation)Enum.Parse(typeof(Student.TypeOfEducation), (string)users["Type_of_education"]);
				this.contactsParents = (string)users["ontacts_parents"];
				this.employmentInTheDepartment = (string)users["Employment_in_the_department"];
				if (users["Student_id"] == null)
					this.userType = UserType.Lecturer;
				else
					this.userType = UserType.Student;
				return true;
			}
			return false;
		}
	}
}