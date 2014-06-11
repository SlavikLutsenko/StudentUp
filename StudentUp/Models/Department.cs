namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий кафедру
	/// </summary>
	public class Department : IPartInstitute
	{
		/// <summary>
		/// Идентификатор кафедры
		/// </summary>
		int departmentID = -1;

		/// <summary>
		/// Название кафедры
		/// </summary>
		string name;

		/// <summary>
		/// Факультетт кафедры
		/// </summary>
		string facultyName;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newDepartmentID">Идентификатор кафедры</param>
		public Department(int newDepartmentID)
		{
			if (newDepartmentID <= 0) throw new ValidationDataException("no id");
			this.departmentID = newDepartmentID;
		}

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newName">Название кафедры</param>
		/// <param name="newFacultyName">Название факультета кафедры</param>
		public Department(string newName, string newFacultyName)
		{
			this.name = newName;
			this.facultyName = newFacultyName;
		}

		/// <summary>
		/// Возвращает идентификатор кафедры
		/// </summary>
		public int ID { get { return this.departmentID; } }

		/// <summary>
		/// Возвращает название кафедры
		/// </summary>
		public string Name { get { return this.name; } }

		/// <summary>
		/// Возвращает название факультета кафедры
		/// </summary>
		public string FacultyName { get { return this.facultyName; } }

		/// <summary>
		/// Проверяет сущетвует ли такая кафедра в БД
		/// </summary>
		/// <returns>true - есть, false - нету</returns>
		public bool IsExistsInDB()
		{
			DB db = new DB();
			string query;
			if (this.departmentID != -1)
				query = string.Format("select * from Department where Department_id = {0};", this.departmentID);
			else
				if (this.name != "" && this.facultyName != "")
					query = string.Format("select * from Department where Name='{0}' and Faculty_name='{1}';", this.name, this.departmentID);
				else query = "";
			DB.ResponseTable department = db.QueryToRespontTable(query);
			return department != null && department.CountRow == 1;
		}

		/// <summary>
		/// Берет всю информацию о кафедре из БД
		/// </summary>
		/// <returns>true - есть, false - нету</returns>
		public bool GetInformationAboutUserFromDB()
		{
			DB db = new DB();
			string query;
			if (this.departmentID != -1)
				query = string.Format("select * from Department where Department_id = {0};", this.departmentID);
			else
				if (this.name != "" && this.facultyName != "")
					query = string.Format("select * from Department where Name='{0}' and Faculty_name='{1}';", this.name, this.departmentID);
				else query = "";
			DB.ResponseTable department = db.QueryToRespontTable(query);
			if (department == null || department.CountRow <= 0) return false;
			department.Read();
			this.departmentID = (int)department["Department_id"];
			this.name = (string)department["Name"];
			this.facultyName = (string)department["Faculty_name"];
			return true;
		}
	}
}