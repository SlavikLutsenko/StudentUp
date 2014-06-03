namespace StudentUp.Models
{
	public class Department : IPartInstitute
	{
		int departmentID = -1;

		string name;

		string facultyName;

		public Department(int newDepartmentID)
		{
			if (newDepartmentID <= 0) throw new ValidationDataException("no id");
			this.departmentID = newDepartmentID;
		}

		public Department(string newName, string newFacultyName)
		{
			this.name = newName;
			this.facultyName = newFacultyName;
		}

		public int ID { get { return this.departmentID; } }

		public bool IsExistsInDB()
		{
			DB db = new DB();
			DB.ResponseTable department = null;
			if (this.departmentID != -1)
				department = db.QueryToRespontTable(string.Format("select * from Department where Department_id = {0};", this.departmentID));
			else if (this.name != "" && this.facultyName != "")
				department = db.QueryToRespontTable(string.Format("select * from Department where Name='{0}' and Faculty_name='{1}';", this.name, this.departmentID));
			return department != null && department.CountRow == 1;
		}

		public bool GetInformationAboutUserFromDB()
		{
			DB db = new DB();
			DB.ResponseTable department = null;
			if (this.departmentID != -1)
				department = db.QueryToRespontTable(string.Format("select * from Department where Department_id = {0};", this.departmentID));
			else if (this.name != "" && this.facultyName != "")
				department = db.QueryToRespontTable(string.Format("select * from Department where Name='{0}' and Faculty_name='{1}';", this.name, this.departmentID));
			if (department == null || department.CountRow <= 0) return false;
			this.departmentID = (int)department["Department_id"];
			this.name = (string)department["Name"];
			this.facultyName = (string)department["Faculty_name"];
			return true;
		}
	}
}