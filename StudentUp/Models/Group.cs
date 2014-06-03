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
			this.groupID = (int)group["Group_id"];
			this.departmentID = (int)group["Department_id"];
			this.name = (string)group["Name"];
			this.emailGroup = (string)group["Email_group"];
			this.elderID = (int)group["Elder_id"];
			return true;
		}
	}
}