using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using StudentUp.Models;
using System.Configuration;

namespace StudentUp.Controllers
{
	/// <summary>
	/// Базовый контролер
	/// </summary>
	public class HomeController : Controller
	{
		/// <summary>
		/// Главная страница
		/// </summary>
		/// <returns>Возвращает главнуюстраницу</returns>
		public ActionResult Index()
		{
			if (Login() != null) return Redirect("/home");
			return View();
		}

		/// <summary>
		/// Производит авторизацию пользователей
		/// </summary>
		/// <param name="email">Email пользователя</param>
		/// <param name="password">Пароль пользователя</param>
		/// <returns>Если пользователь зарегистрирован и ввел правильные данные то он авторизуется иначе его перенаправят на главную страницу с соответствуещим сообщением. </returns>
		[HttpPost]
		public ActionResult Index(string email, string password)
		{
			try
			{
				Users user = new Users(email, password);
				if (!user.IsExistsInDB()) throw new ValidationDataException("no user");
				user.GetInformationAboutUserFromDB();
				user.CreateSession(Response);
				return Redirect("/home");
			}
			catch (ValidationDataException error)
			{
				Messages errorMessages = new Messages();
				foreach (Messages.Message mes in error.GetValue())
				{
					string message = "";
					switch (mes.Value)
					{
						case "no email":
							message = "Вы ввели не правильный формат email";
							break;
						case "no password":
							message = "Вы ввели не правильный формат пароля";
							break;
						case "no user":
							message = @"Пользователь с таким Email и паролем не зарегитсрирова или вы ввели неправильные данные.<br/> Возможно вы хотите <a href='/restorePassword'>востановить пароль</a>.";
							break;
					}
					errorMessages.Add(Messages.Message.TypeMessage.error, message);
				}
				ViewData["messages"] = errorMessages;
				return View();
			}
		}

		/// <summary>
		/// Проверяет залогинился ли уже пользователь
		/// </summary>
		/// <returns>
		/// true - да
		/// false - нет
		/// </returns>
		private Users Login()
		{
			Users user = null;
			if (Users.IsCreatedSession(Request))
			{
				user = Users.GetSession(Request);
				switch (user.Type)
				{
					case Users.UserType.Student:
						user = new Student(user);
						break;
					case Users.UserType.Lecturer:
						user = new Lecturer(user);
						break;
				}
				user.GetInformationAboutUserFromDB();
			}
			return user;
		}

		/// <summary>
		/// Главная страница пользователя
		/// </summary>
		/// <returns>Если пользователь авторизировался и у него созданна сессия, то у него загрузится эта страница иначе его перенаправят на страницу авторизации</returns>
		public ActionResult Home()
		{
			Users user = Login();
			if (user != null)
			{
				ViewData["user"] = user;
				return View();
			}
			return Redirect("/");
		}

		public ActionResult PersonalData()
		{
			Users user = Login();
			if (user != null)
			{
				ViewData["user"] = user;
				return View();
			}
			return Redirect("/");
		}

		[HttpPost]
		public ActionResult EditLecturer(string name, string surname, string secondName, string telephone)
		{
			int userID = Convert.ToInt32(Request.Cookies["userID"].Value);
			DB db = new DB();
			db.QueryToRespontTable(string.Format("update lecturer set Name = '{0}', Surname = '{1}', Second_name = '{2}', Telephone = '{3}'", name, surname, secondName, telephone));
			return Redirect("/PersonalData");
		}

		[HttpPost]
		public ActionResult EditStudent(string name, string surname, string secondName, string telephone, string address, string contactsParents)
		{
			int userID = Convert.ToInt32(Request.Cookies["userID"].Value);
			DB db = new DB();
			db.QueryToRespontTable(string.Format("update student set Name = '{0}', Surname = '{1}', Second_name = '{2}', Telephone = '{3}', Address = '{4}', Сontacts_parents = {5};", name, surname, secondName, telephone, address, contactsParents));
			return Redirect("/PersonalData");
		}

		/// <summary>
		/// Твраница администрирования сайта
		/// </summary>
		/// <returns>Возвращает страницу администрирования если пользователь зарегистрирован иначе перенаправляет на главную страницу</returns>
		public ActionResult Admin()
		{
			Users user = Login();
			if (user != null)
			{
				DB db = new DB();
				DB.ResponseTable responseTable;
				ViewData["user"] = user;
				ViewData["departments"] = db.QueryToRespontTable("select * from Department;");
				ViewData["groups"] = db.QueryToRespontTable("select * from Groups;");
				responseTable = db.QueryToRespontTable("show columns from Student like 'Type_of_education';");
				responseTable.Read();
				ViewData["typeEducation"] = ((string)responseTable["Type"]).Replace("enum('", "").Replace("')", "").Replace("'", "").Split(',');
				ViewData["lecturers"] = db.QueryToRespontTable("select Lecturer_id, Name, Surname, Second_name from Lecturer;");
				responseTable = db.QueryToRespontTable("show columns from Subject like 'Exam_type';");
				responseTable.Read();
				ViewData["examType"] = ((string)responseTable["Type"]).Replace("enum('", "").Replace("')", "").Replace("'", "").Split(',');
				ViewData["students"] = db.QueryToRespontTable("select * from Student;");
				ViewData["subjects"] = db.QueryToRespontTable("select * from Subject;");
				return View();
			}
			return Redirect("/");
		}

		/// <summary>
		/// Добавляет группу
		/// </summary>
		/// <param name="name">Код группы</param>
		/// <param name="department">Кафедра группы</param>
		/// <returns>Возвращает страницу администрирования и сообщения про добавленную группу</returns>
		[HttpPost]
		public ActionResult AddGroup(string name, int department)
		{
			Group.AddGroup(name, department);
			Messages messages = new Messages { { Messages.Message.TypeMessage.good, string.Format("Группа {0} добавленна", name) } };
			TempData["messages"] = messages;
			return Redirect("/Admin");
		}

		/// <summary>
		/// Добовляет пользователя - преподователя
		/// </summary>
		/// <param name="name">Имя преподователя</param>
		/// <param name="surname">Фамилия преподователя</param>
		/// <param name="secondName">Отчество преподователя</param>
		/// <param name="email">Email преподователя</param>
		/// <param name="position">Должность преподователя</param>
		/// <param name="telephone">Телефон преподователя</param>
		/// <param name="department">Кафедра где преподователь будет работать</param>
		/// <param name="admin">Будет ли преподователь администратором</param>
		/// <returns>Возвращает страницу администрирования и сообщения про добавленных пользователей</returns>
		[HttpPost]
		public ActionResult AddLecturer(string name, string surname, string secondName, string email, string position, string telephone, int department, string admin = "off")
		{
			Messages messages = new Messages();
			try
			{
				Lecturer.AddLecturer(name, surname, secondName, email, position, telephone, department, admin == "on");
				messages.Add(Messages.Message.TypeMessage.good, string.Format("Пользователь {0} {1} {2} был добавлен", name, surname, secondName));
			}
			catch (ValidationDataException error)
			{
				Messages erorrMessages = error.GetValue();
				foreach (Messages.Message item in erorrMessages)
				{
					switch (item.Value)
					{
						case "no email":
							messages.Add(Messages.Message.TypeMessage.error, "Вы ввели не правильный email");
							break;
					}
				}
			}
			TempData["messages"] = messages;
			return Redirect("/Admin");
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
		/// <returns>Возвращает страницу администрирования и сообщения про добавленных пользователей</returns>
		[HttpPost]
		public ActionResult AddStudent(string name, string surname, string secondName, string email, string telephone, int group, int currentSemestr, string address, string recordBook, string typeOfEducation, string contactsParents, string employmentInTheDepartment, string admin = "off")
		{
			Messages messages = new Messages();
			try
			{
				Student.AddStudent(name, surname, secondName, email, telephone, group, currentSemestr, address, recordBook,
					typeOfEducation, contactsParents, employmentInTheDepartment, admin == "on");
				messages.Add(Messages.Message.TypeMessage.good, string.Format("Пользователь {0} {1} {2} был добавлен", name, surname, secondName));
			}
			catch (ValidationDataException error)
			{
				Messages erorrMessages = error.GetValue();
				foreach (Messages.Message item in erorrMessages)
				{
					switch (item.Value)
					{
						case "no email":
							messages.Add(Messages.Message.TypeMessage.error, "Вы ввели не правильный email");
							break;
					}
				}
			}
			TempData["messages"] = messages;
			return Redirect("/Admin");
		}

		/// <summary>
		/// Добовляет предмет в БД
		/// </summary>
		/// <param name="name">Название предмета</param>
		/// <param name="lecturer">Идентификатор преподователя ведущий этот предмет</param>
		/// <param name="examType">Тип сдачи предмета</param>
		/// <returns>Возвращает страницу администрирования и сообщения про добавленных предметах</returns>
		[HttpPost]
		public ActionResult AddSubject(string name, int lecturer, string examType)
		{
			Messages messages = new Messages();
			if (Subject.AddSubject(name, lecturer, examType) != null)
				messages.Add(Messages.Message.TypeMessage.good, string.Format("Предмет {0} был добавлен", name));
			else
				messages.Add(Messages.Message.TypeMessage.error, "Вы указали не правильный тип сдачи предмета");
			TempData["messages"] = messages;
			return Redirect("/Admin");
		}

		/// <summary>
		/// Устанавливает предметы студентам
		/// </summary>
		/// <param name="subjectID">Индетификатор предмета</param>
		/// <param name="students">Массив индетификаторов студентов</param>
		/// <returns>Возвращает страницу администрирования и сообщения про результат действия</returns>
		public ActionResult SetSubjectForStudent(int subjectID, int[] students)
		{
			Subject subject = new Subject(subjectID);
			if (subject.IsExistsInDB())
				subject.SetStudent(students);
			Messages messages = new Messages { { Messages.Message.TypeMessage.good, "OK" } };
			TempData["messages"] = messages;
			return Redirect("/Admin");
		}

		/// <summary>
		/// Возващает страницу с предметами пользователя
		/// </summary>
		/// <returns>Страница</returns>
		public ActionResult MySubject()
		{
			Users user = Login();
			if (user != null)
			{
				ViewData["user"] = user;
				ViewData["mySubjects"] = user.GetMySubjects();
				return View();
			}
			return Redirect("/");
		}

		/// <summary>
		/// Выводит страничку с оценками пользователя
		/// </summary>
		/// <param name="subjectID">Идентификатор предмета по которому нужно вывести оценки</param>
		/// <returns>Страница</returns>
		public ActionResult MyMark(int subjectID = 0)
		{
			Users user = Login();
			if (user != null)
			{
				ViewData["user"] = user;
				ViewData["myMarks"] = user.GetMyMarks();
				return View();
			}
			return Redirect("/");
		}

		/// <summary>
		/// Выводит страницу для добавления оценки
		/// </summary>
		/// <returns>Страница</returns>
		public ActionResult AddMark()
		{
			Users user = Login();
			if (user != null)
			{
				if (user.Type == Users.UserType.Lecturer)
				{
					DB db = new DB();
					DB.ResponseTable responseTable;
					ViewData["user"] = user;
					ViewData["mySubjects"] = user.GetMySubjects();
					ViewData["myStudents"] = db.QueryToRespontTable(string.Format("select student.Student_id, student.Group_id, student.Name, student.Surname, student.Second_name from student inner join studentsubject inner join subject on student.Student_id = studentsubject.Student_id and subject.Subject_id = studentsubject.Subject_id and subject.Lecturer_id = {0};", user.LecturerID));
					responseTable = db.QueryToRespontTable("show columns from Marks like 'Type_marks';");
					responseTable.Read();
					ViewData["typeMarks"] = ((string)responseTable["Type"]).Replace("enum('", "").Replace("')", "").Replace("'", "").Split(',');
					return View();
				}
				return Redirect("/MyMark");
			}
			return Redirect("/");
		}

		/// <summary>
		/// Добавляет оценку
		/// </summary>
		/// <param name="student">Студент которому ставится оценка</param>
		/// <param name="subject">Предмет по которому ставится оценка</param>
		/// <param name="mark">Оценка</param>
		/// <param name="bonusMark">Бонусные балы</param>
		/// <param name="maxMark">максимальная оценка</param>
		/// <param name="typeMark">Тип оценки</param>
		/// <param name="date">Дата выставления оценки</param>
		/// <returns>Страница с оценками пользователя</returns>
		[HttpPost]
		public ActionResult AddMark(int student, int subject, int mark, int bonusMark, int maxMark, string typeMark, DateTime date)
		{
			Marks.AddMark(student, subject, mark, bonusMark, maxMark, Marks.ConverStringToEnum(typeMark), date);
			return Redirect("/MyMark");
		}

		/// <summary>
		/// Производит выход пользователя
		/// </summary>
		/// <returns>Главная страница</returns>
		public ActionResult Exit()
		{
			Users.DeleteSession(Response, Request);
			return Redirect("/");
		}

		/// <summary>
		/// Возвращает страницу для востановления пароля
		/// </summary>
		/// <returns>Страница</returns>
		public ActionResult RestorePassword()
		{
			if (TempData["idRestorePassword"] != null && (string)TempData["idRestorePassword"] != "")
				ViewData["messages"] = new Messages
				{
					{
						Messages.Message.TypeMessage.good, string.Format(
							"На вашу почту было отправленно письмо с инструкцией по востанавлению пароля. Если вам не пришло письмо, мы можем его <a href='http://{0}/ResendEmailRestorePassword?idRestorePassword={1}'>отправить повторно</a>",
							Request.Url.Authority, TempData["idRestorePassword"])
					}
				};
			return View();
		}

		/// <summary>
		/// Отправляет письмо для востановления пароля если польщователь зарегистрировался
		/// </summary>
		/// <param name="email">Email пользователя</param>
		/// <returns>Страница</returns>
		[HttpPost]
		public ActionResult RestorePassword(string email)
		{
			Messages messages = new Messages();
			string idRestorePassword = "";
			try
			{
				Users user = new Users(email);
				if (!user.IsExistsInDB()) throw new ValidationDataException("no user");
				user.GetInformationAboutUserFromDB();
				idRestorePassword = Validation.StringToMd5Hash(user.Email + user.Password);
				(new DB()).QueryToRespontTable(string.Format("insert into RestorePassword(Id, User_id) value ('{0}', {1});",
					idRestorePassword, user.ID));
				Mail.SendMail("smtp.gmail.com",
								ConfigurationManager.AppSettings.Get("AIDemail"),
								ConfigurationManager.AppSettings.Get("AIDpassword"),
								user.Email,
								"Востановление пароля",
								"Здравствуйте, " + user.Email +
								"\n\nВы попросили востановить парольна сайте studentUp.com. Если вы этого не делали игнорируйте это сообщений.\n\n" +
								"Для востановления пароля перейдите по ниже указзанной ссылке:\n\n" +
								"http://" + Request.Url.Authority + "/RestorePasswordUser?idRestorePassword=" + idRestorePassword);
				messages.Add(Messages.Message.TypeMessage.good, string.Format("На вашу почту было отправленно письмо с инструкцией по востанавлению пароля. Если вам не пришло письмо, мы можем его <a href='http://{0}/ResendEmailRestorePassword?idRestorePassword={1}'>отправить повторно</a>", Request.Url.Authority, idRestorePassword));
			}
			catch (ValidationDataException error)
			{
				foreach (Messages.Message mes in error.GetValue())
				{
					string message = "";
					switch (mes.Value)
					{
						case "no email":
							message = "Вы ввели не правильный формат email";
							break;
						case "no user":
							message = "Пользователь с таким email не зарегистрирован. Проверте правильно ли Вы ввели свой email.";
							break;
					}
					messages.Add(Messages.Message.TypeMessage.error, message);
				}
			}
			catch (MySqlException a)
			{
				if (a.Number == 1062)
					messages.Add(Messages.Message.TypeMessage.good, string.Format("Вам письмо было уже отправленно. Проверте почту. Если вам не пришло письмо, мы можем его <a href='http://{0}/ResendEmailRestorePassword?idRestorePassword={1}'>отправить повторно</a>", Request.Url.Authority, idRestorePassword));
			}
			ViewData["messages"] = messages;
			return View();
		}

		/// <summary>
		/// Позволяет пользователю ввести новый свой пароль. Если идентификатор сессии востановления пароля не был найден в БД, то пользователь будет перенаправлен на страницу ввода своего email для востановления пароля
		/// </summary>
		/// <param name="idRestorePassword">Идентификатор сессии востановления пароля</param>
		/// <returns>Странница</returns>
		public ActionResult RestorePasswordUser(string idRestorePassword)
		{
			if ((new DB()).QueryToRespontTable(string.Format("select User_id from RestorePassword where Id = \"{0}\";", idRestorePassword)) == null)
				return Redirect("/restorepassword");
			ViewData["id"] = idRestorePassword;
			return View();
		}

		/// <summary>
		/// Производит замену пароля пользователя на новый указанны им же. Если идентификатор сессии востановления пароля не был найден в БД, то пользователь будет перенаправлен на страницу ввода своего email для востановления пароля. Если пароли не совпадают пользователь будет должен заново их ввести
		/// </summary>
		/// <param name="idRestorePassword">Идентификатор сессии востановления пароля</param>
		/// <param name="password">Пароль</param>
		/// <param name="replacePassword">Подтверждение пароля</param>
		/// <returns>Страница</returns>
		[HttpPost]
		public ActionResult RestorePasswordUser(string idRestorePassword, string password, string replacePassword)
		{
			DB db = new DB();
			DB.ResponseTable user = db.QueryToRespontTable(string.Format("select User_id from RestorePassword where Id = '{0}';", idRestorePassword));
			if (user == null) return Redirect("/restorepassword");
			Messages messages = new Messages();
			if (password == "") messages.Add(Messages.Message.TypeMessage.error, "Вы не ввели новый пароль");
			else
				if (password != replacePassword) messages.Add(Messages.Message.TypeMessage.error, "Пароли должны совпадать");
				else
				{
					user.Read();
					db.QueryToRespontTable(string.Format("update users set Password = '{0}' where User_id = {1};" +
														 "delete from RestorePassword where Id = '{2}';", password, user["User_id"], idRestorePassword));
					messages.Add(Messages.Message.TypeMessage.good, string.Format("Пароль был востановлен. Авторизируйтесь в системе используя толькочто созданный пароль.<br /> <a href='http://{0}/'>Вход</a>", Request.Url.Authority));
				}
			ViewData["messages"] = messages;
			return View();
		}

		/// <summary>
		/// Отправляет заново письмо по востановлению пароля пользоввтелю
		/// </summary>
		/// <param name="idRestorePassword">Индефикатор сессии востановления</param>
		/// <returns>Сообщение об отправке письма</returns>
		public ActionResult ResendEmailRestorePassword(string idRestorePassword)
		{
			DB.ResponseTable restoreSession = (new DB()).QueryToRespontTable(string.Format("select User_id from RestorePassword where Id = \"{0}\";", idRestorePassword));
			if ((new DB()).QueryToRespontTable(string.Format("select User_id from RestorePassword where Id = \"{0}\";", idRestorePassword)) == null)
				return Redirect("/restorepassword");
			restoreSession.Read();
			Users user = new Users((int)restoreSession["User_id"]);
			user.GetInformationAboutUserFromDB();
			Mail.SendMail("smtp.gmail.com",
						  ConfigurationManager.AppSettings.Get("AIDemail"),
						  ConfigurationManager.AppSettings.Get("AIDpassword"),
						  user.Email,
						  "Востановление пароля",
						  "Здравствуйте, " + user.Email +
						  "\n\nВы попросили востановить парольна сайте studentUp.com. Если вы этого не делали игнорируйте это сообщений.\n\n" +
						  "Для востановления пароля перейдите по ниже указзанной ссылке:\n\n" +
						  "http://" + Request.Url.Authority + "/RestorePasswordUser?idRestorePassword=" + idRestorePassword);
			TempData["idRestorePassword"] = idRestorePassword;
			return Redirect("/RestorePassword");
		}
	}
}
