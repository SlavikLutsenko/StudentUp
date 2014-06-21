using System;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий группу студентов
	/// </summary>
	public class Group : IPartInstitute
	{
		/// <summary>
		/// Идентификатор группы
		/// </summary>
		int groupID = -1;

		/// <summary>
		/// Идентификатор кафедры группы
		/// </summary>
		int departmentID = -1;

		/// <summary>
		/// Название группы
		/// </summary>
		string name;

		/// <summary>
		/// Email группы
		/// </summary>
		string emailGroup;
		
		/// <summary>
		/// Индуфикатор старосты группы
		/// </summary>
		int elderID = -1;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newGroupID">Индетификатор группы</param>
		public Group(int newGroupID)
		{
			if (newGroupID <= 0) throw new ValidationDataException("no id");
			this.groupID = newGroupID;
		}

		/// <summary>
		/// Возвращает инлетификатор группы
		/// </summary>
		public int ID { get { return this.groupID; } }

		/// <summary>
		/// Возвращает финдетификатор кафедры группы
		/// </summary>
		public int DepartmentID { get { return this.departmentID; } }

		/// <summary>
		/// Возвращает название группы
		/// </summary>
		public string Name { get { return this.name; } }

		/// <summary>
		/// Возвращает email группы
		/// </summary>
		public string EmailGroup { get { return this.emailGroup; } }

		/// <summary>
		/// Возвращает индетификатор старосты группы
		/// </summary>
		public int ElderID { get { return this.elderID; } }

		/// <summary>
		/// Проверяет существует ли такая группы
		/// </summary>
		/// <returns>true - существует, flase - не существует</returns>
		public bool IsExistsInDB()
		{
			DB db = new DB();
			DB.ResponseTable group = null;
			if (this.groupID != -1)
				group = db.QueryToRespontTable(string.Format("select * from Groups where Group_id = {0};", this.groupID));
			return group != null && group.CountRow == 1;
		}

		/// <summary>
		/// Берет информацию о группе из БД
		/// </summary>
		/// <returns>true - группа существует, flase - группы не существует</returns>
		public bool GetInformationAboutUserFromDB()
		{
			DB db = new DB();
			DB.ResponseTable group = null;
			if (this.groupID != -1)
				group = db.QueryToRespontTable(string.Format("select * from Groups where Group_id = {0};", this.groupID));
			if (group == null || group.CountRow <= 0) return false;
			group.Read();
			this.departmentID = Convert.ToInt32(group["Department_id"]);
			this.name = (string)group["Name"];
			this.emailGroup = (string)group["Email_group"];
			this.elderID = Convert.ToInt32(group["Elder_id"]);
			return true;
		}

		/// <summary>
		/// Возвращает список студентов которые находятся в этой группе
		/// </summary>
		/// <returns>Масиив студентов</returns>
		public Student[] GetStudent()
		{
			Student[] result = null;
			DB db = new DB();
			DB.ResponseTable studentID = db.QueryToRespontTable(string.Format("select Student_id from student where Group_id = {0};", groupID));
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
		/// Возвращает список предметов группы
		/// </summary>
		/// <returns>масив предметов</returns>
		public Subject[] GetSubjects()
		{
			Subject[] result = null;
			DB.ResponseTable subjectsID = (new DB()).QueryToRespontTable(string.Format("select studentsubject.Subject_id from groups inner join student inner join studentsubject on groups.Group_id = student.Group_id and student.Student_id = studentsubject.Student_id where groups.Group_id = {0} group by studentsubject.Subject_id;", this.ID));
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
		/// Устанавливает старосту группе
		/// </summary>
		/// <param name="studentID">Идентификатор студента</param>
		/// <returns>Студент который стал старостой</returns>
		public Student SetElder(int studentID)
		{
			DB db = new DB();
			db.QueryToRespontTable(string.Format("update groups set Elder_id = {0} where Group_id = {1};", studentID, this.ID));
			return new Student(studentID);
		}

		/// <summary>
		/// Добавляет группу
		/// </summary>
		/// <param name="nameGroup">Код группы</param>
		/// <param name="department">Кафедра группы</param>
		/// <returns>Новая группа</returns>
		public static Group AddGroup(string nameGroup, int department)
		{
			DB db = new DB();
			db.QueryToRespontTable(string.Format("insert into Groups(Department_id, Name) value ({0}, '{1}');", department, nameGroup));
			DB.ResponseTable idGoup = db.QueryToRespontTable("select LAST_INSERT_ID() as id;");
			idGoup.Read();
			Group group = new Group(Convert.ToInt32(idGoup["id"]));
			group.GetInformationAboutUserFromDB();
			return group;
		}
	}
}