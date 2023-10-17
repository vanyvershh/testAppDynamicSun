using System.ComponentModel.DataAnnotations;

namespace testAppDynamicSun.Domain
{
    public class WeatherValue
    {
        //свойство Id будет служить первичным ключом в соответствующей таблице в БД
        public Guid Id { get; set; }

        //[Required(ErrorMessage = "Заполните дату")]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        //миграция не дает создать с типами dateonly, timeonly

        [Display(Name = "Время")]
        public string? Time { get; set; }
        
        [Display(Name = "Температура")]
        public string? Temperature { get; set; }

        [Display(Name = "Отн.влажность")]
        public string? Humidity { get; set; }

        [Display(Name = "Точка росы")]
        public string? Td { get; set; }

        [Display(Name = "Атм.давление")]
        public string? Pressure { get; set; }

        [Display(Name = "направление ветра")]
        public string? WindDirection { get; set; }

        [Display(Name = "Скорость ветра")]
        public string? WindSpeed { get; set; }

        [Display(Name = "Облачность")]
        public string? Cloudiness { get; set; }

        [Display(Name = "Нижняя граница облачности")]
        public string? CloudBase { get; set; }

        [Display(Name = "Горизонтальная видимость")]
        public string? Visibility { get; set; }

        [Display(Name = "Погодные явления")]
        public string? Conditions { get; set; }
    }
}
