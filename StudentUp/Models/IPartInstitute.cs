namespace StudentUp.Models
{
	/// <summary>
	/// Описывает основыные свойства и методы любой части интитута
	/// </summary>
	interface IPartInstitute
	{
		/// <summary>
		/// Идентификатор объекта
		/// </summary>
		int ID { get; }

		/// <summary>
		/// ПРоверяет содержится ли данный объект в БД
		/// </summary>
		/// <returns>true - обект существует\n false - объекта не существует</returns>
		bool IsExistsInDB();

		/// <summary>
		/// Берет всю информацию об объекти из БД
		/// </summary>
		/// <returns>true - обект существует\n false - объекта не существует</returns>
		bool GetInformationAboutUserFromDB();
	}
}
